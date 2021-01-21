namespace Netherite.Net.Packets.Play.Clientbound
{
    public class TimeUpdate : Packet
    {
        public long WorldAge { get; set; }

        public long WorldTime { get; set; }

        public override bool IsConstantPacket => true;
    }
}
