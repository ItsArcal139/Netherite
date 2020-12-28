using Netherite.Api.Worlds;
using Netherite.Blocks;
using System;

namespace Netherite.Worlds
{
    public class Chunk : IChunk
    {
        public int X { get; private set; }

        public int Z { get; private set; }

        public IWorld World { get; private set; }

        internal Chunk(World world, int x, int z)
        {
            World = world;
            X = x;
            Z = z;

            FillYWithBlock(0, new Block(Material.OakPlanks));
        }

        public ChunkSection[] Sections { get; set; } = new ChunkSection[16];

        public bool IsSectionEmpty(int index)
        {
            ChunkSection s = Sections[index];
            if (s == null) return true;

            return s.IsEmpty;
        }

        public Block GetBlock(int x, int y, int z)
        {
            int sy = y % 16;
            return GetChunkSectionByPosY(y).GetBlock(x, sy, z);
        }

        public void SetBlock(int x, int y, int z, Block b)
        {
            int sy = y % 16;
            GetChunkSectionByPosY(y).SetBlock(x, sy, z, b);
        }

        public void FillYWithBlock(int y, Block b)
        {
            for(int x = 0; x < 16; x++)
            {
                for(int z = 0; z < 16; z++)
                {
                    SetBlock(x, y, z, b);
                }
            }
        }

        public ChunkSection GetChunkSectionByPosY(int y)
        {
            if (y > 256) throw new ArgumentException("y cannot be more than 256");
            if (y < 0) throw new ArgumentException("y cannot be less than 0");

            int index = (int)Math.Floor((double)y / 16);
            if(Sections[index] == null)
            {
                Sections[index] = new ChunkSection(this, (byte)(1 << index));
            }
            return Sections[index];
        }

        public Biome GetBiome(int x, int z)
        {
            return new Biome { Id = 127 };
        }
    }

    public class Biome
    {
        public byte Id { get; set; }
    }
}
