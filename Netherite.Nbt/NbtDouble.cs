namespace Netherite.Nbt
{
    public class NbtDouble : NbtTag
    {
        public NbtDouble() : base(6) { }

        public NbtDouble(double d) : base(6)
        {
            Value = d;
        }

        public double Value { get; set; }

        public static NbtDouble Deserialize(byte[] buffer, ref int index, bool named = false)
        {
            NbtDouble result = new NbtDouble();
            InternalDeserializePhaseA(buffer, ref index, named, TagType.Double, result);
            result.Value = NbtIO.ReadDouble(buffer, ref index);
            return result;
        }

        public override string ToString()
        {
            string name = Name == null ? "None" : $"'{Name}'";
            return $"TAG_Double({name}): {Value}";
        }
    }
}
