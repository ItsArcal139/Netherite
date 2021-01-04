using Netherite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class SpawnPlayer : Packet
    {
        public Player Player { get; set; }

        [Obsolete]
        public short CurrentItem { get; set; }
        [Obsolete]
        public PlayerMetadata Metadata { get; set; }
    }
}
