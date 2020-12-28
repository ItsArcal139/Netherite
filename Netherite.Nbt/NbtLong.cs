namespace Netherite.Nbt
{
    public class NbtLong : NbtTag
    {
        public NbtLong() : base(4) { }

        public NbtLong(long n) : base(4)
        {
            Value = n;
        }

        public long Value { get; set; }

        public static NbtLong Deserialize(byte[] buffer, ref int index, bool named = false)
        {
            NbtLong result = new NbtLong();
            InternalDeserializePhaseA(buffer, ref index, named, TagType.Long, result);
            result.Value = NbtIO.ReadLong(buffer, ref index);
            return result;
        }

        public override string ToString()
        {
            string name = Name == null ? "None" : $"'{Name}'";
            return $"TAG_Long({name}): {Value}L";
        }
    }
}
