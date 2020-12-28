using Netherite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class SpawnPlayer : Packet
    {
        public int EntityID { get; set; }
        public Guid Guid { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public short CurrentItem { get; set; }
        public PlayerMetadata Metadata { get; set; }
    }
}
