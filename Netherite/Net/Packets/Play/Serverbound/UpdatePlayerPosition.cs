using Netherite.Entities;
using Netherite.Net.Packets.Play.Clientbound;
using Netherite.Physics;
using System;
using System.Threading.Tasks;

namespace Netherite.Net.Packets.Play.Serverbound
{
    public class UpdatePlayerPosition : Packet
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public bool OnGround { get; set; }

        public override bool IsConstantPacket => true;

        public override async Task HandleAsync(Server server, Player player)
        {
            Vector3 newPos = new Vector3(X, Y, Z);
            Vector3 delta = newPos - player.Position;
            player.Position = newPos;

            Packet p = new EntityRelativeMove
            {
                Entity = player,
                Delta = delta,
                OnGround = OnGround
            };

            if(Math.Abs(delta.X) >= 4 || Math.Abs(delta.Y) >= 4 || Math.Abs(delta.Z) >= 4)
            {
                p = new EntityTeleport
                {
                    Entity = player,
                    Position = newPos,
                    Yaw = player.Yaw,
                    Pitch = player.Pitch,
                    OnGround = OnGround
                };
            }

            foreach (var pl in server.OnlinePlayers)
            {
                if(pl != player)
                {
                    await pl.Client.SendPacketAsync(p);
                }
            }

            await Task.CompletedTask;
        }
    }
}
