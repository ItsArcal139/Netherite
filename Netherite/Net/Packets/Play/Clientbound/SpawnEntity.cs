using Netherite.Entities;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class SpawnEntity : Packet
    {
        public Entity Entity { get; set; }

        public SpawnEntity() : base() { }

        public SpawnEntity(Entity entity) : base()
        {
            Entity = entity;
        }
    }
}
