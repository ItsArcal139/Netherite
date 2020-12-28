using Netherite.Data.Entities;
using Netherite.Worlds;
using System.Collections.Generic;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class JoinGame : Packet
    {
        public int EntityID { get; set; }

        public bool IsHardcore { get; set; }

        public GameMode Mode { get; set; }

        public GameMode? PreviousMode { get; set; } = null;

        public ICollection<Identifier> Worlds { get; set; }

        public Identifier WorldName { get; set; }

        public long Seed { get; set; }

        public int MaxPlayers { get; set; }

        public int ViewDistance { get; set; }

        public bool ReducedDebugInfo { get; set; }

        public bool EnableRespawnScreen { get; set; }

        public bool IsDebugWorld { get; set; }

        public bool IsFlatWorld { get; set; }
    }
}
