using Netherite.Api.Worlds;
using Netherite.Blocks;
using Netherite.Data.Nbt;
using Netherite.Worlds.Biomes;
using System;

namespace Netherite.Worlds
{
    public struct Heightmap
    {
        public long[] MotionBlocking { get; set; }
        public long[] MotionBlockingNoLeaves { get; set; }
        public long[] OceanFloor { get; set; }
        public long[] WorldSurface { get; set; }
    }

    public class Chunk : IChunk
    {
        public int DataVersion => Region.DataVersion;

        public int X { get; private set; }

        public int Z { get; private set; }

        public Heightmap Heightmap { get; private set; }

        public IWorld World => Region.World;

        public Region Region { get; private set; }

        public Biome[] Biomes { get; private set; }

        internal Chunk(Region region, int x, int z)
        {
            Region = region;
            X = x;
            Z = z;
        }

        internal Chunk(Region region, int x, int z, NbtLevel level)
        {
            Region = region;
            X = x;
            Z = z;

            foreach(var section in level.Sections)
            {
                var y = section.Y;
                if (y == 255) continue;

                Sections[y] = new ChunkSection(this, section);
            }

            Heightmap = new Heightmap
            {
                MotionBlocking = level.Heightmaps.MotionBlocking,
                MotionBlockingNoLeaves = level.Heightmaps.MotionBlockingNoLeaves,
                OceanFloor = level.Heightmaps.OceanFloor,
                WorldSurface = level.Heightmaps.WorldSurface
            };

            Biomes = new Biome[level.Biomes.Length];
            for(int i=0; i<level.Biomes.Length; i++)
            {
                Biomes[i] = Biome.GetBiome(level.Biomes[i]);
            }
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

        public void FillYWithBlock(int y, Block b) => GetChunkSectionByPosY(y).FillYWithBlock(y % 16, b);

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

        public Biome GetBiome(int x, int y, int z)
        {
            // Index formula from https://wiki.vg/Chunk_Format#Biomes
            int index = ((y >> 2) & 63) << 4 | ((z >> 2) & 3) << 2 | ((x >> 2) & 3);
            return Biomes[index];
        }
    }
}
