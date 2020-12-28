namespace Netherite.Nbt
{
    public class NbtLongArray : NbtTag
    {
        public NbtLongArray() : base(12) { }

        public NbtLongArray(long[] n) : base(12)
        {
            Value = n;
        }

        public long[] Value { get; set; }

        public static NbtLongArray Deserialize(byte[] buffer, ref int index, bool named = false)
        {
            NbtLongArray result = new NbtLongArray();
            InternalDeserializePhaseA(buffer, ref index, named, TagType.LongArray, result);
            int count = NbtIO.ReadInt(buffer, ref index);

            result.Value = new long[count];
            for (int i = 0; i < count; i++)
            {
                result.Value[i] = NbtIO.ReadLong(buffer, ref index);
            }

            return result;
        }

        public override string ToString()
        {
            string name = Name == null ? "None" : $"'{Name}'";
            return $"TAG_Long_Array({name}): [{Value.Length} items]";
        }
    }
}
