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

                Random r = new Random();
                int eid = r.Next();

                await p.Client.SendPacketAsync(new SpawnPlayer
                {
                    Pitch = player.Pitch,
                    Yaw = player.Yaw,
                    CurrentItem = 5,
                    EntityID = eid,
                    Guid = player.Guid,
                    X = player.Position.X,
                    Y = player.Position.Y,
                    Z = player.Position.Z,
                    Metadata = player.Metadata
                });

                await p.Client.SendPacketAsync(new EntityHeadLook
                {
                    Entity = new DummyEntity(eid),
                    Yaw = player.Yaw
                });
            }
        }
    }
}
