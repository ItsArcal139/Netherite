using Netherite.Data;
using Netherite.Data.Entities;

namespace Netherite.Worlds.Biomes
{
    public struct BiomeEffects
    {
        public Color FogColor { get; set; }

        public Color SkyColor { get; set; }

        public Color WaterColor { get; set; }

        public Color WaterFogColor { get; set; }

        public BiomeMoodSound? MoodSound { get; set; }

        public Color? FoliageColor { get; set; }

        public string GrassColorModifier { get; set; }

        public BiomeEffectMusic? Music { get; set; }

        public Identifier? AmbientSound { get; set; }

        public BiomeAdditionsSound? AdditionsSound { get; set; }

        public BiomeParticle? Particle { get; set; }
    }
}
