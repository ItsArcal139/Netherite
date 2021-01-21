using Netherite.Worlds;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class ChunkDataPacket : Packet
    {
        public Chunk Chunk { get; set; }

        public override bool IsConstantPacket => true;
    }
}
