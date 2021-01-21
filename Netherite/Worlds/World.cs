using Netherite.Api.Worlds;
using Netherite.Blocks;
using Netherite.Data.Entities;
using Netherite.Entities;
using Netherite.Worlds.Biomes;
using Netherite.Worlds.Dimensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var r = Region.FromFile($"{Path}/region/r.{rx}.{rz}.mca", this, rx, rz);
                regions.Add((rx, rz), r);
                return r;
            }
            else
            {
                return regions[(rx, rz)];
            }
        }

        public Chunk GetChunk(int chunkX, int chunkZ)
        {
            var region = GetRegion(chunkX * 16, chunkZ * 16);
            return region.GetChunk(chunkX, chunkZ);
        }

        public async Task<Chunk> GetChunkAsync(int x, int z)
        {
            var region = GetRegion(x * 16, z * 16);
            return await region.GetChunkAsync(x, z);
        }

        public Chunk GetChunkByBlockPos(int blockX, int blockZ)
        {
            var region = GetRegion(blockX, blockZ);
            int cx = (int)Math.Floor((double)blockX / 16);
            int cz = (int)Math.Floor((double)blockZ / 16);
            return region.GetChunk(cx, cz);
        }

        public async Task<Chunk> GetChunkByBlockPosAsync(int blockX, int blockZ)
        {
            var region = GetRegion(blockX, blockZ);
            int cx = (int)Math.Floor((double)blockX / 16);
            int cz = (int)Math.Floor((double)blockZ / 16);
            return await region.GetChunkAsync(cx, cz);
        }

        public Biome GetBiome(int blockX, int y, int blockZ)
        {
            var region = GetRegion(blockX, blockZ);
            int cx = (int)Math.Floor((double)blockX / 16);
            int cz = (int)Math.Floor((double)blockZ / 16);
            return region.GetChunk(cx, cz).GetBiome(blockX - cx * 16, y, blockZ - cz * 16);
        }

        public async Task<Biome> GetBiomeAsync(int blockX, int y, int blockZ)
        {
            var region = GetRegion(blockX, blockZ);
            int cx = (int)Math.Floor((double)blockX / 16);
            int cz = (int)Math.Floor((double)blockZ / 16);

            var chunk = await region.GetChunkAsync(cx, cz);
            return chunk.GetBiome(blockX - cx * 16, y, blockZ - cz * 16);
        }

        public World(string path)
        {
            Path = path;
            GetChunk(0, 0);
        }

        public Block GetBlock(int x, int y, int z) => GetChunkByBlockPos(x, z).GetBlock(x % 16, y, z % 16);

        public void SetBlock(int x, int y, int z, BlockState b) => GetChunkByBlockPos(x, z).SetBlock(x % 16, y, z % 16, b);

        public List<Entity> Entities { get; private set; } = new List<Entity>();

        public Entity SpawnEntity<T>() where T : Entity => throw new NotImplementedException();

        public int GetHighestBlockY(int x, int z)
        {
            int y = 255;
            for (; y >= 0; y--)
            {
                var block = GetBlock(x, y, z);
                if(block.State.Material != Material.Air)
                {
                    break;
                }
            }
            return y;
        }

        public void Tick()
        {
            foreach (var entity in Entities)
            {
                entity.Tick();
            }
        }
    }
}
