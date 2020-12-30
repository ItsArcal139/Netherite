using Netherite.Blocks;
using Netherite.Entities;
using Netherite.Net.Packets.Play.Clientbound;
using Netherite.Physics;
using Netherite.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Netherite.Net.Packets.Play.Serverbound
{
    public class BlockPlace : Packet
    {
        public Vector3 Position { get; set; }

        public byte Face { get; set; }

        public ItemStack HeldItem { get; set; }

        public Vector3 CursorPosition { get; set; }

        public override async Task HandleAsync(Server server, Player player)
        {
            // We don't know how to handle this yet
            if (Face == 255)
            {
                Logger.Log($"BlockPlace has face 255, ignore");
                return;
            }

            var offset = new Vector3();
            switch(Face)
            {
                case 0:
                    offset.Y = -1;
                    break;
                case 1:
                    offset.Y = 1;
                    break;
                case 2:
                    offset.Z = -1;
                    break;
                case 3:
                    offset.Z = 1;
                    break;
                case 4:
                    offset.X = -1;
                    break;
                case 5:
                    offset.X = 1;
                    break;
            }

            var blockPos = Position + offset;

            Logger.Log($"Place a block {HeldItem?.Material} at pos {blockPos}");
            player.World.SetBlock((int)blockPos.X, (int)blockPos.Y, (int)blockPos.Z, new Block(HeldItem.Material));

            foreach(var p in server.OnlinePlayers)
            {
                await p.Client.SendPacketAsync(new BlockChange
                {
                    Position = blockPos,
                    // BlockState = HeldItem.Material
                });
            }
        }
    }
}
