using Netherite.Entities;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class SpawnLivingEntity : Packet
    {
        public LivingEntity Entity { get; set; }

        public SpawnLivingEntity() : base() { }

        public SpawnLivingEntity(LivingEntity entity) : base()
        {
            Entity = entity;
        }
    }
}
