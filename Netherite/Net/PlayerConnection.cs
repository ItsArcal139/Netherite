using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using Netherite.Net.Packets;
using System.Collections.Generic;
using Netherite.Texts;
using Netherite.Net.Protocols;
using Netherite.Net.IO;
using Netherite.Data.Entities;
using Netherite.Worlds;
using Netherite.Utils;
using System.Diagnostics;
using Netherite.Net.Packets.Login.Serverbound;
using Netherite.Net.Packets.Login.Clientbound;
using Netherite.Net.Packets.Status;
using Netherite.Net.Packets.Handshake.Serverbound;
using Netherite.Net.Packets.Status.Serverbound;
using Netherite.Net.Packets.Status.Clientbound;
using Netherite.Net.Packets.Play.Clientbound;
using Netherite.Net.Packets.Play;
using Netherite.Entities;
using Netherite.Auth;
using Netherite.Physics;
using Netherite.Utils.Mojang;
using System.Linq;
using System.Security.Cryptography;
using System.Numerics;
using Vector3 = Netherite.Physics.Vector3;
using Netherite.Blocks;
using System.IO;

namespace Netherite.Net
{
    public class PlayerConnection
    {
        private PacketCryptography crypto = new PacketCryptography();

        public PacketState CurrentState { get; private set; } = PacketState.Handshake;

        public Socket Handle { get; private set; }

        public bool Connected { get; private set; } = true;

        public Protocol Protocol { get; private set; } = Protocol.FromVersion(-1);

        private int clientProtocol = -1;

        private byte[] randomToken;

        private byte[] sharedKey;

        public Player Player { get; set; }

        public Server Server { get; set; }
        public bool EncryptionEnabled { get; private set; }

        private MinecraftStream stream;

