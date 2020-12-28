namespace Netherite.Nbt
{
    public class NbtByteArray : NbtTag
    {
        public NbtByteArray() : base(7) { }

        public NbtByteArray(byte[] arr) : base(7)
        {
            Value = arr;
        }

        public byte[] Value { get; set; }

        public static NbtByteArray Deserialize(byte[] buffer, ref int index, bool named = false)
        {
            NbtByteArray result = new NbtByteArray();
            InternalDeserializePhaseA(buffer, ref index, named, TagType.ByteArray, result);
            result.Value = NbtIO.ReadByteArray(buffer, ref index);
            return result;
        }

        public override string ToString()
        {
            string name = Name == null ? "None" : $"'{Name}'";
            return $"TAG_Byte_Array({name}): [{Value.Length} bytes]";
        }
    }
}
