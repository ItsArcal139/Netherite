using Netherite.Api.Worlds;
using Netherite.Blocks;
using Netherite.Data.Entities;
using Netherite.Entities;
using Netherite.Worlds.Dimensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Netherite.Worlds
{
    public class World : IWorld
    {
        public Dimension Dimension { get; set; } = Dimension.Overworld;

        public Identifier Name { get; set; }

        public long Seed { get; set; }

        private Dictionary<(int, int), Region> regions = new Dictionary<(int, int), Region>();

        public string Path { get; set; }

        public Region GetRegion(int blockX, int blockZ)
        {
            int rx = (int)Math.Floor((double)blockX / 512);
            int rz = (int)Math.Floor((double)blockZ / 512);

            if (!regions.ContainsKey((rx, rz)))
            {
                var r = Region.ReadFromFile($"{Path}/region/r.{rx}.{rz}.mca", this, rx, rz);
                regions.Add((rx, rz), r);
                return r;
            } else
            {
                return regions[(rx, rz)];
            }
        }

        public Chunk GetChunk(int x, int z)
        {
            var region = GetRegion(x * 16, z * 16);
            return region.GetChunk(x, z);
        }

        public Chunk GetChunkByBlockPos(int blockX, int blockZ)
        {
            var region = GetRegion(blockX, blockZ);
            int cx = (int)Math.Floor((double)blockX / 16);
            int cz = (int)Math.Floor((double)blockZ / 16);
            return region.GetChunk(cx, cz);
        }

        public World(string path)
        {
            Path = path;
            GetChunk(0, 0);
        }

        public Block GetBlock(int x, int y, int z) => GetChunkByBlockPos(x, z).GetBlock(x % 16, y, z % 16);

        public void SetBlock(int x, int y, int z, Block b) => GetChunkByBlockPos(x, z).SetBlock(x % 16, y, z % 16, b);

        public Entity SpawnEntity<T>() where T : Entity => throw new NotImplementedException();
    }
}
