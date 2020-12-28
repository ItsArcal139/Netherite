using Netherite.Api.Worlds;
using Netherite.Blocks;
using Netherite.Data.Entities;
using Netherite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Netherite.Worlds
{
    public class World : IWorld
    {
        public Identifier Name { get; set; }

        public long Seed { get; set; }

        public Chunk GetChunk(int x, int z) => new Chunk(this, x, z);

        public Block GetBlock(int x, int y, int z) => GetChunk(x, z).GetBlock(x % 16, y, z % 16);

        public void SetBlock(int x, int y, int z, Block b) => GetChunk(x, z).SetBlock(x % 16, y, z % 16, b);

        public Entity SpawnEntity<T>() where T : Entity => throw new NotImplementedException();
    }
}
