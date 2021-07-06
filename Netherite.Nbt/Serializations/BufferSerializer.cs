using System;
using System.Collections.Generic;

namespace Netherite.Nbt.Serializations
{
    internal class BufferSerializer : INbtSerializer
    {
        private BufferWriter writer = new();

        public byte[] ToBuffer() => writer.ToBuffer();

        private void WritePrefix(NbtTag tag)
        {
            writer.WriteByte(tag.RawType);
            if (tag.Name != null)
            {
                writer.WriteString(tag.Name);
            }
        }

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

        public void WriteByte(NbtByte tag)
        {
            WritePrefix(tag);
            writer.WriteByte(tag.Value);
        }

        public void WriteByteArray(NbtByteArray tag)
        {
            WritePrefix(tag);
            writer.WriteByteArray(tag.Value);
        }

        public void WriteCompound(NbtCompound tag)
        {
            WritePrefix(tag);
            foreach (KeyValuePair<string, NbtTag> child in tag)
            {
                Write(child.Value);
            }
            writer.WriteByte(0);
        }

        public void WriteDouble(NbtDouble tag)
        {
            WritePrefix(tag);
            writer.WriteDouble(tag.Value);
        }

        public void WriteFloat(NbtFloat tag)
        {
            WritePrefix(tag);
            writer.WriteFloat(tag.Value);
        }

        public void WriteInt(NbtInt tag)
        {
            WritePrefix(tag);
            writer.WriteInt(tag.Value);
        }

        public void WriteIntArray(NbtIntArray tag)
        {
            WritePrefix(tag);
            writer.WriteInt(tag.Value.Length);
            foreach (int i in tag.Value)
            {
                writer.WriteInt(i);
            }
        }

        public void WriteList(NbtList tag)
        {
            WritePrefix(tag);
            writer.WriteByte((byte)tag.ContentType);
            writer.WriteInt(tag.Count);

            var c = new BufferSerializer();
            foreach (NbtTag i in tag)
            {
                c.Write(i);
                byte[] buf = c.writer.ToBuffer();
                for (int j = 1; j < buf.Length; j++)
                {
                    writer.WriteByte(buf[j]);
                }
            }
        }

        public void WriteLong(NbtLong tag)
        {
            WritePrefix(tag);
            writer.WriteLong(tag.Value);
        }

        public void WriteLongArray(NbtLongArray tag)
        {
            WritePrefix(tag);
            writer.WriteInt(tag.Value.Length);
            foreach (long i in tag.Value)
            {
                writer.WriteLong(i);
            }
        }

        public void WriteShort(NbtShort tag)
        {
            WritePrefix(tag);
            writer.WriteShort(tag.Value);
        }

        public void WriteString(NbtString tag)
        {
            WritePrefix(tag);
            writer.WriteString(tag.Value);
        }
    }
}
