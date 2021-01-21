using Netherite.Auth;
using Netherite.Net;
using Netherite.Net.Packets.Play.Clientbound;
using Netherite.Physics;
using Netherite.Texts;
using Netherite.Worlds;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Netherite.Entities
{
    public class Player : Entity, ICommandSender
    {
        public GameMode Mode { get; set; } = GameMode.Creative;

        public GameProfile Profile { get; set; }

        public PlayerConnection Client { get; set; }

        public new PlayerMetadata Metadata { get; set; } = new PlayerMetadata
        {
            Health = 20
        };

        public Player(World w, GameProfile profile, PlayerConnection client) : base(w)
        {
            Profile = profile;
            Client = client;
        }

        public new Guid Guid => Profile.Guid;

        public string Name => Profile.Name;

        public async Task SendMessageAsync(Text text)
        {
            await Client.SendPacketAsync(new ChatPacket(text, ChatPacket.ChatPosition.System, Guid.NewGuid()));
        }

        public async Task SendMessageAsync(string message) => await SendMessageAsync(LiteralText.Of(message));

        public void SendMessage(Text text) => _ = SendMessageAsync(text);

        public void SendMessage(string message) => _ = SendMessageAsync(message);

        public async Task ShowTitleAsync(Text title, Text subtitle, int fadeIn, int stay, int fadeOut)
        {
            await Client.SendPacketAsync(new DisplayTitle
            {
                Action = DisplayTitle.PacketAction.SetTitle,
                Title = title
            });

            await Client.SendPacketAsync(new DisplayTitle
            {
                Action = DisplayTitle.PacketAction.SetSubtitle,
                Subtitle = subtitle
            });

            await Client.SendPacketAsync(new DisplayTitle
            {
                Action = DisplayTitle.PacketAction.SetTimesAndDisplay,
                FadeIn = fadeIn,
                Stay = stay,
                FadeOut = fadeOut
            });
        }

        public void ShowTitle(Text title, Text subtitle, int fadeIn, int stay, int fadeOut) => _ = ShowTitleAsync(title, subtitle, fadeIn, stay, fadeOut);

        #region Player List - header & footer
        private Text listHeader = null;

        public Text ListHeader
        {
            get
            {
                return listHeader;
            }

            set
            {
                listHeader = value;
                SetPlayerListHeaderFooter(ListHeader, ListFooter);
            }
        }

        private Text listFooter = null;

        public Text ListFooter
        {
            get
            {
                return listFooter;
            }

            set
            {
                listFooter = value;
                SetPlayerListHeaderFooter(ListHeader, ListFooter);
            }
        }

        public async Task SetPlayerListHeaderFooterAsync(Text header, Text footer)
        {
            await Client.SendPacketAsync(new PlayerListHeaderAndFooter(header, footer));
        }

        public void SetPlayerListHeaderFooter(Text header, Text footer) => _ = SetPlayerListHeaderFooterAsync(header, footer);
        #endregion

        public Vector3 CompassTarget { get; set; }

        public EndPoint EndPoint => Client.Handle.RemoteEndPoint;

        public bool IsSleepingIgnored { get; set; }

        public Vector3 SpawnLocation { get; set; }

        public void PlayNote(Vector3 location, byte instrument, byte note)
        {
            throw new NotImplementedException();
        }

        #region Chunk load / unload
        public List<Chunk> LoadedChunks { get; } = new List<Chunk>();

        private Chunk lastChunk = null;

        private bool sentLook = false;

        public async Task UpdateChunks()
        {
            List<Chunk> nearby = new List<Chunk>();
            Chunk center = World.GetChunkByBlockPos((int)Position.X, (int)Position.Z);

            if (!sentLook)
            {
                _ = Client.SendPacketAsync(new ViewPosition
                {
                    ChunkX = center.X,
                    ChunkZ = center.Z
                });

                await Client.SendPacketAsync(new PlayerPositionAndLook
                {
                    X = Position.X,
                    Y = Position.Y + 1,
                    Z = Position.Z,
                    TeleportId = 9
                });
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    nearby.Add(World.GetChunk(center.X - i, center.Z - j));
                    nearby.Add(World.GetChunk(center.X + i, center.Z + j));
                }
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    nearby.Add(World.GetChunk(center.X - i, center.Z + j));
                    nearby.Add(World.GetChunk(center.X + i, center.Z - j));
                }
            }

            if (lastChunk == null)
            {
                lastChunk = center;
            }
            else
            {
                if (lastChunk != center)
                {
                    _ = Client.SendPacketAsync(new ViewPosition
                    {
                        ChunkX = center.X,
                        ChunkZ = center.Z
                    });
                    lastChunk = center;
                }
            }

            foreach (var chunk in nearby)
            {
                if (!LoadedChunks.Contains(chunk))
                {
                    await LoadChunkAsync(chunk);
                    LoadedChunks.Add(chunk);
                }

                if (!sentLook && chunk == center)
                {
                    await Client.SendPacketAsync(new PlayerPositionAndLook
                    {
                        X = Position.X,
                        Y = Position.Y,
                        Z = Position.Z,
                        TeleportId = 9
                    });
                    sentLook = true;
                }
            }

            var unloadList = LoadedChunks.FindAll(c => !nearby.Contains(c));
            foreach(var chunk in unloadList)
            {
                LoadedChunks.Remove(chunk);
                await UnloadChunkAsync(chunk);
            }
        }
        #endregion

        public override void Tick()
        {
            UpdateChunks().GetAwaiter().GetResult();
        }

        public async Task LoadChunkAsync(Chunk chunk)
        {
            await Client.SendPacketAsync(new ChunkDataPacket
            {
                Chunk = chunk
            });
        }

        public async Task UnloadChunkAsync(Chunk chunk)
        {
            await Client.SendPacketAsync(new UnloadChunkPacket
            {
                Chunk = chunk
            });
        }
    }
}
