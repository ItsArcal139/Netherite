using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Netherite.Nbt
{
    public abstract class NbtTag : INbtTag
    {
        public byte RawType { get; private set; }

        public enum TagType : byte
        {
            End, Byte, Short, Int, Long, Float, Double,
            ByteArray, String, List, Compound, IntArray, LongArray
        }

        public string Name { get; set; } = null;

        protected NbtTag(byte b)
        {
            RawType = b;
        }

        public static NbtTag Deserialize(byte[] buffer, ref int index, bool named = false, TagType? type = null)
        {
            switch (type ?? (TagType)NbtIO.ReadByte(buffer, ref index))
            {
                case TagType.End:
                    return null;
                case TagType.Byte:
                    return NbtByte.Deserialize(buffer, ref index, named);
                case TagType.Short:
                    return NbtShort.Deserialize(buffer, ref index, named);
                case TagType.Int:
                    return NbtInt.Deserialize(buffer, ref index, named);
                case TagType.Long:
                    return NbtLong.Deserialize(buffer, ref index, named);
                case TagType.Float:
                    return NbtFloat.Deserialize(buffer, ref index, named);
                case TagType.Double:
                    return NbtDouble.Deserialize(buffer, ref index, named);
                case TagType.ByteArray:
                    return NbtByteArray.Deserialize(buffer, ref index, named);
                case TagType.String:
                    return NbtString.Deserialize(buffer, ref index, named);
                case TagType.List:
                    return NbtList.Deserialize(buffer, ref index, named);
                case TagType.Compound:
                    return NbtCompound.Deserialize(buffer, ref index, named);
                case TagType.IntArray:
                    return NbtIntArray.Deserialize(buffer, ref index, named);
                case TagType.LongArray:
                    return NbtLongArray.Deserialize(buffer, ref index, named);
            }
            throw new ArgumentException($"Unknown type {buffer[index]} at index {index}: {(char)buffer[index]}");
        }

        protected static void InternalDeserializePhaseA(byte[] buffer, ref int index, bool named, TagType target, NbtTag instance)
        {
            if (named)
            {
                instance.Name = NbtIO.ReadString(buffer, ref index);
            }
        }

        public new abstract string ToString();
    }
}
