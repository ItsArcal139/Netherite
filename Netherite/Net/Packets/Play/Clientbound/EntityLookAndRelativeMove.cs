using Netherite.Entities;
using Netherite.Physics;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class EntityLookAndRelativeMove : Packet
    {
        public Entity Entity { get; set; }
        public Vector3 Delta { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }
    }
}