        public PlayerConnection(Server server, Socket handle)
        {
            Server = server;
            Handle = handle;
            stream = new MinecraftStream(new NetworkStream(Handle));

            Disconnected += () =>
            {
                if (Player != null)
                {
                    Logger.Info(
                        TranslateText.Of("{0} {1} ")
                            .AddWith(
                                LiteralText.Of("<->").SetColor(TextColor.DarkGray)
                            )
                            .AddWith(
                                LiteralText.Of($"[{Player?.Name ?? Handle.RemoteEndPoint.ToString()}]").SetColor(TextColor.DarkGray)
                            )
                            .AddExtra(
                                LiteralText.Of("has disconnected.")
                            ));

                    _ = Server.BroadcastPacket(new PlayerInfo
                    {
                        Action = PlayerInfo.PacketAction.RemovePlayer,
                        Players = new List<PlayerInfo.Meta>
                    {
                        new PlayerInfo.Meta
                        {
                            Profile = Player.Profile
                        }
                    }
                    });

                    _ = Server.BroadcastPacket(new DestroyEntityPacket
                    {
                        Entities = new List<Entity>
                        {
                            Player
                        }
                    });
                }
            };

            ReceivedPacket += async p =>
            {
                Logger.LogPacket(
                    TranslateText.Of("{0} {1} ")
                        .AddWith(
                            LiteralText.Of("<--").SetColor(TextColor.Aqua)
                        )
                        .AddWith(
                            LiteralText.Of($"[{Player?.Name ?? Handle.RemoteEndPoint.ToString()}]").SetColor(TextColor.DarkGray)
                        )
                        .AddExtra(
                            TranslateText.Of("Received: \t{0} \t{1}").AddWith(
                                LiteralText.Of(CurrentState.ToString() + ((byte)CurrentState > 0 ? "\t" : "")).SetColor(TextColor.Green),
                                LiteralText.Of(p.GetType().Name).SetColor(TextColor.Gold)
                            )
                        ));

                if (p is SetProtocol rp)
                {
                    clientProtocol = rp.ProtocolVersion;

                    var state = rp.NextState;
                    if (state != PacketState.Login && state != PacketState.Status)
                    {
                        await DisconnectAsync($"Expected state to be 1 or 2, found {(int)state} ({state})");
                    }
                    CurrentState = state;
                }

                if (p is StatusRequest)
                {
                    var r = new ProtocolResponse
                    {
                        Description = LiteralText.Of("Netherite"),
                        Players = new ProtocolResponse.PlayersMeta()
                        {
                            Max = 1
                        },
                        Version = new ProtocolResponse.VersionMeta()
                        {
                            Name = "Netherite 0.1 (MC 1.16.4)",
                            Protocol = Protocol.HasVersion(clientProtocol) ? clientProtocol : Protocol.LatestVersion
                        }
                    };

                    await SendPacketAsync(new StatusResponse(r));
                }

                if (p is PingPong ping)
                {
                    await SendPacketAsync(p);
                }

                if (p is LoginStart start)
                {
                    Protocol target = null;
                    try
                    {
                        target = Protocol.FromVersion(clientProtocol);
                        Protocol = target;

                        Guid g = Guid.NewGuid();
                        string name = start.UserName;

                        if (Server.OnlineMode)
                        {
                            var onlineProfile = await MinecraftAPI.GetUserAsync(name);

                            Player = new Player(Server.Worlds[0], onlineProfile, this);

                            crypto.GenerateKeyPair();
                            var val = crypto.GeneratePublicKeyAndToken();
                            randomToken = val.randomToken;

                            await SendPacketAsync(new EncryptionRequest
                            {
                                ServerID = "",
                                PublicKey = val.publicKey,
                                VerifyToken = val.randomToken
                            });

                            Logger.Log(
                                TranslateText.Of("{0} {1} ")
                                    .AddWith(
                                        LiteralText.Of("<->").SetColor(TextColor.DarkGray)
                                    )
                                    .AddWith(
                                        LiteralText.Of($"[{Player?.Name ?? Handle.RemoteEndPoint.ToString()}]").SetColor(TextColor.DarkGray)
                                    )
                                    .AddExtra(
                                        LiteralText.Of("is logging in...")
                                    ));
                        }
                        else
                        {
                            Logger.Warn($"**** Netherite is in OFFLINE mode!! ****");
                            GameProfile profile = new GameProfile(g, name);
                            Player = new Player(Server.Worlds[0], profile, this);

                            Logger.Info(
                                TranslateText.Of("{0} {1} ")
                                    .AddWith(
                                        LiteralText.Of("<->").SetColor(TextColor.DarkGray)
                                    )
                                    .AddWith(
                                        LiteralText.Of($"[{Player?.Name ?? Handle.RemoteEndPoint.ToString()}]").SetColor(TextColor.DarkGray)
                                    )
                                    .AddExtra(
                                        LiteralText.Of("has connected.")
                                    ));

                            await ConnectAsync();
                        }
                    }
                    catch (PlatformNotSupportedException)
                    {
                        string supported = "";
                        foreach (Protocol pt in Protocol.LoadedProtocols)
                        {
                            supported += ", " + pt.VersionName;
                        }

                        await SendPacketAsync(new LoginKick(LiteralText.Of("[Netherite] ")
                            .AddExtra(
                                LiteralText.Of("Netherite currently supports " + supported[2..]).SetColor(TextColor.Red)
                            )));
                    }
                }

                if (p is EncryptionResponse er)
                {
                    sharedKey = crypto.Decrypt(er.SharedSecret);
                    var decrypted = crypto.Decrypt(er.VerifyToken);

                    if (!decrypted.SequenceEqual(randomToken))
                    {
                        await DisconnectAsync("Invalid token!");
                        return;
                    }

                    var serverId = MinecraftShaDigest(sharedKey.Concat(crypto.PublicKey).ToArray());

                    var resp = await MinecraftAPI.HasJoined(Player.Name, serverId);
                    if (resp is null)
                    {
                        Logger.Error(
                            LiteralText.Of($"Failed to auth {Player.Name}")
                        );
                        await DisconnectAsync("Unable to authenticate..");
                        return;
                    }

                    Player.Profile = resp;
                    Logger.Info(
                        TranslateText.Of("{0}'s UUID is {1}")
                            .AddWith(
                                LiteralText.Of(Player.Name).SetColor(TextColor.Gold)
                            )
                            .AddWith(
                                LiteralText.Of(Player.Guid.ToString()).SetColor(TextColor.Green)
                            )
                        );

                    EncryptionEnabled = true;
                    stream = new AesStream(stream.BaseStream, sharedKey);

                    await ConnectAsync();
                }
            };
        }

        // https://gist.github.com/ammaraskar/7b4a3f73bee9dc4136539644a0f27e63
        private string MinecraftShaDigest(byte[] data)
        {
            var hash = new SHA1Managed().ComputeHash(data);
            // Reverse the bytes since BigInteger uses little endian
            Array.Reverse(hash);

            var b = new BigInteger(hash);
            // very annoyingly, BigInteger in C# tries to be smart and puts in
            // a leading 0 when formatting as a hex number to allow roundtripping 
            // of negative numbers, thus we have to trim it off.
            if (b < 0)
            {
                // toss in a negative sign if the interpreted number is negative
                return $"-{(-b).ToString("x").TrimStart('0')}";
            }
            else
            {
                return b.ToString("x").TrimStart('0');
            }
        }

