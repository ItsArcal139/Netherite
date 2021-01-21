using Netherite.Entities;
using Netherite.Physics;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class EntityRelativeMove : Packet
    {
        public Entity Entity { get; set; }
        public Vector3 Delta { get; set; }
        public bool OnGround { get; set; }

        public override bool IsConstantPacket => true;
    }
}
