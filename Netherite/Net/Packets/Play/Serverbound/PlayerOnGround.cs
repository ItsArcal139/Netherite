﻿namespace Netherite.Net.Packets.Play.Serverbound
{
    public class PlayerOnGround : Packet
    {
        public bool OnGround { get; set; }

        public override bool IsConstantPacket => true;
    }
}
