namespace Netherite.Net.Packets.Play.Clientbound
{
    public class HeldItemChangePacket : Packet
    {
        public byte Slot { get; set; }
    }
}
