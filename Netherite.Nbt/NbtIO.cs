using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Netherite.Nbt
{
    internal static class NbtIO
    {
        internal static byte ReadByte(byte[] buffer, ref int index)
        {
            return buffer[index++];
        }

        internal static void WriteByte(byte b, List<byte> output)
        {
            output.Add(b);
        }

        internal static short ReadShort(byte[] buffer, ref int index)
        {
            short result = 0;
            for (int i = 0; i < 2; i++)
            {
                result = (short)(result << 8 | buffer[index++]);
            }
            return result;
        }

        internal static void WriteShort(short s, List<byte> output)
        {
            for(int i=0; i<2; i++)
            {
                byte b = (byte) (s & 0xff);
                output.Add(b);
                s = (short) (s >> 8);
            }
        }

        internal static int ReadInt(byte[] buffer, ref int index)
        {
            int result = 0;
            for (int i = 0; i < 4; i++)
            {
                result = result << 8 | buffer[index++];
            }
            return result;
        }

        internal static void WriteInt(int n, List<byte> output)
        {
            for (int i = 0; i < 4; i++)
            {
                byte b = (byte)(n & 0xff);
                output.Add(b);
                n = n >> 8;
            }
        }

        internal static ushort ReadUShort(byte[] buffer, ref int index)
        {
            byte a = ReadByte(buffer, ref index);
            byte b = ReadByte(buffer, ref index);
            return (ushort)(a << 8 | b);
        }

        internal static uint ReadUInt(byte[] buffer, ref int index)
        {
            uint result = 0;
            for (int i = 0; i < 4; i++)
            {
                result = result << 8 | buffer[index++];
            }
            return result;
        }

        internal static long ReadLong(byte[] buffer, ref int index)
        {
            long result = 0;
            for (int i = 0; i < 8; i++)
            {
                result = result << 8 | buffer[index++];
            }
            return result;
        }

        internal static void WriteLong(long n, List<byte> output)
        {
            for (int i = 0; i < 8; i++)
            {
                byte b = (byte)(n & 0xff);
                output.Add(b);
                n = n >> 8;
            }
        }

        internal static float ReadFloat(byte[] buffer, ref int index)
        {
            return BitConverter.Int32BitsToSingle(ReadInt(buffer, ref index));
        }

        internal static double ReadDouble(byte[] buffer, ref int index)
        {
            return BitConverter.Int64BitsToDouble(ReadLong(buffer, ref index));
        }

        internal static byte[] ReadByteArray(byte[] buffer, ref int index)
        {
            int length = ReadInt(buffer, ref index);
            byte[] result = new byte[length];
            Array.Copy(buffer, index, result, 0, length);
            index += (int)length;
            return result;
        }

        internal static string ReadString(byte[] buffer, ref int index)
        {
            ushort length = ReadUShort(buffer, ref index);
            byte[] result = new byte[length];
            Array.Copy(buffer, index, result, 0, length);
            index += length;
            return Encoding.UTF8.GetString(result);
        }

        internal static byte Peek(byte[] buffer, ref int index)
        {
            return buffer[index];
        }

        internal static NbtTag.TagType PeekType(byte[] buffer, ref int index)
        {
            return (NbtTag.TagType)Peek(buffer, ref index);
        }
    }
}
