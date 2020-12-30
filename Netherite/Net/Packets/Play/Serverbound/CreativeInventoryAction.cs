using Netherite.Entities;
using Netherite.Nbt.Serializations;
using Netherite.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Netherite.Net.Packets.Play.Serverbound
{
    public class CreativeInventoryAction : Packet
    {
        public byte Slot { get; set; }

        public ItemStack ItemStack { get; set; }

        public override Task HandleAsync(Server server, Player player)
        {
            if(ItemStack != null)
                Logger.Log(NbtConvert.SerializeToString(ItemStack.Data));
            return Task.CompletedTask;
        }
    }
}
