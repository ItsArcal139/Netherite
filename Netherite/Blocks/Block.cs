using System;
using System.Collections.Generic;
using System.Text;
using Netherite.Physics;
using Netherite.Worlds;

namespace Netherite.Blocks
{
    public class Block
    {
        public BlockState State { get; set; }

        public Vector3 Position { get; set; }

        public Block(BlockState state, World world = null)
        {
            State = state;
            World = world;
        }

        public World World { get; set; }

        public Region Region => World?.GetRegion((int)Position.X, (int)Position.Z);

        public Chunk Chunk => World?.GetChunkByBlockPos((int)Position.X, (int)Position.Z);
    }
}
