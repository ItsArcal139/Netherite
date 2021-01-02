using Netherite.Data.Entities;

namespace Netherite.Worlds.Biomes
{
    public struct BiomeMoodSound
    {
        public int BlockSearchExtent { get; set; }

        public double Offset { get; set; }

        public Identifier Sound { get; set; }

        public int TickDelay { get; set; }
    }
}
