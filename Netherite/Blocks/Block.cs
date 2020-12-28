using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Blocks
{
    public class Block
    {
        public Material Material { get; set; }

        public Block(Material material)
        {
            Material = material;
        }
    }
}
