using Netherite.Entities;
using Netherite.Physics;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class EntityVelocityPacket : Packet
    {
        public Entity Entity { get; set; }
        public Vector3 Velocity { get; set; }

        public EntityVelocityPacket() : base() { }

        public EntityVelocityPacket(Entity entity, Vector3 velocity) : this()
        {
            Entity = entity;
            Velocity = velocity;
        }
    }
}
