namespace Netherite.Net.Packets.Status
{
    public class PingPong : Packet
    {
        public long Payload { get; set; }

        public PingPong() : base() { }

        public PingPong(long payload) : base()
        {
            Payload = payload;
        }
    }
}
