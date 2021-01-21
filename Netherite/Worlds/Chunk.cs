using Netherite.Api.Worlds;
using Netherite.Blocks;
using Netherite.Data.Nbt;
using Netherite.Net.Protocols;
using Netherite.Utils;
using Netherite.Worlds.Biomes;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public Biome[] Biomes { get; private set; } = new Biome[1024];

        public ChunkSection[] Sections { get; private set; }

        public DateTime LastModifiedTime => Region.GetLastModifiedTime(this);

        internal Chunk(Region region, int x, int z)
        {
            Region = region;
            X = x;
            Z = z;
            Heightmap = new Heightmap
            {
                MotionBlocking = new long[36]
            };
            Sections = new ChunkSection[16];
            Array.Fill(Biomes, Biome.GetBiome(127));
        }

        internal Chunk(Region region, int x, int z, NbtLevel level)
        {
            Region = region;
            X = x;
            Z = z;

            Sections = new ChunkSection[level.Biomes.Length / 64];

            if (level.Sections != null)
            {
                foreach (var section in level.Sections)
                {
                    var y = section.Y;
                    if (y == 255) continue;

                    Sections[y] = new ChunkSection(this, section);
                }
            }

            Heightmap = new Heightmap
            {
                MotionBlocking = level.Heightmaps.MotionBlocking ?? new long[36],
                MotionBlockingNoLeaves = level.Heightmaps.MotionBlockingNoLeaves ?? new long[36],
                OceanFloor = level.Heightmaps.OceanFloor ?? new long[36],
                WorldSurface = level.Heightmaps.WorldSurface ?? new long[36]
            };

            Biomes = new Biome[level.Biomes.Length];
            for(int i=0; i<level.Biomes.Length; i++)
            {
                Biomes[i] = Biome.GetBiome(level.Biomes[i]);
            }
        }

        public bool IsSectionEmpty(int index)
        {
            if (index >= Sections.Length) return true;
            ChunkSection s = Sections[index];
            if (s == null) return true;

            return s.IsEmpty;
        }

        public Block GetBlock(int x, int y, int z)
        {
            int sy = y % 16;
            return GetChunkSectionByPosY(y).GetBlock(x, sy, z);
        }

        public void SetBlock(int x, int y, int z, BlockState b)
        {
            int sy = y % 16;
            GetChunkSectionByPosY(y).SetBlock(x, sy, z, b);
        }

        public void FillYWithBlock(int y, BlockState b) => GetChunkSectionByPosY(y).FillYWithBlock(y % 16, b);

        public ChunkSection GetChunkSectionByPosY(int y)
        {
            Preconditions.IsPositive(y, "y");
            Preconditions.Ensure(y <= 256, "y cannot be more than 256.");

            int index = (int)Math.Floor((double)y / 16);
            if(Sections[index] == null)
            {
                Sections[index] = new ChunkSection(this, (short)(1 << index));
            }
            return Sections[index];
        }

        public Biome GetBiome(int chunkX, int chunkY, int chunkZ)
        {
            // Index formula from https://wiki.vg/Chunk_Format#Biomes
            int index = ((chunkY >> 2) & 63) << 4 | ((chunkZ >> 2) & 3) << 2 | ((chunkX >> 2) & 3);
            return Biomes[index];
        }

        [Obsolete]
        public Biome GetBiome(int chunkX, int chunkZ) => GetBiome(chunkX, 0, chunkZ);

        internal NbtLevel WriteToNbt()
        {
            NbtLevel root = new NbtLevel();
            root.DataVersion = Region.LatestStableDataVersion;
            root.Biomes = Biomes.ToList().ConvertAll(b => b.Id).ToArray();
            root.Entities = new List<object>();
            root.Heightmaps = new NbtLevel.HeightMapList
            {
                // TODO: Write real data here
                MotionBlocking = new long[36],
                MotionBlockingNoLeaves = new long[36],
                OceanFloor = new long[36],
                WorldSurface = new long[36]
            };
            root.InhabitedTime = 0;
            root.LastUpdate = (long)(DateTime.Now - DateTime.UnixEpoch).TotalSeconds;
            root.Sections = new List<NbtLevel.NbtSection>();
            foreach(var s in Sections)
            {
                root.Sections.Add(s.WriteToNbt());
            }

            return root;
        }
    }
}
