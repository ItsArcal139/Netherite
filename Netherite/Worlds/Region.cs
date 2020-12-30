using Netherite.Data.Nbt;
using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Netherite.Worlds
{
    public class Region : IDisposable
    {
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

            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(a);
            }
            int offset = BitConverter.ToInt32(a);
            return (offset * 4096, b * 4096);
        }

        public Chunk GetChunk(int chunkX, int chunkZ)
        {
            if(map.ContainsKey((chunkX, chunkZ)))
            {
                return map[(chunkX, chunkZ)];
            }

            var (offset, size) = GetChunkOffsetAndSize(chunkX, chunkZ);

            byte[] a = new byte[4];
            fs.Seek(offset, SeekOrigin.Begin);
            fs.Read(a, 0, 4);
            int b = fs.ReadByte();

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(a);
            }

            int length = BitConverter.ToInt32(a);
            if (b == 1)
            {
                throw new NotSupportedException("Gzip is not supported");
            }

            byte[] buf = new byte[length];
            fs.Read(buf, 0, length);

            int i = 0;
            byte[] chunk = ZLibUtils.Decompress(buf);
            File.WriteAllBytes($"{fs.Name}-({chunkX},{chunkZ}).nbt", chunk);

            NbtCompound c = (NbtCompound)NbtTag.Deserialize(chunk, ref i, true);
            var level = NbtConvert.Deserialize<NbtLevel>(c["Level"]);

            Chunk ck = new Chunk(this, chunkX, chunkZ, level);
            map[(chunkX, chunkZ)] = ck;
            return ck;
        }

        public static Region ReadFromFile(string name, World w, int x, int z)
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
