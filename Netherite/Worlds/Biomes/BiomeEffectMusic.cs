using Netherite.Data.Entities;

namespace Netherite.Worlds.Biomes
{
    public struct BiomeEffectMusic
    {
        public bool ReplaceCurrentMusic { get; set; }
        public Identifier Sound { get; set; }
        public int MinDelay { get; set; }
        public int MaxDelay { get; set; }
    }
}
