﻿using Netherite.Blocks;
using Netherite.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class BlockChange : Packet
    {
        public Vector3 Position { get; set; }

        public BlockState State { get; set; }
    }
}
