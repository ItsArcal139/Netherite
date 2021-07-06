using System;
using Netherite.Net.IO;

namespace Netherite.Utils
{
    /// <summary>
    /// An utility class that stores buffer data until it can be handled as a packet and take it out.
    /// </summary>
    public class BufferStorage
    {
        private byte[] buffer = new byte[0];

        public void Store(byte[] buffer)
        {
            byte[] temp = new byte[buffer.Length + this.buffer.Length];
            Array.Copy(this.buffer, temp, this.buffer.Length);
            Array.Copy(buffer, 0, temp, this.buffer.Length, buffer.Length);
            this.buffer = temp;
        }

        public bool CanReadAsPacket()
        {
            BufferReader reader = new BufferReader(buffer);
            if (!reader.CanReadVarInt()) return false;

            int length = reader.ReadVarInt(out int offset);
            length += offset;
            return length <= buffer.Length;
        }

        public byte[] TakeOutAsPacket()
        {
            BufferReader reader = new BufferReader(buffer);
            int length = reader.ReadVarInt(out int offset);
            length += offset;

            byte[] packet = new byte[length];
            Array.Copy(buffer, packet, length);

            byte[] temp = new byte[buffer.Length - length];
            Array.Copy(buffer, length, temp, 0, temp.Length);
            buffer = temp;
            return packet;
        }
    }
}
