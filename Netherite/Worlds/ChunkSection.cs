using Netherite.Blocks;
using Netherite.Data.Entities;
using Netherite.Data.Nbt;
using Netherite.Texts;
using Netherite.Utils;
using System;
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

            for(int i=0; i<bitPerBlock; i++)
            {
                var n = (byte)((data[longIndex] >> (readBit++)) & 1);
                result |= (byte)(n << i);
            }

            if(RemainingBits() < bitPerBlock)
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

        public Block[] Blocks { get; private set; } = new Block[4096];

        public NibbleArray SkyLight { get; internal set; }

        public NibbleArray BlockLight { get; internal set; }

        internal ChunkSection(Chunk chunk, short yFlag)
        {
            Chunk = chunk;
            ValidateYFlag(yFlag);
            YFlag = yFlag;

            Array.Fill(Blocks, new Block(new BlockState(new Identifier("air"))));
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

                for (int i = 0; i < 4096; i++)
                {
                    byte b = blocks[i];

                    if (b < palette.Count)
                    {
                        NbtBlockState bs = palette[b];
                        BlockState s = BlockState.FromNbt(bs);
                        Blocks[i] = new Block(s);
                    }
                    else
                    {
                        Blocks[i] = new Block(new BlockState(new Identifier("air")));
                    }
                }

                BlockLight = new NibbleArray(section.SkyLight);
            }
            else
            {
                Array.Fill(Blocks, new Block(new BlockState(new Identifier("air"))));

                BlockLight = new NibbleArray(4096);
                BlockLight.Fill(15);
            }

            SkyLight = new NibbleArray(section.SkyLight);
        }

        private void ValidateYFlag(short flag)
        {
            for (int i = 0; i < 16; i++)
            {
                if (Math.Pow(2, i) == flag)
                {
                    return;
                }
            }
            throw new ArgumentException($"Bitmask {flag:b16} is invalid");
        }

        public Block GetBlock(int x, int y, int z) => Blocks[ToOneDimensionIndex(x, y, z)];

        public void SetBlock(int x, int y, int z, Block b) => Blocks[ToOneDimensionIndex(x, y, z)] = b;

        public void FillYWithBlock(int y, Block b)
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

        public int Count => Blocks.Sum(b => b.State.Material == Material.Air ? 0 : 1);

        public bool IsEmpty => Count == 0;
    }
}
