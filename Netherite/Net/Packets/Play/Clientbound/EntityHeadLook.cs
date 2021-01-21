using Netherite.Entities;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class EntityHeadLook : Packet
    {
        public Entity Entity { get; set; }
        public float Yaw { get; set; }

        public override bool IsConstantPacket => true;
    }
}
