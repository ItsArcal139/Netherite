using System;
using System.Collections;
using System.Collections.Generic;

namespace Netherite.Nbt
{
    public class NbtList : NbtTag, IList<NbtTag>
    {
        public NbtList() : base(9) { }

        public NbtList(TagType type) : base(9)
        {
            ContentType = type;
        }

        public NbtList(TagType type, List<NbtTag> list) : base(9)
        {
            ContentType = type;
            children = list;
        }

        public TagType ContentType { get; set; }

        public int Count => children.Count;

        public bool IsReadOnly => false;

        public NbtTag this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int IndexOf(NbtTag item) => children.IndexOf(item);

        public void Insert(int index, NbtTag item) => children.Insert(index, item);

        public void RemoveAt(int index) => children.RemoveAt(index);

        public void Add(NbtTag item) => children.Add(item);

        public void Clear() => children.Clear();

        public bool Contains(NbtTag item) => children.Contains(item);

        public void CopyTo(NbtTag[] array, int arrayIndex) => children.CopyTo(array, arrayIndex);

        public bool Remove(NbtTag item) => children.Remove(item);

        public IEnumerator<NbtTag> GetEnumerator() => children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => children.GetEnumerator();

        private List<NbtTag> children = new List<NbtTag>();

        public static NbtList Deserialize(byte[] buffer, ref int index, bool named = false)
        {
            NbtList result = new NbtList();
            InternalDeserializePhaseA(buffer, ref index, named, TagType.List, result);
            TagType type = (TagType)NbtIO.ReadByte(buffer, ref index);
            int count = NbtIO.ReadInt(buffer, ref index);
            for (int i = 0; i < count; i++)
            {
                result.Add(NbtTag.Deserialize(buffer, ref index, false, type));
            }
            return result;
        }

        public override string ToString()
        {
            string name = Name == null ? "None" : $"'{Name}'";
            string result = $"TAG_List({name}): {Count} entries\n{{";

            foreach (NbtTag tag in children)
            {
                string[] lines = tag.ToString().Split('\n');
                foreach (string line in lines)
                {
                    result += "\n  " + line.ToString();
                }
            }

            return result + "\n}";
        }
    }
}
