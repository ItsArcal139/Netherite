namespace Netherite.Nbt
{
    public class NbtIntArray : NbtTag
    {
        public NbtIntArray() : base(11) { }

        public NbtIntArray(int[] arr) : base(11)
        {
            Value = arr;
        }

        public int[] Value { get; set; }

        public static NbtIntArray Deserialize(byte[] buffer, ref int index, bool named = false)
        {
            NbtIntArray result = new NbtIntArray();
            InternalDeserializePhaseA(buffer, ref index, named, TagType.IntArray, result);
            int count = NbtIO.ReadInt(buffer, ref index);
            
            result.Value = new int[count];
            for(int i=0; i<count; i++)
            {
                result.Value[i] = NbtIO.ReadInt(buffer, ref index);
            }

            return result;
        }

        public override string ToString()
        {
            string name = Name == null ? "None" : $"'{Name}'";
            return $"TAG_Int_Array({name}): [{Value.Length} items]";
        }
    }
}
