namespace Netherite.Nbt
{
    public class NbtByte : NbtTag
    {
        public NbtByte() : base(1) { }

        public NbtByte(byte value) : base(1)
        {
            Value = value;
        }

        public byte Value { get; set; }

        public static NbtByte Deserialize(byte[] buffer, ref int index, bool named = false)
        {
            NbtByte result = new NbtByte();
            InternalDeserializePhaseA(buffer, ref index, named, TagType.Byte, result);
            result.Value = NbtIO.ReadByte(buffer, ref index);
            return result;
        }

        public override string ToString()
        {
            string name = Name == null ? "None" : $"'{Name}'";
            return $"TAG_Byte({name}): {Value}";
        }
    }
}
