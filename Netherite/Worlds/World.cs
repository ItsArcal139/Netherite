using Netherite.Api.Worlds;
using Netherite.Blocks;
using Netherite.Data.Entities;
using Netherite.Entities;
using Netherite.Exceptions;
using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Utils;
using Netherite.Worlds.Biomes;
using Netherite.Worlds.Dimensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Netherite.Worlds
{
    public class World : IWorld
    {
        public Dimension Dimension { get; set; } = Dimension.Overworld;

        public Identifier Name { get; set; }

        /// <summary>
        /// The user-friendly level name in-game.
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// The random seed of the world.
        /// </summary>
        public long Seed { get; set; }

        /// <summary>
        /// The list of loaded regions of the world.
        /// </summary>
        private readonly Dictionary<(int, int), Region> regions = new Dictionary<(int, int), Region>();

        /// <summary>
        /// The root directory path of the world.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The time of the world.
        /// </summary>
        public long Time { get; set; } = 0;

        /// <summary>
        /// The game version of the world.
        /// </summary>
        public GameVersion Version { get; set; }

        /// <summary>
        /// Creates an instance of the world at the given path.
        /// </summary>
        /// <param name="path">The path of the world.</param>
        /// <exception cref="WorldNotFoundException" />
        /// <exception cref="WorldFileCorruptedException" />
        public World(string path)
        {
            Path = path;
            ReadLevelFile();
        }

        /// <summary>
        /// Reads level.dat and stores level informations.
        /// </summary>
        /// <exception cref="WorldNotFoundException" />
        /// <exception cref="WorldFileCorruptedException" />
        private void ReadLevelFile()
        {
            Logger.Log($"Reading level.dat from world folder \"{Path}\"...");

            if (!Directory.Exists(Path))
            {
                throw new WorldNotFoundException(this);
            }

            try
            {
                byte[] buffer = GZipUtils.Decompress(File.ReadAllBytes($"{Path}/level.dat"));
                NbtCompound data = (NbtCompound)((NbtCompound)NbtTag.Deserialize(buffer, true))["Data"];

                Version = NbtConvert.Deserialize<GameVersion>(data["Version"]);
                Logger.Log($"Version: {Version.Name} ({Version.DataVersion})" + (Version.IsSnapshot ? ", Snapshot" : ""));

                LevelName = ((NbtString)data["LevelName"]).ToValue();
                Logger.Log($"Level name: {LevelName}");

                // Seed value location is changed since 1.16
                if (Version.DataVersion < 2566)
                {
                    // Seed is saved under $.RandomSeed
                    Seed = ((NbtLong)data["RandomSeed"]).Value;
                }
                else
                {
                    // Seed is saved under $.WorldGenSettings.seed
                    Seed = ((NbtLong)((NbtCompound)data["WorldGenSettings"])["seed"]).Value;
                }
                Logger.Log($"Seed: {Seed}");
            } catch(Exception ex)
            {
                throw new WorldFileCorruptedException(this, $"{Path}/level.dat", ex);
            }
        }

        private SemaphoreSlim dictLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Get a region by a specified block position.
        /// </summary>
        /// <param name="blockX">The X value in blocks.</param>
        /// <param name="blockZ">The Z value in blocks.</param>
        /// <returns>The region at the specified position.</returns>
        public Region GetRegion(int blockX, int blockZ)
        {
            dictLock.Wait();

            int rx = (int)Math.Floor((double)blockX / 512);
            int rz = (int)Math.Floor((double)blockZ / 512);

            if (!regions.ContainsKey((rx, rz)))
            {
                var r = Region.FromFile($"{Path}/region/r.{rx}.{rz}.mca", this, rx, rz);
                regions.Add((rx, rz), r);
                dictLock.Release();
                return r;
            }
            else
            {
                dictLock.Release();
                return regions[(rx, rz)];
            }
        }

        public async Task<Region> GetRegionAsync(int blockX, int blockZ)
        {
            return await Task.Run(() =>
            {
                return GetRegion(blockX, blockZ);
            });
        }

        /// <summary>
        /// Get a chunk by its X and Z position.
        /// </summary>
        /// <param name="chunkX">The X value in chunks.</param>
        /// <param name="chunkZ">The Z value in chunks.</param>
        /// <returns>The desired chunk.</returns>
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
                if (block.State.Material != Material.Air)
                {
                    break;
                }
            }
            return y;
        }

        public void Tick()
        {
            Time++;
            foreach (var entity in Entities)
            {
                entity.Tick();
            }
        }
    }
}
