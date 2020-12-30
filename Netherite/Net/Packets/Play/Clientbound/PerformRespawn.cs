using Netherite.Data.Entities;
using Netherite.Worlds;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class PerformRespawn : Packet
    {
        public int Dimension { get; set; }

        public byte Difficulty { get; set; }

        public GameMode Mode { get; set; }

        public Identifier WorldName { get; set; }
    }
}
