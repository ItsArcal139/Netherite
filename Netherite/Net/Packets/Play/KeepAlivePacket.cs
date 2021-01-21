namespace Netherite.Net.Packets.Play
{
    public class KeepAlivePacket : Packet
    {
        public long Payload { get; set; }

        public override bool IsConstantPacket => true;

        public KeepAlivePacket(long payload) : base()
        {
            Payload = payload;
        }
    }
}
