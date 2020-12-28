using Netherite.Nbt;
using Netherite.Nbt.Serializations.Converters;
using Netherite.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Converters.Nbt
{
    public class ColorConverter : NbtConverter
    {
        public override object FromNbt(NbtTag tag)
        {
            return new Color(((NbtInt)tag).Value);
        }

        public override NbtTag ToNbt(object obj)
        {
            return new NbtInt(((Color)obj).RGB);
        }
    }
}
