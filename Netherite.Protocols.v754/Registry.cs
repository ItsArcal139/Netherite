using Netherite.Blocks;
using Netherite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Protocols.v754
{
    public partial class Registry
    {
        public Block GetBlock(short state) => new Block((Material)StateToMatch[state].Numeric);
    }
}