        public async Task ConnectAsync()
        {
            Logger.Info(
                TranslateText.Of("{0} logged in with protocol {1}, {2} -> {3}")
                    .AddWith(
                        LiteralText.Of(Player.Name).SetColor(TextColor.Gold)
                    )
                    .AddWith(
                        LiteralText.Of(Protocol.VersionName).SetColor(TextColor.Gold)
                    )
                    .AddWith(
                        LiteralText.Of(Protocol.Version.ToString()).SetColor(TextColor.Gold)
                    )
                    .AddWith(
                        Text.RepresentType(Protocol.GetType())
                    )
            );

            await SendPacketAsync(new LoginSuccess(Player.Guid, Player.Name));
            CurrentState = PacketState.Play;

            var join = new JoinGame
            {
                Player = Player,
                IsHardcore = false,
                Mode = Player.Mode,
                PreviousMode = null,
                Worlds = new List<Identifier>
                {
                    new Identifier("overworld"),
                    new Identifier("the_nether"),
                    new Identifier("the_end")
                },
                Dimension = Player.World.Dimension,
                WorldName = new Identifier("overworld"),
                Seed = 0L,
                MaxPlayers = 1,
                ViewDistance = 12,
                ReducedDebugInfo = false,
                EnableRespawnScreen = true,
                IsDebugWorld = false,
                IsFlatWorld = true
            };

            await SendPacketAsync(join);

            StartKeepAliveLoop();

            await SendServerBrand();

            await SendPacketAsync(new HeldItemChangePacket
            {
                Slot = 4
            });

            await SendPlayerInfo();

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    await SendPacketAsync(new ChunkDataPacket
                    {
                        Chunk = Player.World.GetChunk(i - 6, j - 6)
                    });
                    if (!Connected) break;
                }
                if (!Connected) break;
            }

            await SendPacketAsync(new BlockChange
            {
                Position = new Vector3
                {
                    X = 0,
                    Y = 1,
                    Z = 0
                },
                State = new BlockState(new Identifier("grass_block"))
            });

            Player.Position = new Vector3(0.5, 3, 0.5);

            foreach (var player in Server.OnlinePlayers)
            {
                if (player.Handle != Player.Handle)
                {
                    await SendPacketAsync(new SpawnPlayer
                    {
                        EntityID = player.Handle,
                        Guid = player.Guid,
                        X = player.Position.X,
                        Y = player.Position.Y,
                        Z = player.Position.Z,
                        Yaw = 0,
                        Pitch = 0,
                        CurrentItem = 0,
                        Metadata = player.Metadata
                    });

                    await player.Client.SendPacketAsync(new SpawnPlayer
                    {
                        EntityID = Player.Handle,
                        Guid = Player.Guid,
                        X = Player.Position.X,
                        Y = Player.Position.Y,
                        Z = Player.Position.Z,
                        Yaw = 0,
                        Pitch = 0,
                        CurrentItem = 0,
                        Metadata = player.Metadata
                    });
                }
            }

            await SendPacketAsync(new PlayerPositionAndLook
            {
                X = Player.Position.X,
                Y = Player.Position.Y,
                Z = Player.Position.Z
            });

            await SendPacketAsync(new ChatPacket(LiteralText.Of("[Netherite] Hello from Netherite"), ChatPacket.ChatPosition.Chat, Guid.NewGuid()));

