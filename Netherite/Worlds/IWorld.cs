using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Api.Worlds
{
    public interface IWorld
    {
        long Seed { get; }
    }

    public interface IChunk
    {
        int X { get; }

        int Z { get; }

        IWorld World { get; }
    }
}
