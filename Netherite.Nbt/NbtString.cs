namespace Netherite.Nbt
{
    public class NbtString : NbtTag
    {
        public NbtString() : this("") { }

        public NbtString(string s) : base(8)
        {
            Value = s;
        }

        public string Value { get; set; }

        public static NbtString Deserialize(byte[] buffer, ref int index, bool named = false)
        {
            NbtString result = new NbtString();
            InternalDeserializePhaseA(buffer, ref index, named, TagType.String, result);
            result.Value = NbtIO.ReadString(buffer, ref index);
            return result;
        }

        public override string ToString()
        {
            string name = Name == null ? "None" : $"'{Name}'";
            return $"TAG_String({name}): '{Value}'";
        }
    }
}