            // await SendPacketAsync(new PacketPlayOutKick(LiteralText.Of("[Netherite] Wait until Netherite has been fully implemented!")));
        }

        public async Task DisconnectAsync(Text reason)
        {
            if (CurrentState == PacketState.Play)
            {
                await SendPacketAsync(new Kick(reason));
            }
            else
            {
                await SendPacketAsync(new LoginKick(reason));
            }
        }

        public Task DisconnectAsync(string reason) => DisconnectAsync(LiteralText.Of(reason));

        private async Task SendServerBrand()
        {
            await SendPacketAsync(new ServerBrand
            {
                Name = "Netherite"
            });
        }

        private async Task SendPlayerInfo()
        {
            foreach (var player in Server.OnlinePlayers)
            {
                var p = new PlayerInfo
                {
                    Action = PlayerInfo.PacketAction.AddPlayer,
                    Players = player == Player ? (
                        Server.OnlinePlayers.ConvertAll(pl =>
                        {
                            return new PlayerInfo.Meta
                            {
                                Profile = pl.Profile,
                                Mode = pl.Mode,
                                Latency = 0
                            };
                        })
                    ) : (new List<PlayerInfo.Meta>
                    {
                        new PlayerInfo.Meta
                        {
                            Profile = Player.Profile,
                            Mode = Player.Mode,
                            Latency = 0
                        }
                    })
                };

                await player.Client.SendPacketAsync(p);
            }
        }

        public event Action Disconnected;

        private SemaphoreSlim socketLock = new SemaphoreSlim(1, 1);

        public void StartLoop()
        {
            Task.Run(async () =>
            {
                while (Connected)
                {
                    try
                    {
                        while (Handle.Available == 0 && Handle.Connected)
                        {
                            if (Handle.Poll(0, SelectMode.SelectRead)) break;
                        }
                        if (Handle.Poll(0, SelectMode.SelectRead) && Handle.Available == 0) break;

                        byte[] content = new byte[Handle.Available];
                        // await Handle.ReceiveAsync(new ArraySegment<byte>(content), SocketFlags.None);
                        await stream.ReadAsync(content, 0, content.Length);

                        await ReadPacketAsync(content);
                    }
                    catch (Exception ex)
                    {
                        foreach (string line in ex.ToString().Split('\n'))
                        {
                            Logger.Error(LiteralText.Of(line));
                        }
                    }
                }
                Connected = false;
                Disconnected?.Invoke();
            });
        }

        public void StartKeepAliveLoop()
        {
            Task.Run(() =>
            {
                SemaphoreSlim queueLock = new SemaphoreSlim(1, 1);
                Random r = new Random();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                Task.Run(async () =>
                {
                    while (Connected)
                    {
                        if (stopwatch.Elapsed.TotalSeconds > 30)
                        {
                            await SendPacketAsync(new Kick(LiteralText.Of("[Netherite] Timed out")));
                        }
                    }
                });

                ReceivedPacket += p =>
                {
                    if (p is KeepAlivePacket)
                    {
                        stopwatch.Restart();
                    }
                };

                while (Connected)
                {
                    Task.Run(async () =>
                    {
                        long n = r.Next();
                        await SendPacketAsync(new KeepAlivePacket(n));
                    });

                    Thread.Sleep(5000);
                }
            });
        }

        public event Action<Packet> ReceivedPacket;

        /// <summary>
        /// 讀取玩家端傳送的封包。
        /// </summary>
        /// <param name="buffer">封包內容。</param>
        /// <returns></returns>
        private async Task ReadPacketAsync(byte[] buffer)
        {
            int index = 0;
            int length = buffer.Length;
            List<Packet> packets = new List<Packet>();

            while (length > index)
            {
                if (CurrentState != PacketState.Handshake || CurrentState == PacketState.Handshake && buffer[0] != 0xfe)
                {
                    BufferReader reader = new BufferReader(buffer);
                    byte[] raw = reader.ReadByteArray(out int len);
                    index += len;

                    BufferReader packetReader = new BufferReader(raw);

                    Packet p = Protocol.Read(CurrentState, packetReader);
                    ReceivedPacket?.Invoke(p);
                    _ = p.HandleAsync(Server, Player);
                }
                else
                {
                    index = length;
                }

                byte[] temp = new byte[length - index];
                Array.Copy(buffer, index, temp, 0, temp.Length);
                buffer = temp;

                length -= index;
                index = 0;
            }

            await Task.CompletedTask;
        }

        public event Action<Packet> SentPacket;

        private BufferWriter writer = new BufferWriter();

        private SemaphoreSlim writeLock = new SemaphoreSlim(1, 1);

        private int cdc = 0;

        /// <summary>
        /// 將指定的封包 <see cref="Packet"/> 傳送給玩家。
        /// </summary>
        /// <returns></returns>
        public async Task SendPacketAsync(Packet packet)
        {
            if (!Connected) return;

            await writeLock.WaitAsync();
            try
            {
                await Protocol.Write(packet, writer);
                byte[] buffer = writer.ToBuffer();

                if (packet is ChunkDataPacket)
                {
                    File.WriteAllBytes("dump" + (cdc++) + ".bin", buffer);
                }

                Logger.LogPacket(
                    TranslateText.Of("{0} {1} ")
                        .AddWith(
                            LiteralText.Of("-->").SetColor(TextColor.Green)
                        )
                        .AddWith(
                            LiteralText.Of($"[{Player?.Name ?? Handle.RemoteEndPoint.ToString()}]").SetColor(TextColor.DarkGray)
                        )
                        .AddExtra(
                            TranslateText.Of("Sending: \t{0} \t{1}: ").AddWith(
                                LiteralText.Of(CurrentState.ToString() + ((byte)CurrentState > 0 ? "\t" : "")).SetColor(TextColor.Green),
                                LiteralText.Of(packet.GetType().Name).SetColor(TextColor.Gold)
                            )
                        )
                        .AddExtra(
                            TranslateText.Of("[{0} bytes]").AddWith(
                                LiteralText.Of(buffer.Length + "").SetColor(TextColor.DarkGray)
                            ).SetColor(TextColor.DarkGray)
                        ));

                //await Handle.SendAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                await stream.WriteAsync(buffer);

                SentPacket?.Invoke(packet);
            }
            catch (Exception ex)
            {
                foreach (string line in ex.ToString().Split('\n'))
                {
                    Logger.Error(LiteralText.Of(line));
                }
            }
            finally
            {
                writeLock.Release();
            }
        }
    }
}
