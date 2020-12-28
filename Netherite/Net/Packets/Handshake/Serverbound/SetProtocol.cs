using System;

namespace Netherite.Net.Packets.Handshake.Serverbound
{
    public class SetProtocol : Packet
    {
        public int ProtocolVersion { get; set; }

        public string ServerAddress { get; set; }

        public ushort ServerPort { get; set; }

        public PacketState NextState { get; set; }
    }
}
