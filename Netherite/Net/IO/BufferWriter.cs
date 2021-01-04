using Netherite.Data.Entities;
using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Physics;
using Netherite.Texts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Net.IO
{
    public class BufferWriter
    {
        private List<byte> container = new List<byte>();

        public void Flush(int id = -1)
        {
            byte[] raw = container.ToArray();
            container.Clear();
            WriteVarInt(id);
            WriteBytesDirectly(raw);

            byte[] result = container.ToArray();
            container.Clear();

            WriteByteArray(result);
        }

        public byte[] ToBuffer()
        {
            byte[] result = container.ToArray();
            container.Clear();
            return result;
        }

        public void WriteFixedPointI(double d)
        {
            WriteInt((int)Math.Round(d * 32));
        }

        public void WriteFixedPointB(double d)
        {
            WriteByte((byte)Math.Round(d * 32));
        }

        public void WriteLongPos(Vector3 pos)
        {
            int x = (int)pos.X;
            int y = (int)pos.Y;
            int z = (int)pos.Z;

            long result = ((x & 0x3ffffff) << 38 | ((y & 0xfff) << 26) | (z & 0x3ffffff));
            WriteLong(result);
        }

        public void WriteVarInt(int value)
        {
            uint v = (uint)value;
            do
            {
                byte temp = (byte)(v & 0b01111111);
                v >>= 7;
                if (v != 0)
                {
                    temp |= 0b10000000;
                }
                container.Add(temp);
            } while (v != 0);
        }

        public void WriteVarLong(long value)
        {
            ulong v = (ulong)value;
            do
            {
                byte temp = (byte)(v & 0b01111111);
                v >>= 7;
                if (v != 0)
                {
                    temp |= 0b10000000;
                }
                container.Add(temp);
            } while (v != 0);
        }

        public void WriteBool(bool flag)
        {
            WriteByte((byte)(flag ? 1 : 0));
        }

        public void WriteByteArray(byte[] arr)
        {
            WriteVarInt(arr.Length);
            container.AddRange(arr);
        }

        internal void WriteBytesDirectly(byte[] arr)
        {
            container.AddRange(arr);
        }

        public void WriteString(string str)
        {
            WriteByteArray(Encoding.UTF8.GetBytes(str));
        }

        public void WriteLong(long l)
        {
            container.Add((byte)(l >> 8 * 7 & 0xff));
            container.Add((byte)(l >> 8 * 6 & 0xff));
            container.Add((byte)(l >> 8 * 5 & 0xff));
            container.Add((byte)(l >> 8 * 4 & 0xff));
            container.Add((byte)(l >> 8 * 3 & 0xff));
            container.Add((byte)(l >> 8 * 2 & 0xff));
            container.Add((byte)(l >> 8 * 1 & 0xff));
            container.Add((byte)(l & 0xff));
        }

        public void WriteNbt(NbtCompound tag)
        {
            WriteBytesDirectly(NbtConvert.SerializeToBuffer(tag));
        }

        public void WriteInt(int n)
        {
            container.Add((byte)(n >> 8 * 3 & 0xff));
            container.Add((byte)(n >> 8 * 2 & 0xff));
            container.Add((byte)(n >> 8 * 1 & 0xff));
            container.Add((byte)(n & 0xff));
        }

        public void WriteIdentifier(Identifier id)
        {
            WriteString(id.ToString());
        }

        public void WriteShort(int n)
        {
            container.Add((byte)(n >> 8 * 1 & 0xff));
            container.Add((byte)(n & 0xff));
        }

        public void WriteDouble(double d)
        {
            WriteLong(BitConverter.DoubleToInt64Bits(d));
        }

        public void WriteFloat(float f)
        {
            WriteInt(BitConverter.SingleToInt32Bits(f));
        }

        public void WriteGuid(Guid g)
        {
            // Converts .NET Guid to Java UUID.
            // The 8 most significant bits are reversed in groups.
            byte[] buf = g.ToByteArray();
            Array.Reverse(buf, 0, 4);
            Array.Reverse(buf, 4, 2);
            Array.Reverse(buf, 6, 2);
            container.AddRange(buf);
        }

        public void WriteByte(byte b)
        {
            container.Add(b);
        }

        public void WriteChat(Text text)
        {
            string json = JsonConvert.SerializeObject(text, new JsonSerializerSettings()
            {
                ContractResolver = TextShouldSerializeContractResolver.Instance
            });
            WriteString(json);
        }

        public void WriteAngle(double degrees)
        {
            double angle = (degrees % 360) / 360 * 256;
            byte steps = (byte)Math.Round(angle);
            WriteByte(steps);
        }

        public void WriteAngle(byte b)
        {
            WriteByte(b);
        }
    }
}
