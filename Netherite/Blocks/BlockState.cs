using Netherite.Data.Entities;
using Netherite.Data.Nbt;
using Netherite.Nbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Netherite.Blocks
{
    public struct BlockState
    {
        public Material Material { get; set; }

        public Dictionary<string, string> Properties { get; set; }

        public BlockState(Identifier id, NbtCompound props = null)
        {
            var name = id.Key;
            var result = "";
            foreach (Match m in new Regex("([a-z0-9]*)").Matches(name))
            {
                if (m.Value.Length > 0)
                {
                    result += m.Value[0].ToString().ToUpper();
                    result += m.Value[1..];
                }
            }

            Material = (Material)Enum.Parse(typeof(Material), result);

            Properties = new Dictionary<string, string>();

            if (props != null)
            {
                foreach (var prop in props)
                {
                    if (!(prop.Value is NbtString ns))
                    {
                        throw new Exception("Not a NbtString");
                    }
                    Properties.Add(prop.Key, ns.Value);
                }
            }
        }

        public static BlockState FromNbt(NbtBlockState state)
        {
            string key = state.Name.Split(':')[0];
            string name = state.Name.Split(':')[1];
            var id = new Identifier(name, key);

            return new BlockState(id, state.Properties);
        }
    }
}
