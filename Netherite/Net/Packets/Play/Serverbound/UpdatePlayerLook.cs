using Netherite.Entities;
using Netherite.Net.Packets.Play.Clientbound;
using System.Threading.Tasks;

namespace Netherite.Net.Packets.Play.Serverbound
{
    public class UpdatePlayerLook : Packet
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }

        public override async Task HandleAsync(Server server, Player player)
        {
            player.Yaw = Yaw;
            player.Pitch = Pitch;

            foreach (var pl in server.OnlinePlayers)
            {
                if (pl != player)
                {
                    await pl.Client.SendPacketAsync(new EntityLook
                    {
                        Entity = player,
                        Yaw = Yaw,
                        Pitch = Pitch,
                        OnGround = OnGround
                    });

                    await pl.Client.SendPacketAsync(new EntityHeadLook
                    {
                        Entity = player,
                        Yaw = Yaw
                    });
                }
            }

            await Task.CompletedTask;
        }
    }

    public class EntityAction : Packet
    {
        public Entity Entity { get; set; }
        public int ActionID { get; set; }
        public int ActionParam { get; set; }

        public override async Task HandleAsync(Server server, Player player)
        {
            bool updatesMeta = false;
            switch(ActionID)
            {
                case 0:
                case 1:
                    updatesMeta = true;
                    player.Metadata.IsCrouching = ActionID == 0;
                    break;
                case 2:
                    break;
                case 3:
                case 4:
                    updatesMeta = true;
                    player.Metadata.IsSprinting = ActionID == 3;
                    break;
                case 5:
                case 6:
                    break;
            }

            if(updatesMeta)
            {
                foreach(var p in server.OnlinePlayers)
                {
                    if(player != p)
                    {
                        await p.Client.SendPacketAsync(new EntityMetadataPacket
                        {
                            Entity = player,
                            Metadata = player.Metadata
                        });
                    }
                }
            }
        }
    }
}
