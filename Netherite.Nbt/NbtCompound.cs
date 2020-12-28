using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Netherite.Nbt
{
    public class NbtCompound : NbtTag, IDictionary<string, NbtTag>
    {
        public NbtTag this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<string> Keys => dict.Keys;

        public ICollection<NbtTag> Values => dict.Values;

        public int Count => dict.Count;

        public bool IsReadOnly => false;

        public void Add(string key, NbtTag value)
        {
            dict.Add(key, value);
            value.Name = key;
        }

        public void Add(KeyValuePair<string, NbtTag> item) => dict.Add(item.Key, item.Value);

        public void Clear() => dict.Clear();

        public bool Contains(KeyValuePair<string, NbtTag> item) => throw new NotImplementedException();

        public bool ContainsKey(string key) => dict.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, NbtTag>[] array, int arrayIndex) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<string, NbtTag>> GetEnumerator() => dict.GetEnumerator();

        public bool Remove(string key) => dict.Remove(key);

        public bool Remove(KeyValuePair<string, NbtTag> item) => throw new NotImplementedException();

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out NbtTag value) => dict.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();

        private Dictionary<string, NbtTag> dict = new Dictionary<string, NbtTag>();

        public NbtCompound() : base(10) { }

        public static NbtCompound Deserialize(byte[] buffer, ref int index, bool named = false)
        {
            NbtCompound result = new NbtCompound();

            InternalDeserializePhaseA(buffer, ref index, named, TagType.Compound, result);

            while (buffer[index] != 0)
            {
                NbtTag tag = NbtTag.Deserialize(buffer, ref index, true);
                result.Add(tag.Name, tag);
            }
            index++;

            return result;
        }

        public override string ToString()
        {
            string name = Name == null ? "None" : $"'{Name}'";
            string result = $"TAG_Compound({name}): {Count} entries\n{{";

            foreach (NbtTag tag in Values)
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
