using Netherite.Data.Nbt;
using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Netherite.Worlds
{
    public class Region : IDisposable
    {
        private static bool warnedUnstable = false;

        public const int LatestStableDataVersion = 2584;

        public int DataVersion { get; private set; }

        public int X { get; private set; }

        public int Z { get; private set; }

        public World World { get; private set; }

        private Region(World w, int x, int z)
        {
            World = w;
            X = x;
            Z = z;
        }

        private int FromChunkPos(int n) => (int)Math.Floor((double)n / 32);

        public string DefaultFileName => "r." + FromChunkPos(X) + "." + FromChunkPos(Z) + ".mca";

        private byte[] header = new byte[8192];

        private FileStream fs = null;
        private bool disposedValue;

        private int GetChunkLocationEntry(int chunkX, int chunkZ) => ((chunkX - X * 32) + (chunkZ - Z * 32) * 32) * 4;

        private Dictionary<(int, int), Chunk> map = new Dictionary<(int, int), Chunk>();

        private (int, int) GetChunkOffsetAndSize(int chunkX, int chunkZ)
        {
            int index = GetChunkLocationEntry(chunkX, chunkZ);
            byte[] a = new byte[4];

            fs.Seek(index, SeekOrigin.Begin);
            fs.Read(a, 1, 3);
            int b = fs.ReadByte();

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(a);
            }
            int offset = BitConverter.ToInt32(a);
            return (offset * 4096, b * 4096);
        }

        public Chunk GetChunk(int chunkX, int chunkZ)
        {
            if (!map.ContainsKey((chunkX, chunkZ)))
            {
                LoadChunk(chunkX, chunkZ);
            }
            return map[(chunkX, chunkZ)];
        }

        public void LoadChunk(int chunkX, int chunkZ)
        {
            // Don't know how to use the 2nd parameter...
            var (offset, _) = GetChunkOffsetAndSize(chunkX, chunkZ);

            byte[] a = new byte[4];
            fs.Seek(offset, SeekOrigin.Begin);
            fs.Read(a, 0, 4);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(a);
            }

            int length = BitConverter.ToInt32(a);
            int compressMode = fs.ReadByte();

            // Read compressed data.
            byte[] buf = new byte[length];
            fs.Read(buf, 0, length);

            // Decompress the data.
            byte[] chunk;
            if (compressMode == 1)
            {
                chunk = GZipUtils.Decompress(buf);
            }
            else
            {
                chunk = ZLibUtils.Decompress(buf);
            }

            int i = 0;

            NbtCompound c = (NbtCompound)NbtTag.Deserialize(chunk, ref i, true);
            DataVersion = ((NbtInt)c["DataVersion"]).Value;
            if (DataVersion > LatestStableDataVersion && !warnedUnstable)
            {
                warnedUnstable = true;
                Logger.Warn("*** You are loading a newer world. ***");
                Logger.Warn("Although the original world will not be changed, the world data");
                Logger.Warn("processed by Netherite could be corrupted. This is not supported.");
            }

            // File.WriteAllBytes(fs.Name + $"({chunkX},{chunkZ}).nbt", NbtConvert.SerializeToBuffer(c));
            var level = NbtConvert.Deserialize<NbtLevel>(c["Level"]);

            Chunk ck = new Chunk(this, chunkX, chunkZ, level);
            map[(chunkX, chunkZ)] = ck;
        }

        private SemaphoreSlim chunkLoadLock = new SemaphoreSlim(1, 1);

        public async Task LoadChunkAsync(int chunkX, int chunkZ)
        {
            await chunkLoadLock.WaitAsync();
            LoadChunk(chunkX, chunkZ);
            chunkLoadLock.Release();
        }

        public async Task<Chunk> GetChunkAsync(int chunkX, int chunkZ)
        {
            if (!map.ContainsKey((chunkX, chunkZ)))
            {
                await LoadChunkAsync(chunkX, chunkZ);
            }
            return map[(chunkX, chunkZ)];
        }

        public static Region FromFile(string name, World w, int x, int z)
        {
            Region result = new Region(w, x, z);
            FileStream fs = new FileStream(name, FileMode.Open);
            fs.Read(result.header, 0, result.header.Length);

            result.fs = fs;
            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    fs.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Region()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
