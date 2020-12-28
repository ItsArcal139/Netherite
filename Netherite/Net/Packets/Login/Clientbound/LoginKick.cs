using Netherite.Texts;

namespace Netherite.Net.Packets.Login.Clientbound
{
    public class LoginKick : Packet
    {
        private const int PacketID = 0x0;

        public Text Reason { get; set; }

        public LoginKick() : base() { }

        public LoginKick(Text reason) : base()
        {
            Reason = reason;
        }
    }
}
