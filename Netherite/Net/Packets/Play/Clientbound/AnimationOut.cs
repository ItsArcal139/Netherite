using Netherite.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class AnimationOut : Packet
    {
        public Entity Entity { get; set; }

        public byte Animation { get; set; }
    }
}
