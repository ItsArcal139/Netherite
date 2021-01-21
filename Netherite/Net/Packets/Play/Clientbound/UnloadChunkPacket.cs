using Netherite.Worlds;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class UnloadChunkPacket : Packet
    {
        public Chunk Chunk { get; set; }
    }
}
