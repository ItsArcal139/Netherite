using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Netherite.Nbt
{
    public class NbtCompound : NbtTag, IDictionary<string, NbtTag>
    {
        public NbtTag this[string key]
        {
            get => children.Find(tag => tag.Name == key);
            set
            {
                // Should I respect C# standard, which throws error
                // when old item is not found?

                NbtTag old = this[key];
                if (old != null)
                {
                    children.Remove(old);
                }

                value.Name = key;
                children.Add(value);
            }
        }

        public ICollection<string> Keys => children.ConvertAll(tag => tag.Name);

        public ICollection<NbtTag> Values => children.ConvertAll(t => t);

        public int Count => children.Count;

        public bool IsReadOnly => false;

        public void Add(string key, NbtTag value)
        {
            this[key] = value;
            value.Name = key;
        }

        public void Add(KeyValuePair<string, NbtTag> item) => Add(item.Key, item.Value);

        public void Clear() => children.Clear();

        public bool Contains(KeyValuePair<string, NbtTag> item) => ContainsKey(item.Key);

        public bool ContainsKey(string key) => this[key] != null;

        public void CopyTo(KeyValuePair<string, NbtTag>[] array, int arrayIndex)
        {
            foreach (var pair in children.ConvertAll(t => new KeyValuePair<string, NbtTag>(t.Name, t)))
            {
                array[arrayIndex++] = pair;
            }
        }

        public IEnumerator<KeyValuePair<string, NbtTag>> GetEnumerator() => children.ConvertAll(t => new KeyValuePair<string, NbtTag>(t.Name, t)).GetEnumerator();

        public bool Remove(string key) => children.Remove(this[key]);

        public bool Remove(KeyValuePair<string, NbtTag> item) => Remove(item.Key);

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out NbtTag value)
        {
            value = this[key];
            return value != null;
            ;
        }

        IEnumerator IEnumerable.GetEnumerator() => children.GetEnumerator();

        private List<NbtTag> children = new List<NbtTag>();

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
