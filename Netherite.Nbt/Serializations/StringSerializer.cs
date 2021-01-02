using System.Collections.Generic;

namespace Netherite.Nbt.Serializations
{
    internal class StringSerializer : INbtSerializer
    {
        private string result = "";

        public override string ToString() => result;

        public void Write(NbtTag tag)
        {
            if (tag is NbtByte a) WriteByte(a);
            if (tag is NbtShort b) WriteShort(b);
            if (tag is NbtInt c) WriteInt(c);
            if (tag is NbtLong d) WriteLong(d);
            if (tag is NbtFloat e) WriteFloat(e);
            if (tag is NbtDouble f) WriteDouble(f);
            if (tag is NbtByteArray g) WriteByteArray(g);
            if (tag is NbtString h) WriteString(h);
            if (tag is NbtList i) WriteList(i);
            if (tag is NbtCompound j) WriteCompound(j);
            if (tag is NbtIntArray k) WriteIntArray(k);
            if (tag is NbtLongArray l) WriteLongArray(l);
        }

        private void WritePrefix(NbtTag tag)
        {
            if (tag.Name != null)
            {
                result += ($@"{tag.Name}:");
            }
        }

        public void WriteByte(NbtByte tag)
        {
            WritePrefix(tag);
            result += ($"{tag.Value}b");
        }

        public void WriteByteArray(NbtByteArray tag)
        {
            WritePrefix(tag);
            result += ($"[");

            bool first = true;
            foreach (long n in tag.Value)
            {
                if (!first) result += (",");
                first = false;
                result += ($"{n}b");
            }

            result += ($"]");
        }

        public void WriteCompound(NbtCompound tag)
        {
            WritePrefix(tag);
            result += ("{");

            bool first = true;
            foreach (KeyValuePair<string, NbtTag> child in tag)
            {
                if (!first) result += (",");
                first = false;
                Write(child.Value);
            }

            result += ("}");
        }

        public void WriteDouble(NbtDouble tag)
        {
            WritePrefix(tag);
            result += ($"{tag.Value}d");
        }

        public void WriteFloat(NbtFloat tag)
        {
            WritePrefix(tag);
            result += ($"{tag.Value}f");
        }

        public void WriteInt(NbtInt tag)
        {
            WritePrefix(tag);
            result += ($"{tag.Value}");
        }

        public void WriteIntArray(NbtIntArray tag)
        {
            WritePrefix(tag);
            result += ($"[");

            bool first = true;
            foreach (int n in tag.Value)
            {
                if (!first) result += (",");
                first = false;
                result += ($"{n}");
            }

            result += ($"]");
        }

        public void WriteList(NbtList tag)
        {
            WritePrefix(tag);
            result += ($"[");

            bool first = true;
            foreach (NbtTag n in tag)
            {
                if (!first) result += (",");
                first = false;
                Write(n);
            }

            result += ($"]");
        }

        public void WriteLong(NbtLong tag)
        {
            WritePrefix(tag);
            result += ($"{tag.Value}L");
        }

        public void WriteLongArray(NbtLongArray tag)
        {
            WritePrefix(tag);
            result += ($"[");

            bool first = true;
            foreach (long n in tag.Value)
            {
                if (!first) result += (",");
                first = false;
                result += ($"{n}L");
            }

            result += ($"]");
        }

        public void WriteShort(NbtShort tag)
        {
            WritePrefix(tag);
            result += ($"{tag.Value}s");
        }

        public void WriteString(NbtString tag)
        {
            WritePrefix(tag);
            result += ($@"""{tag.Value.Replace("\\", "\\\\")}""");
        }
    }
}
