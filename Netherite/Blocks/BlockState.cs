using Netherite.Data.Entities;
using Netherite.Data.Nbt;
using Netherite.Nbt;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static BlockState FromNbt(NbtBlockState state)
        {
            string key = state.Name.Split(':')[0];
            string name = state.Name.Split(':')[1];
            var id = new Identifier(name, key);

            var p = new Dictionary<string, string>();
            foreach(var prop in state.Properties)
            {
                if(!(prop.Value is NbtString ns))
                {
                    throw new Exception("Not a NbtString");
                }
                p.Add(prop.Key, ns.Value);
            }

            return new BlockState
            {
                Id = id,
                Properties = p
            };
        }
    }
}
