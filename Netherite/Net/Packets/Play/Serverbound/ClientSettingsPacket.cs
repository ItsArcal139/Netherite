using System;

namespace Netherite.Net.Packets.Play.Serverbound
{
    public class ClientSettingsPacket : Packet
    {

        public string Locale { get; set; }

        public byte ViewDistance { get; set; }

        public byte ChatMode { get; set; }

        public bool ChatColors { get; set; }

        public SkinPart SkinParts { get; set; }
    }

    [Flags]
    public enum SkinPart : byte
    {
        Cape = 0x01,
        Jacket = 0x02,
        LeftSleeve = 0x04,
        RightSleeve = 0x08,
        LeftPants = 0x10,
        RightPants = 0x20,
        Hat = 0x40
    }
}
