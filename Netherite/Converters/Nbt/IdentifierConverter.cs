using Netherite.Data.Entities;
using Netherite.Nbt;
using Netherite.Nbt.Serializations.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Converters.Nbt
{
    public class IdentifierConverter : NbtConverter
    {
        public override object FromNbt(NbtTag tag)
        {
            if (!(tag is NbtString)) throw new InvalidCastException("Expected identifier to be a string, found " + (NbtTag.TagType)tag.RawType);
            string[] info = ((NbtString)tag).Value.Split(':', 2);
            return new Identifier(info[1], info[0]);
        }

        public override NbtTag ToNbt(object obj)
        {
            if (obj == null) return null;
            Identifier id = (Identifier)obj;
            return new NbtString(id.ToString());
        }
    }
}
