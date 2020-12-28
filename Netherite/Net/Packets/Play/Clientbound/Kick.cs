using Netherite.Texts;

namespace Netherite.Net.Packets
{
    public class Kick : Packet
    {
        public Text Reason { get; set; }

        public Kick(Text reason) : base()
        {
            Reason = reason;
        }
    }
}
