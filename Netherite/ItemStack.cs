using Netherite.Nbt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite
{
    public class ItemStack
    {
        public Material Material { get; set; }

        public byte Count { get; set; }

        public NbtCompound Data { get; set; }
    }
}
