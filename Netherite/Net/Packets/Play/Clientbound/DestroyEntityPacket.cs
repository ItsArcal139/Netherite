using Netherite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class DestroyEntityPacket : Packet
    {
        public List<Entity> Entities { get; set; }
    }
}
