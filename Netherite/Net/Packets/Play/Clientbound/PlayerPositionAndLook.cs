using Netherite.Entities;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class PlayerPositionAndLook : Packet
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public float Yaw { get; set; }

        public float Pitch { get; set; }

        public byte RelationFlags { get; set; }
    }
}
