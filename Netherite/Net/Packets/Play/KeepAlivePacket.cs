namespace Netherite.Net.Packets.Play
{
    public class KeepAlivePacket : Packet
    {
        public long Payload { get; set; }

        public KeepAlivePacket(long payload) : base()
        {
            Payload = payload;
        }
    }
}
