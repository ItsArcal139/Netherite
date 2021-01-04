using Netherite.Data.Entities;
using Netherite.Data.Nbt;
using Netherite.Nbt;
using Netherite.Utils;
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

            object mat;
            if (!Enum.TryParse(typeof(Material), result, out mat))
            {
                Material = Material.Stone;
            } else {
                Material = (Material)mat;
            }

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

        internal ICollection<string> GetKeyOrder()
        {
            switch(Material)
            {
                case Material.Ladder:
                    return new List<string>
                    {
                        "facing", "waterlogged"
                    };
                case Material.NoteBlock:
                    return new List<string>
                    {
                        "instrument", "note", "powered"
                    };
                case Material.Lever:
                case Material.StoneButton:
                    return new List<string>
                    {
                        "face", "facing", "powered"
                    };
                case Material.RedstoneWire:
                    return new List<string>
                    {
                        "east", "north", "power", "south", "west"
                    };
            }
            return Properties.Keys;
        }

        public override string ToString()
        {
            var name = "minecraft:" + Material.ToString().ToSnakeCase();
            if (Properties == null || Properties.Count == 0) return name;
            name += "[";

            var props = "";
            foreach(var key in GetKeyOrder())
            {
                props += "," + key + "=" + Properties[key];
            }

            name += props[1..] + "]";
            return name;
        }
    }
}
