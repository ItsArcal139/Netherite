using Netherite.Entities;
using Netherite.Nbt;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class EntityNBT : Packet
    {
        public Entity Entity { get; set; }
        public NbtCompound Tag { get; set; }
    }
}
