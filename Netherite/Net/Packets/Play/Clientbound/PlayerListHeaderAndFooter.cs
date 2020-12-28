using Netherite.Texts;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class PlayerListHeaderAndFooter : Packet
    {
        public Text Header { get; set; }

        public Text Footer { get; set; }

        public PlayerListHeaderAndFooter(Text header, Text footer) : base()
        {
            Header = header;
            Footer = footer;
        }
    }
}
