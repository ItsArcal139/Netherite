using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Netherite.Net.IO;
using Netherite.Net.Packets;
using Netherite.Net.Packets.Handshake.Serverbound;
using Netherite.Net.Packets.Login.Serverbound;
using Netherite.Net.Protocols;
using Netherite.Texts;
using Netherite.Utils;

namespace Netherite.Net
{
    public class ServerConnection
    {
        private PacketCryptography crypto = new PacketCryptography();

        private int clientProtocol = -1;

        private byte[] randomToken;

        private byte[] sharedKey;

        /// <summary>
        /// The internal stream used by this connection.
        /// </summary>
        private MinecraftStream stream;

        /// <summary>
        /// The current protocol state of this connection.
        /// </summary>
        public PacketState CurrentState { get; set; } = PacketState.Handshake;

        /// <summary>
        /// The <see cref="Socket"/> handle of this connection.
        /// </summary>
        public Socket Handle { get; private set; }

        /// <summary>
        /// Whether the player is connected to this connection.
        /// </summary>
        public bool Connected { get; private set; } = true;

        /// <summary>
        /// The protocol to be used in this connection.
        /// </summary>
        public Protocol Protocol { get; private set; } = Protocol.FromVersion(754);

        // <summary>
        /// Whether this connection is being encrypted.
        /// </summary>
        public bool EncryptionEnabled { get; private set; }

        public CancellationTokenSource CancelTokenSource { get; private set; } = new CancellationTokenSource();

        public ServerConnection(Socket handle)
        {
            Handle = handle;
            stream = new MinecraftStream(new NetworkStream(handle));

            Disconnected += HandleDisconnection;
            ReceivedPacket += HandlePacketReceive;
        }

        private void HandleDisconnection()
        {
            Logger.Info(
                TranslateText.Of("{0} {1} ")
                    .AddWith(
                        LiteralText.Of("<->").SetColor(TextColor.DarkGray)
                    )
                    .AddWith(
                        LiteralText.Of($"[{Handle.RemoteEndPoint}]").SetColor(TextColor.DarkGray)
                    )
                    .AddExtra(
                        LiteralText.Of(" Disconnected.")
                    ));
        }

        private async void HandlePacketReceive(Packet p)
        {
            if (!p.IsConstantPacket)
            {
                Func<Text> b = () =>
                {
                    if (p is UnknownPacket up)
                    {
                        return TranslateText.Of(" [{0} bytes]").AddWith(
                            LiteralText.Of(up.buffer.Length + "").SetColor(TextColor.DarkGray)
                        ).SetColor(TextColor.DarkGray);
                    }
                    return LiteralText.Of("");
                };

                Logger.Verbose(
                    TranslateText.Of("{0} {1} ")
                        .AddWith(
                            LiteralText.Of("<-").SetColor(TextColor.Aqua)
                        )
                        .AddWith(
                            LiteralText.Of($"[{Handle.RemoteEndPoint}]").SetColor(TextColor.DarkGray)
                        )
                        .AddExtra(
                            TranslateText.Of("Received: \t{0} \t{1}").AddWith(
                                LiteralText.Of(CurrentState.ToString() + ((byte)CurrentState > 0 ? "\t" : "")).SetColor(TextColor.Green),
                                LiteralText.Of(p.GetType().Name).SetColor(TextColor.Gold)
                            )
                        )
                        .AddExtra(b()));
            }
        }

        public event Action Disconnected;

        public void StartLoop()
        {
            Task.Run(async () =>
            {
                BufferStorage storage = new BufferStorage();

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

                        storage.Store(content);
                        while (storage.CanReadAsPacket())
                        {
                            await ReadPacketAsync(storage.TakeOutAsPacket());
                        }
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

        public event Action<Packet> ReceivedPacket;

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

                    Packet p = Protocol.Read(ProtocolRole.Client, CurrentState, packetReader);
                    ReceivedPacket?.Invoke(p);

                    await p.ClientHandleAsync(this);
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

        /// <summary>
        /// Sends the given <see cref="Packet"/> to the client.
        /// </summary>
        /// <returns></returns>
        public async Task SendPacketAsync(Packet packet, CancellationTokenSource cts = null)
        {
            if (!Connected) return;

            await writeLock.WaitAsync();
            if (cts != null && cts.IsCancellationRequested)
            {
                writeLock.Release();
                return;
            }

            try
            {
                await Protocol.Write(ProtocolRole.Client, packet, writer);
                byte[] buffer = writer.ToBuffer();

                // Check the connection again
                if (!Connected) return;

                if (!packet.IsConstantPacket)
                    Logger.Verbose(
                        TranslateText.Of("{0} {1} ")
                            .AddWith(
                                LiteralText.Of("->").SetColor(TextColor.Green)
                            )
                            .AddWith(
                                LiteralText.Of($"[{Handle?.RemoteEndPoint}]").SetColor(TextColor.DarkGray)
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

        public async Task DisconnectAsync()
        {
            Connected = false;
            await Task.CompletedTask;
        }

        public async Task LoginAsync()
        {
            await SendPacketAsync(new SetProtocol
            {
                ProtocolVersion = Protocol.Version,
                ServerAddress = "addre.ss",
                ServerPort = 12345,
                NextState = PacketState.Login
            });

            CurrentState = PacketState.Login;

            await SendPacketAsync(new LoginStart
            {
                UserName = "Netherite"
            });
        }
    }
}
