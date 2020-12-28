using Netherite.Blocks;
using Netherite.Net.IO;
using Netherite.Utils;
using Netherite.Worlds;
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
            NibbleArray blockLight = new NibbleArray(4096);
            NibbleArray skyLight = new NibbleArray(4096);

            ushort mask = 0;
            for (int i=0; i<16; i++)
            {
                if(!Chunk.IsSectionEmpty(i))
                {
                    mask |= (ushort)(1 << i);
                    var section = Chunk.Sections[i];

                    int lightIndex = 0;
                    foreach(Block b in section.Blocks)
                    {
                        var legacy = LegacyMaterial.FromMaterial(b.Material);
                        short id = legacy.Id;
                        byte data = legacy.Data;

                        short a = (short)(id << 4 | data & 0xf);
                        byte f = (byte)((a >> 8) & 0xff);
                        byte g = (byte)(a & 0xff);
                        secBuf.WriteByte(g);
                        secBuf.WriteByte(f); // The fk is this endian encoding

                        blockLight[lightIndex] = 15;
                        skyLight[lightIndex] = 15;
                    }
                }
            }

            var secData = secBuf.ToBuffer();

            if (emitHeaders)
            {
                writer.WriteShort(mask);
                writer.WriteVarInt(secData.Length + skyLight.Length / 2 + blockLight.Length / 2 + 256);
            }
            
            foreach(byte b in secData)
            {
                writer.WriteByte(b);
            }

            foreach(byte b in blockLight.Data)
            {
                writer.WriteByte(b);
            }

            foreach(byte b in skyLight.Data)
            {
                writer.WriteByte(b);
            }

            for(int i=0; i<256; i++)
            {
                writer.WriteByte(127); // void biome
            }
        }
    }
}
