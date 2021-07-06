using System;
using Netherite.Worlds;

namespace Netherite.Exceptions
{
    public class NetheriteException : Exception
    {
        public NetheriteException() { }

        public NetheriteException(string msg) : base(msg) { }

        public NetheriteException(string msg, Exception inner) : base(msg, inner) { }
    }

    public class NetheriteWorldException : NetheriteException
    {
        public World World { get; private set; }

        public NetheriteWorldException(World world)
        {
            World = world;
        }

        public NetheriteWorldException(World world, string msg) : base(msg)
        {
            World = world;
        }

        public NetheriteWorldException(World world, string msg, Exception inner) : base(msg, inner)
        {
            World = world;
        }
    }

    public class WorldNotFoundException : NetheriteWorldException
    {
        public WorldNotFoundException(World world) : base(world, @$"The world at ""{world.Path}"" is not found.") { }
    }

    public class WorldFileCorruptedException : NetheriteWorldException
    {
        public WorldFileCorruptedException(World world, string filePath) : base(world, @$"The file at ""{filePath}"" is corrupted.") { }

        public WorldFileCorruptedException(World world, string filePath, Exception inner) : base(world, @$"The file at ""{filePath}"" is corrupted.", inner) { }
    }
}
