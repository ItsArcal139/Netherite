using Netherite.Api.Worlds;
using Netherite.Data.Entities;
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
    }
}
