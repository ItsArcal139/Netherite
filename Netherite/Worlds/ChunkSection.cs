using Netherite.Blocks;
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

        internal ChunkSection(Chunk chunk, short yFlag)
        {
            Chunk = chunk;
            ValidateYFlag(yFlag);
            YFlag = yFlag;

            Array.Fill(Blocks, new Block(Material.Air));
        }

        private void ValidateYFlag(short flag)
        {
            for(int i=0; i<16; i++)
            {
                if(Math.Pow(2, i) == flag)
                {
                    return;
                }
            }
            throw new ArgumentException($"Bitmask {flag:b16} is invalid");
        }

        public Block GetBlock(int x, int y, int z) => Blocks[ToOneDimensionIndex(x, y, z)];

        public void SetBlock(int x, int y, int z, Block b) => Blocks[ToOneDimensionIndex(x, y, z)] = b;

        private int ToOneDimensionIndex(int x, int y, int z) => (y * 16 + z) * 16 + x;

        public int Count => Blocks.Sum(b => b.Material == Material.Air ? 0 : 1);

        public bool IsEmpty => Count == 0;
    }
}
