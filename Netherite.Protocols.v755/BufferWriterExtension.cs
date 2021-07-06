using Netherite.Blocks;
using Netherite.Data.Nbt;
using Netherite.Nbt;
using Netherite.Net.IO;
using Netherite.Utils;
using Netherite.Worlds;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Netherite.Protocols.v755
{
    public static class BufferWriterExtension
    {
        public static void WriteChunk(this BufferWriter writer, Chunk chunk)
        {
            writer.WriteInt(chunk.X);
            writer.WriteInt(chunk.Z);

            int mask = 0;
            BufferWriter col = new BufferWriter();
            foreach(var section in chunk.Sections)
            {
                if(section != null && section.Count != 0)
                {
                    mask |= (int)section.YFlag;
                    col.WriteChunkSection(section);
                }
            }
            writer.WriteVarInt(mask);

            NbtCompound heightmap = new NbtCompound();
            heightmap.Name = "";
            heightmap.Add("MOTION_BLOCKING", new NbtLongArray(chunk.Heightmap.MotionBlocking));
            writer.WriteNbt(heightmap);

            // Biomes
            writer.WriteVarInt(chunk.Biomes.Length);
            foreach (var biome in chunk.Biomes)
            {
                writer.WriteVarInt(biome?.Id ?? 0);
            }

            // Chunk Data
            byte[] buf = col.ToBuffer();
            writer.WriteByteArray(buf);

            // Block entities
            writer.WriteVarInt(0);
        }

        private static int GetArrayLength(int bit)
        {
            int result = 0;
            int b = 0;
            for (int i = 0; i < 4096; i++)
            {
                if (b + bit > 64)
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
                bitsPerBlock = 15;
            }

            bitsPerBlock = 15;

            writer.WriteShort(section.Count);
            writer.WriteByte(bitsPerBlock);

            // Palette & data
            int counter = 0;
            Dictionary<string, ulong> paletteMap = new Dictionary<string, ulong>();
            List<int> palette = new List<int>();
            ulong[] data = new ulong[GetArrayLength(bitsPerBlock)];

            int sLong = 0;
            int sOffset = 0;

            for (int i = 0; i < 4096; i++)
            {
                string state = section.Palette[section.BlockIndices[i]].ToString();
                ulong pid;

                if (bitsPerBlock <= 8)
                {
                    if (!paletteMap.ContainsKey(state))
                    {
                        paletteMap.Add(state, (ulong)counter);

                        var t = Registry.IdState.Find(t => t.Item2 == state);
                        var a = t.Item1;
                        if (t.Item2 == null) a = paletteMap.Count;
                        palette.Add(a);
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
        }
    }
}
