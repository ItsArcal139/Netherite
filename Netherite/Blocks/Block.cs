using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Blocks
{
    public class Block
    {
        public BlockState State { get; set; }

        public Block(BlockState state)
        {
            State = state;
        }
    }
}
