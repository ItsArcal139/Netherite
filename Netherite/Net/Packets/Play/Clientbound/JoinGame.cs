﻿using Netherite.Data.Entities;
using Netherite.Entities;
using Netherite.Worlds;
using Netherite.Worlds.Dimensions;
using System.Collections.Generic;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class JoinGame : Packet
    {
        public Player Player { get; set; }

        public bool IsHardcore { get; set; }

        public GameMode Mode { get; set; }

        public GameMode? PreviousMode { get; set; } = null;

        public ICollection<Identifier> Worlds { get; set; }

        public Dimension Dimension { get; set; }

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
