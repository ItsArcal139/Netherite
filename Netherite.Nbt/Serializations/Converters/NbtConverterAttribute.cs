using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Nbt.Serializations.Converters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NbtConverterAttribute : Attribute
    {
        public Type ConverterType { get; set; }

        public NbtConverterAttribute(Type converterType)
        {
            if(!typeof(NbtConverter).IsAssignableFrom(converterType))
                throw new ArgumentException("Type " + converterType + " is not of type " + nameof(NbtConverter));
            ConverterType = converterType;
        }
    }

    public abstract class NbtConverter
    {
        public abstract NbtTag ToNbt(object obj);
        public abstract object FromNbt(NbtTag tag);
    }
}
