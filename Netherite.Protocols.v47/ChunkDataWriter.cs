using Netherite.Blocks;
using Netherite.Net.IO;
using Netherite.Utils;
using Netherite.Worlds;
using System;
using System.Text;

namespace Netherite.Protocols.v47
{
    public class ChunkDataWriter
    {
        public Chunk Chunk { get; set; }

        public ChunkDataWriter(Chunk chunk)
        {
            Chunk = chunk;
        }

        public void WriteTo(BufferWriter writer, bool emitHeaders = true)
        {
            if (emitHeaders)
            {
                writer.WriteInt(Chunk.X);
                writer.WriteInt(Chunk.Z);
                writer.WriteBool(true);
            }

            BufferWriter secBuf = new BufferWriter();

            ushort mask = 0;
            int m = 0;
            for (int i = 0; i < 16; i++)
            {
                if (!Chunk.IsSectionEmpty(i))
                {
                    mask |= (ushort)(1 << i);
                    var section = Chunk.Sections[i];

                    for (int j = 0; j < 4096; j++)
                    {
                        Block b = section.Blocks[j];

                        LegacyMaterial legacy = LegacyMaterial.FromMaterial(b.State.Material);
                        short id = legacy.Id;
                        byte data = legacy.Data;

                        short a = (short)(id << 4 | data & 0xf);
                        byte f = (byte)((a >> 8) & 0xff);
                        byte g = (byte)(a & 0xff);
                        secBuf.WriteByte(g);
                        secBuf.WriteByte(f); // The fk is this endian encoding
                    }
                    m++;
                }
            }

            for (int i = 0; i < 16; i++)
            {
                short flag = (short)(1 << i);
                if ((mask & flag) == flag)
                {
                    var section = Chunk.Sections[i];
                    foreach (byte b in section.BlockLight.Data)
                    {
                        secBuf.WriteByte(b);
                    }
                }
            }

            for (int i = 0; i < 16; i++)
            {
                short flag = (short)(1 << i);
                if ((mask & flag) == flag)
                {
                    var section = Chunk.Sections[i];
                    foreach (byte b in section.SkyLight.Data)
                    {
                        secBuf.WriteByte(b);
                    }
                }
            }

            for (int i = 0; i < 256; i++)
            {
                secBuf.WriteByte(127);
            }

            var secData = secBuf.ToBuffer();

            if (emitHeaders)
            {
                writer.WriteShort(mask);
                writer.WriteVarInt(secData.Length);
            }

            foreach (byte b in secData)
            {
                writer.WriteByte(b);
            }
        }
    }
}
