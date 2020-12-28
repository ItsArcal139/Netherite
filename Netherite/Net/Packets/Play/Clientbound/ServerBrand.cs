using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class ServerBrand : Packet
    {
        public string Name { get; set; }
    }
}
