using Netherite.Data;
using Netherite.Data.Entities;
using Netherite.Nbt;
using Netherite.Utils;

namespace Netherite.Worlds.Biomes
{
    internal class MinecraftBiome : Biome
    {
        internal MinecraftBiome(NbtCompound c) : base()
        {
            string[] name = ((NbtString)c["name"]).Value.Split(':', 2);
            Name = new Identifier(name[1], name[0]);

            NbtCompound element = (NbtCompound)c["element"];
            Category = ((NbtString)element["category"]).Value;
            Depth = ((NbtFloat)element["depth"]).Value;
            Downfall = ((NbtFloat)element["downfall"]).Value;
            Precipitation = ((NbtString)element["precipitation"]).Value.ToEnum<Precipitation>();
            Scale = ((NbtFloat)element["scale"]).Value;
            Temperature = ((NbtFloat)element["temperature"]).Value;

            NbtCompound e = (NbtCompound)element["effects"];
            BiomeEffects effects = new BiomeEffects();
            effects.SkyColor = new Color(((NbtInt)e["sky_color"]).Value);
            effects.WaterFogColor = new Color(((NbtInt)e["water_fog_color"]).Value);
            effects.FogColor = new Color(((NbtInt)e["fog_color"]).Value);
            effects.WaterColor = new Color(((NbtInt)e["water_color"]).Value);

            if (e.ContainsKey("foliage_color"))
            {
                effects.FoliageColor = new Color(((NbtInt)e["foliage_color"]).Value);
            }

            if (e.ContainsKey("grass_color_modifier"))
            {
                effects.GrassColorModifier = ((NbtString)e["grass_color_modifier"]).Value;
            }

            if (e.ContainsKey("music"))
            {
                BiomeEffectMusic music = new BiomeEffectMusic();
                NbtCompound m = (NbtCompound)e["music"];
                music.ReplaceCurrentMusic = ((NbtByte)m["replace_current_music"]).AsBool;
                string[] sound = ((NbtString)m["sound"]).Value.Split(':', 2);
                music.Sound = new Identifier(sound[1], sound[0]);
                music.MaxDelay = ((NbtInt)m["max_delay"]).Value;
                music.MinDelay = ((NbtInt)m["min_delay"]).Value;
                effects.Music = music;
            }

            if (e.ContainsKey("ambient_sound"))
            {
                string[] sound = ((NbtString)e["ambient_sound"]).Value.Split(':', 2);
                effects.AmbientSound = new Identifier(sound[1], sound[0]);
            }

            if (e.ContainsKey("additions_sound"))
            {
                BiomeAdditionsSound additions = new BiomeAdditionsSound();
                NbtCompound m = (NbtCompound)e["additions_sound"];
                string[] sound = ((NbtString)m["sound"]).Value.Split(':', 2);
                additions.Sound = new Identifier(sound[1], sound[0]);
                additions.TickChance = ((NbtDouble)m["tick_chance"]).Value;
                effects.AdditionsSound = additions;
            }

            if (e.ContainsKey("mood_sound"))
            {
                BiomeMoodSound moodSound = new BiomeMoodSound();
                NbtCompound m = (NbtCompound)e["mood_sound"];
                string[] sound = ((NbtString)m["sound"]).Value.Split(':', 2);
                moodSound.Sound = new Identifier(sound[1], sound[0]);
                moodSound.TickDelay = ((NbtInt)m["tick_delay"]).Value;
                moodSound.Offset = ((NbtDouble)m["offset"]).Value;
                moodSound.BlockSearchExtent = ((NbtInt)m["block_search_extent"]).Value;
                effects.MoodSound = moodSound;
            }

            if (e.ContainsKey("particle"))
            {
                BiomeParticle particle = new BiomeParticle();
                NbtCompound p = (NbtCompound)e["particle"];
                NbtCompound o = (NbtCompound)p["options"];
                particle.Probability = ((NbtFloat)p["probability"]).Value;
                particle.Type = ((NbtString)o["type"]).Value;
                effects.Particle = particle;
            }

            Effects = effects;
        }
    }
}
