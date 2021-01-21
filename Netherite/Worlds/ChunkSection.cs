using Netherite.Blocks;
using Netherite.Data.Entities;
using Netherite.Data.Nbt;
using Netherite.Nbt;
using Netherite.Texts;
using Netherite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Netherite.Worlds
{
    internal class SectionDataReader
    {
        private long[] data;
        private int bitPerBlock;

        private int longIndex = 0;
        private byte readBit = 0;

        private int callCount = 0;

        internal SectionDataReader(long[] data)
        {
            this.data = data;
            bitPerBlock = data.Length / 64;
        }

        internal int RemainingBits()
        {
            return 64 - readBit;
        }

        internal byte NextBlock1_16()
        {
            callCount++;

            byte result = 0;

            for (int i = 0; i < bitPerBlock; i++)
            {
                var n = (byte)((data[longIndex] >> (readBit++)) & 1);
                result |= (byte)(n << i);
            }

            if (RemainingBits() < bitPerBlock)
            {
                longIndex++;
                readBit = 0;
            }

            return result;
        }

        internal byte NextBlockPre1_16()
        {
            callCount++;

            byte result = 0;

            for (int i = 0; i < bitPerBlock; i++)
            {
                if (readBit >= 64)
                {
                    longIndex++;
                    readBit = 0;
                }

                var n = (byte)((data[longIndex] >> (readBit++)) & 1);
                result |= (byte)(n << i);
            }

            return result;
        }
    }

    public class ChunkSection
    {
        public Chunk Chunk { get; private set; }

        public short YFlag { get; private set; }

        public byte BitsPerBlock { get; private set; }

        public byte StartY => (byte)(Math.Log2(YFlag) * 16);

        public Range YRange => StartY..(StartY + 15);

        public int[] BlockIndices { get; private set; } = new int[4096];

        public List<BlockState> Palette { get; set; } = new List<BlockState>();

        public NibbleArray SkyLight { get; internal set; }

        public NibbleArray BlockLight { get; internal set; }

        internal ChunkSection(Chunk chunk, short yFlag)
        {
            Chunk = chunk;
            ValidateYFlag(yFlag);
            YFlag = yFlag;

            Palette.Add(new BlockState(new Identifier("air")));
            Array.Fill(BlockIndices, 0);
        }

        internal ChunkSection(Chunk chunk, NbtLevel.NbtSection section)
        {
            Chunk = chunk;
            YFlag = (short)(1 << section.Y);

            if (section.BlockStates != null)
            {
                byte[] blocks = new byte[4096];
                SectionDataReader reader = new SectionDataReader(section.BlockStates);
                BitsPerBlock = (byte)(section.BlockStates.Length / 64);

                for (int i = 0; i < 4096; i++)
                {
                    blocks[i] = chunk.DataVersion < 2566 ? reader.NextBlockPre1_16() : reader.NextBlock1_16();
                }

                var palette = section.Palette;
                foreach (var p in palette)
                {
                    BlockState s = BlockState.FromNbt(p);
                    Palette.Add(s);
                }

                for (int i = 0; i < 4096; i++)
                {
                    BlockIndices[i] = blocks[i];
                }

                byte[] blbuf = section.BlockLight;
                if (blbuf == null) blbuf = new byte[2048];
                BlockLight = new NibbleArray(blbuf);
            }
            else
            {
                Palette.Add(new BlockState(new Identifier("air")));
                Array.Fill(BlockIndices, (byte)0);
                BitsPerBlock = 4;

                BlockLight = new NibbleArray(4096);
                BlockLight.Fill(15);
            }

            byte[] slbuf = section.SkyLight;
            if (slbuf == null) slbuf = new byte[2048];
            SkyLight = new NibbleArray(slbuf);
        }

        private void ValidateYFlag(short flag)
        {
            for (int i = 0; i < 16; i++)
            {
                if ((short)(1 << i) == flag)
                {
                    return;
                }
            }
            throw new ArgumentException($"Bitmask {Convert.ToString(flag, 2)} is invalid");
        }

        public Block GetBlock(int x, int y, int z) => GetBlock(ToOneDimensionIndex(x, y, z));

        public Block GetBlock(int index) => new Block(Palette[BlockIndices[index]]);

        public void SetBlock(int x, int y, int z, BlockState b)
        {
            if (!Palette.Contains(b))
            {
                Palette.Add(b);
                BlockIndices[ToOneDimensionIndex(x, y, z)] = Palette.Count - 1;
            }
            else
            {
                BlockIndices[ToOneDimensionIndex(x, y, z)] = Palette.FindIndex(a => a.Equals(b));
            }
        }

        public void FillYWithBlock(int y, BlockState b)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    SetBlock(x, y, z, b);
                }
            }
        }

        private int ToOneDimensionIndex(int x, int y, int z) => (y * 16 + z) * 16 + x;

        public int Count => BlockIndices.Sum(b => Palette[b].Material == Material.Air ? 0 : 1);

        public bool IsEmpty => Count == 0;

        internal NbtLevel.NbtSection WriteToNbt()
        {
            var root = new NbtLevel.NbtSection();

            byte bitsPerBlock = BitsPerBlock;
            if (bitsPerBlock <= 4)
            {
                bitsPerBlock = 4;
            }

            if (bitsPerBlock > 8)
            {
                bitsPerBlock = 15;
            }

            // Palette & data
            int counter = 0;
            Dictionary<BlockState, ulong> paletteMap = new Dictionary<BlockState, ulong>();
            ulong[] data = new ulong[GetDataArrayLength(bitsPerBlock)];

            int sLong = 0;
            int sOffset = 0;

            for (int i = 0; i < 4096; i++)
            {
                BlockState state = GetBlock(i).State;
                ulong pid;

                if (!paletteMap.ContainsKey(state))
                {
                    paletteMap.Add(state, (ulong)counter);
                    pid = (ulong)counter++;
                }
                else
                {
                    pid = paletteMap[state];
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

            root.BlockStates = data.ToList().ConvertAll(l => (long)l).ToArray();
            root.Palette = new List<NbtBlockState>();
            foreach (var s in paletteMap.Keys)
            {
                NbtBlockState item = new NbtBlockState();
                item.Name = "minecraft:" + Enum.GetName(typeof(Material), s.Material).ToSnakeCase();
                if (s.Properties.Count > 0)
                {
                    item.Properties = new NbtCompound();
                    foreach (var pair in s.Properties)
                    {
                        item.Properties.Add(pair.Key, new NbtString(pair.Value));
                    }
                }
            }

            return root;
        }

        private static int GetDataArrayLength(int bit)
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
    }
}
