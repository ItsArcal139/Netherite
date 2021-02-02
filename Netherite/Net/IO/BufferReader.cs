using Netherite.Nbt;
using Netherite.Physics;
using System;
using System.Text;

namespace Netherite.Net.IO
{
    public class BufferReader
    {
        private int index = 0;
        private byte[] buffer;

        public BufferReader(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public byte[] Buffer => buffer;

        public long ReadLong(out int length)
        {
            long result = 0;
            for (int i = 0; i < 8; i++)
            {
                result = result << 8 | buffer[index++];
            }
            length = 8;
            return result;
        }

        public int ReadInt(out int length)
        {
            int result = 0;
            for (int i = 0; i < 4; i++)
            {
                result = result << 8 | buffer[index++];
            }
            length = 4;
            return result;
        }

        public int ReadInt() => ReadInt(out int _);

        public long ReadLong() => ReadLong(out int _);

        public byte ReadByte(out int length)
        {
            length = 1;
            return buffer[index++];
        }

        public byte ReadByte() => ReadByte(out int _);

        public bool ReadBool(out int length) => ReadByte(out length) == 1;

        public bool ReadBool() => ReadByte() == 1;

        public double ReadDouble(out int length) => BitConverter.Int64BitsToDouble(ReadLong(out length));

        public double ReadDouble() => ReadDouble(out int _);

        public float ReadFloat(out int length) => BitConverter.Int32BitsToSingle(ReadInt(out length));

        public float ReadFloat() => ReadFloat(out int _);

        public byte[] ReadRemaining()
        {
            int len = buffer.Length - index;
            byte[] result = new byte[len];
            Array.Copy(buffer, index, result, 0, len);
            return result;
        }

        public int ReadVarInt(out int length, bool peek = false)
        {
            int o = index;

            int numRead = 0;
            int result = 0;
            byte read;

            do
            {
                read = buffer[index++];
                int value = read & 0b01111111;
                result |= value << 7 * numRead;

                numRead++;
                if (numRead > 5)
                {
                    throw new ArgumentException("VarInt too big");
                }
            } while ((read & 0b10000000) != 0);

            length = index - o;
            if (peek) index = o;
            return result;
        }

        public long ReadVarLong(out int length, bool peek = false)
        {
            int o = index;

            int numRead = 0;
            long result = 0;
            byte read;

            do
            {
                read = buffer[index++];
                long value = read & 0b01111111;
                result |= value << 7 * numRead;

                numRead++;
                if (numRead > 10)
                {
                    throw new ArgumentException("VarLong too big");
                }
            } while ((read & 0b10000000) != 0);

            length = index - o;
            if (peek) index = o;
            return result;
        }

        public bool CanReadVarInt()
        {
            for(int i=index; i<buffer.Length; i++)
            {
                if ((buffer[i] & 0b10000000) == 0) return true;
            }
            return false;
        }

        public int ReadVarInt(bool peek = false)
        {
            return ReadVarInt(out int _, peek);
        }

        public byte[] ReadByteArray(out int length)
        {
            int o = index;
            int len = ReadVarInt();
            byte[] result = new byte[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = buffer[index++];
            }

            length = index - o;
            return result;
        }

        public byte[] ReadByteArray()
        {
            return ReadByteArray(out int _);
        }

        public string ReadString(out int length)
        {
            byte[] r = ReadByteArray(out length);
            return Encoding.UTF8.GetString(r);
        }

        public string ReadString()
        {
            return ReadString(out int _);
        }

        public ushort ReadUShort(out int length)
        {
            int o = index;
            int result = 0;
            result = result | buffer[index++];
            result = result << 8 | buffer[index++];
            length = index - o;
            return (ushort)result;
        }

        public ushort ReadUShort()
        {
            return ReadUShort(out int _);
        }

        public short ReadShort(out int length)
        {
            int o = index;
            int result = 0;
            result = result | buffer[index++];
            result = result << 8 | buffer[index++];
            length = index - o;
            return (short)result;
        }

        public short ReadShort() => ReadShort(out _);

        public Vector3 ReadLocation(out int length)
        {
            long val = ReadLong(out length);
            long x = val >> 38;
            long y = (val >> 26) & 0xFFF;
            long z = val << 38 >> 38;

            if (x >= 1 << 25)
            {
                x -= 1 << 26;
            }

            if (y >= 1 << 11)
            {
                y -= 1 << 12;
            }

            if (z >= 1 << 25)
            {
                z -= 1 << 26;
            }

            return new Vector3
            {
                X = x,
                Y = y,
                Z = z
            };
        }

        public Vector3 ReadLocation() => ReadLocation(out _);

        public NbtTag ReadNbt(out int length)
        {
            int o = index;
            var result = NbtTag.Deserialize(buffer, ref index, true);
            length = index - o;
            return result;
        }

        public NbtTag ReadNbt() => ReadNbt(out _);

        public Guid ReadGuid(out int length)
        {
            byte[] buf = new byte[16];
            Array.Copy(buffer, index, buf, 0, 16);
            index += 16;
            length = 16;

            Array.Reverse(buf, 0, 4);
            Array.Reverse(buf, 4, 2);
            Array.Reverse(buf, 6, 2);

            return new Guid(buf);
        }

        public Guid ReadGuid() => ReadGuid(out _);

        public void Skip(int step) => index += step;

        public double ReadAngle(out int length) => ReadByte(out length) / 256 * 360;

        public double ReadAngle() => ReadAngle(out _);
    }
}
