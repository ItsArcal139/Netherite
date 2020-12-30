using Netherite.Data.Entities;
using Netherite.Nbt;
using System.Collections.Generic;

namespace Netherite.Blocks
{
    public struct BlockState
    {
        public Identifier Id { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public BlockState(Identifier id)
        {
            Id = id;
            Properties = new Dictionary<string, string>();
        }

        public BlockState(Identifier id, NbtCompound props)
        {
            Id = id;
            Properties = new Dictionary<string, string>();

            foreach (var p in props)
            {
                Properties.Add(p.Key, p.Value.ToValue());
            }
        }
    }
}
