using Netherite.Attributes;
using Netherite.Data.Entities;
using Netherite.Entities;
using Netherite.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class SculkVibrationSignal : Packet
    {
        public Vector3 Source { get; set; }
        public Identifier DestinationType { get; set; }
        public Entity TargetEntity { get; set; }
        public Vector3 TargetPosition { get; set; }
        public int ArrivalTicks { get; set; }
    }
}
