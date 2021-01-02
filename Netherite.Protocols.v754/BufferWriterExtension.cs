using Netherite.Blocks;
using Netherite.Nbt;
using Netherite.Net.IO;
using Netherite.Worlds;
using System.Collections.Generic;

namespace Netherite.Protocols.v754
{
    public static class BufferWriterExtension
    {
        public static void WriteChunk(this BufferWriter writer, Chunk chunk)
        {
            writer.WriteInt(chunk.X);
            writer.WriteInt(chunk.Z);
            writer.WriteBool(true); // full chunk

            int mask = 0;
            BufferWriter col = new BufferWriter();
            for (int sectionY = 0; sectionY < (256 / 16); sectionY++)
            {
                if (!chunk.IsSectionEmpty(sectionY))
                {
                    mask |= (1 << sectionY);
                    col.WriteChunkSection(chunk.Sections[sectionY]);
                }
            }

            writer.WriteVarInt(mask);

            NbtCompound heightmap = new NbtCompound();
            heightmap.Name = "";
            heightmap.Add("MOTION_BLOCKING", new NbtLongArray(chunk.Heightmap.MotionBlocking));
            writer.WriteNbt(heightmap);

            writer.WriteVarInt(1024);
            foreach (var biome in chunk.Biomes)
            {
                writer.WriteVarInt(biome.Id);
            }

            byte[] buf = col.ToBuffer();
            writer.WriteByteArray(buf);

            writer.WriteVarInt(0);
        }

        private static int GetArrayLength(int bit)
        {
            int result = 0;
            int b = 0;
            for (int i = 0; i < 4096; i++)
            {
                if(b + bit > 64)
                {
                    result++;
                    b = 0;
                }
                b += bit;
            }
            if (b != 0) result++;
            return result;
        }

        public static void WriteChunkSection(this BufferWriter writer, ChunkSection section)
        {
            byte bitsPerBlock = section.BitsPerBlock;
            if (bitsPerBlock <= 4)
            {
                bitsPerBlock = 4;
            }

            if (bitsPerBlock > 8)
            {
                bitsPerBlock = 14;
            }

            writer.WriteShort(section.Count);
            writer.WriteByte(bitsPerBlock);

            // Palette & data
            int counter = 0;
            Dictionary<string, ulong> paletteMap = new Dictionary<string, ulong>();
            List<int> palette = new List<int>();
            ulong[] data = new ulong[GetArrayLength(bitsPerBlock)];

            int sLong = 0;
            int sOffset = 0;

            for (int i = 0; i < section.Blocks.Length; i++)
            {
                Block b = section.Blocks[i];
                string state = b.State.ToString();
                ulong pid;

                if (bitsPerBlock <= 8)
                {
                    if (!paletteMap.ContainsKey(state))
                    {
                        paletteMap.Add(state, (ulong)counter);
                        palette.Add(Registry.IdState.Find(t => t.Item2 == state).Item1);
                        pid = (ulong)counter++;
                    }
                    else
                    {
                        pid = paletteMap[state];
                    }
                }
                else
                {
                    pid = (ulong)Registry.IdState.Find(t => t.Item2 == state).Item1;
                }


                if (64 - sOffset < bitsPerBlock)
                {
                    sLong++;
                    sOffset = 0;
                }

                pid &= (ulong)((1 << bitsPerBlock) - 1);
                data[sLong] |= pid << sOffset;
                sOffset += bitsPerBlock;
            }

            if (bitsPerBlock <= 8)
            {
                // Write palette
                writer.WriteVarInt(palette.Count);
                foreach (int id in palette)
                {
                    writer.WriteVarInt(id);
                }
            }

            // Write Data
            writer.WriteVarInt(data.Length);
            foreach (long val in data)
            {
                writer.WriteLong(val);
            }

            foreach (byte b in section.BlockLight.Data)
            {
                writer.WriteByte(b);
            }

            foreach (byte b in section.SkyLight.Data)
            {
                writer.WriteByte(b);
            }

        }
    }
}
