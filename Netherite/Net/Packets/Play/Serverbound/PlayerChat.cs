using Netherite.Blocks;
using Netherite.Data.Entities;
using Netherite.Entities;
using Netherite.Net.Packets.Play.Clientbound;
using Netherite.Physics;
using Netherite.Texts;
using Netherite.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Netherite.Net.Packets.Play.Serverbound
{
    public class PlayerChat : Packet
    {
        public string Message { get; set; }

        public override async Task HandleAsync(Server server, Player player)
        {
            foreach(var p in server.OnlinePlayers)
            {
                p.SendMessage($"<{player.Name}> {Message}");
            }
            Logger.Info(
                LiteralText.Of($"<{player.Name}> {Message}")
            );

            await Task.CompletedTask;
        }
    }
}
