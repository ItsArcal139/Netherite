using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Nbt.Serializations
{
    internal class BufferWriter
    {
        private List<byte> container = new List<byte>();

        public byte[] ToBuffer()
        {
            byte[] result = container.ToArray();
            container.Clear();
            return result;
        }

        public void WriteFloat(float f)
        {
            WriteInt(BitConverter.SingleToInt32Bits(f));
        }

        public void WriteDouble(double d)
        {
            WriteLong(BitConverter.DoubleToInt64Bits(d));
        }

        public void WriteShort(short value)
        {
            container.Add((byte)(value >> (8 * 1) & 0xff));
            container.Add((byte)(value & 0xff));
        }

        public void WriteUShort(ushort value)
        {
            container.Add((byte)(value >> (8 * 1) & 0xff));
            container.Add((byte)(value & 0xff));
        }

        public void WriteInt(int value)
        {
            container.Add((byte)(value >> (8 * 3) & 0xff));
            container.Add((byte)(value >> (8 * 2) & 0xff));
            container.Add((byte)(value >> (8 * 1) & 0xff));
            container.Add((byte)(value & 0xff));
        }

        public void WriteByteArray(byte[] arr)
        {
            WriteInt(arr.Length);
            container.AddRange(arr);
        }

        internal void WriteBytesDirectly(byte[] arr)
        {
            container.AddRange(arr);
        }

        public void WriteString(string str)
        {
            WriteUShort((ushort)str.Length);
            container.AddRange(Encoding.UTF8.GetBytes(str));
        }

        public void WriteLong(long l)
        {
            container.Add((byte)(l >> (8 * 7) & 0xff));
            container.Add((byte)(l >> (8 * 6) & 0xff));
            container.Add((byte)(l >> (8 * 5) & 0xff));
            container.Add((byte)(l >> (8 * 4) & 0xff));
            container.Add((byte)(l >> (8 * 3) & 0xff));
            container.Add((byte)(l >> (8 * 2) & 0xff));
            container.Add((byte)(l >> (8 * 1) & 0xff));
            container.Add((byte)(l & 0xff));
        }

        public void WriteByte(byte b)
        {
            container.Add(b);
        }
    }
}
