using Netherite.Blocks;
using Netherite.Data.Entities;
using Netherite.Data.Nbt;
using Netherite.Utils;
using System;
using System.Linq;

namespace Netherite.Worlds
{
    public class ChunkSection
    {
        public Chunk Chunk { get; private set; }

        public short YFlag { get; private set; }

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

            Array.Fill(Blocks, new Block(new BlockState
            {
                Id = new Identifier("air")
            }));
        }

        internal ChunkSection(Chunk chunk, NbtLevel.NbtSection section)
        {
            Chunk = chunk;
            YFlag = (short)(1 << section.Y);

            long[] buf = section.BlockStates;

            NibbleArray blocks = new NibbleArray(4096);

            for (int i = 0; i < 256; i++)
            {
                long l = buf[i];

                byte[] data = BitConverter.GetBytes(l);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(data);
                }

                for (int j = 0; j < 8; j++)
                {
                    blocks.Data[i * 8 + j] = data[j];
                }
            }

            var palette = section.Palette;

            for (int i = 0; i < 4096; i++)
            {
                byte b = blocks[i];
                NbtBlockState bs = palette[b];

                BlockState s = BlockState.FromNbt(bs);

                Blocks[i] = new Block(s);
            }

            SkyLight = new NibbleArray(section.SkyLight);
            BlockLight = new NibbleArray(section.SkyLight);
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

        public int Count => Blocks.Sum(b => b.State.Id.Key == "air" ? 0 : 1);

        public bool IsEmpty => Count == 0;
    }
}
