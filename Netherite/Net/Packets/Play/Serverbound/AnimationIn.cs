using Netherite.Entities;
using Netherite.Net.Packets.Play.Clientbound;
using Netherite.Worlds;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Netherite.Net.Packets.Play
{
    public class AnimationIn : Packet
    {
        public override bool IsConstantPacket => true;

        public override async Task HandleAsync(Server server, Player player)
        {
            foreach(var p in server.OnlinePlayers)
            {
                if(player != p)
                {
                    await p.Client.SendPacketAsync(new AnimationOut
                    {
                        Entity = player,
                        Animation = 0
                    });
                }
            }
        }
    }
}
