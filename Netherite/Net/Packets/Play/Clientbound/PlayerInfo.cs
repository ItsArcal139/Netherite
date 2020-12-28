using Netherite.Auth;
using Netherite.Texts;
using Netherite.Worlds;
using System.Collections.Generic;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class PlayerInfo : Packet
    {
        public class Meta
        {
            public GameProfile Profile { get; set; }
            public GameMode Mode { get; set; }
            public Text DisplayName { get; set; }

            /// <summary>
            /// 玩家的來回延遲時間，以毫秒為單位。
            /// </summary>
            public int Latency { get; set; }
        }

        public enum PacketAction
        {
            AddPlayer, UpdateGameMode, UpdateLatency, UpdateDisplayName, RemovePlayer
        }

        public PacketAction Action { get; set; }

        public List<Meta> Players { get; set; }

        public PlayerInfo() : base() { }

        public PlayerInfo(PacketAction action, params Meta[] players) : this()
        {
            Action = action;
            Players.AddRange(players);
        }
    }
}
