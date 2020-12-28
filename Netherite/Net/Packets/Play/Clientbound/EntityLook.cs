using Netherite.Entities;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class EntityLook : Packet
    {
        public Entity Entity { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }
    }
}
