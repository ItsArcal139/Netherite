﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Netherite.Net
{
    public partial class MinecraftStream : Stream
    {
        private bool disposed;

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public override long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public Stream BaseStream { get; set; }

        private MemoryStream debugMemoryStream;

        public SemaphoreSlim Lock { get; } = new SemaphoreSlim(1, 1);

        public MinecraftStream(bool debug = false) : this(new MemoryStream(), debug) { }

        public MinecraftStream(Stream parent, bool debug = false)
        {
            if (debug)
            {
                debugMemoryStream = new MemoryStream();
            }
            BaseStream = parent;
        }

        public MinecraftStream(byte[] data)
        {
            BaseStream = new MemoryStream(data);
        }

        public Task ClearDebug()
        {
            this.debugMemoryStream.Dispose();
            this.debugMemoryStream = new MemoryStream();

            return Task.CompletedTask;
        }

        public override void Flush() => BaseStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            try
            {
                var read = await BaseStream.ReadAsync(buffer.AsMemory(offset, count), cancellationToken);

                return read;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public virtual async Task<int> ReadAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            try
            {
                var read = await BaseStream.ReadAsync(buffer, cancellationToken);

                return read;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (debugMemoryStream != null)
                await debugMemoryStream.WriteAsync(buffer.AsMemory(offset, count), cancellationToken);

            await BaseStream.WriteAsync(buffer.AsMemory(offset, count), cancellationToken);
        }

        public virtual async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            if (debugMemoryStream != null)
                await debugMemoryStream.WriteAsync(buffer, cancellationToken);

            await BaseStream.WriteAsync(buffer, cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (debugMemoryStream != null)
                debugMemoryStream.Write(buffer, offset, count);

            BaseStream.Write(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);

        public override void SetLength(long value) => BaseStream.SetLength(value);

        public byte[] ToArray()
        {
            this.Position = 0;
            var buffer = new byte[this.Length];
            for (var totalBytesCopied = 0; totalBytesCopied < this.Length;)
                totalBytesCopied += this.Read(buffer, totalBytesCopied, Convert.ToInt32(this.Length) - totalBytesCopied);
            return buffer;
        }

        public override void Close() => BaseStream.Close();

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
            {
                this.BaseStream.Dispose();
                this.Lock.Dispose();
            }

            this.disposed = true;
        }
    }
}
