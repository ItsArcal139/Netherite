using Netherite.Texts;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class DisplayTitle : Packet
    {
        public enum PacketAction
        {
            SetTitle, SetSubtitle, SetTimesAndDisplay, Hide, Reset
        }

        public PacketAction Action { get; set; }

        public Text Title { get; set; }

        public Text Subtitle { get; set; }

        public int FadeIn { get; set; }

        public int Stay { get; set; }
        
        public int FadeOut { get; set; }
    }
}
