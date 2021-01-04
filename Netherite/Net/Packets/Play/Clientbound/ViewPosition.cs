using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class ViewPosition : Packet
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }
    }
}
