namespace Netherite.Nbt
{
    public class NbtShort : NbtTag
    {
        public NbtShort() : base(2) { }

        public NbtShort(short n) : base(2)
        {
            Value = n;
        }

        public short Value { get; set; }

        public static NbtShort Deserialize(byte[] buffer, ref int index, bool named = false)
        {
            NbtShort result = new NbtShort();
            InternalDeserializePhaseA(buffer, ref index, named, TagType.Short, result);
            result.Value = NbtIO.ReadShort(buffer, ref index);
            return result;
        }

        public override string ToString()
        {
            string name = Name == null ? "None" : $"'{Name}'";
            return $"TAG_Short({name}): {Value}";
        }
    }
}
