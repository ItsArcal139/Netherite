using Netherite.Data.Entities;
using Netherite.Nbt;
using Netherite.Texts;
using Netherite.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Netherite.Worlds.Biomes
{
    public abstract class Biome
    {
        public Identifier Name { get; protected set; }

        public int Id { get; private set; }

        private static int counter = 0;

        private static List<Biome> registry = new List<Biome>();

        public static NbtCompound GetCodecs()
        {
            NbtCompound result = new NbtCompound();
            result.Name = "";
            result.Add("type", new NbtString("minecraft:worldgen/biome"));

            NbtList list = new NbtList(NbtTag.TagType.Compound);
            foreach (var biome in registry)
            {
                list.Add(biome.GetCodec());
            }

            result.Add("value", list);
            return result;
        }

        public virtual string Category { get; protected set; }

        public virtual float Depth { get; protected set; }

        public virtual float Downfall { get; protected set; }

        public virtual Precipitation Precipitation { get; protected set; }

        public virtual float Scale { get; protected set; }

        public virtual float Temperature { get; protected set; }

        public virtual BiomeEffects Effects { get; protected set; }

        public NbtCompound GetCodec()
        {
            NbtCompound result = new NbtCompound();
            result.Add("name", new NbtString(Name.ToString()));
            result.Add("id", new NbtInt(Id));

            NbtCompound element = new NbtCompound();
            element.Add("category", new NbtString(Category));
            element.Add("depth", new NbtFloat(Depth));
            element.Add("downfall", new NbtFloat(Downfall));

            NbtCompound effects = new NbtCompound();
            effects.Add("sky_color", new NbtInt(Effects.SkyColor.RGB));
            effects.Add("water_fog_color", new NbtInt(Effects.WaterFogColor.RGB));
            effects.Add("fog_color", new NbtInt(Effects.FogColor.RGB));
            effects.Add("water_color", new NbtInt(Effects.WaterColor.RGB));

            if (Effects.MoodSound.HasValue)
            {
                BiomeMoodSound m = Effects.MoodSound.Value;
                NbtCompound moodSound = new NbtCompound();
                moodSound.Add("sound", new NbtString(m.Sound.ToString()));
                moodSound.Add("tick_delay", new NbtInt(m.TickDelay));
                moodSound.Add("offset", new NbtDouble(m.Offset));
                moodSound.Add("block_search_extend", new NbtInt(m.BlockSearchExtent));
                effects.Add("mood_sound", moodSound);
            }

            if (Effects.FoliageColor.HasValue)
            {
                effects.Add("foliage_color", new NbtInt(Effects.FoliageColor.Value.RGB));
            }

            if (Effects.GrassColorModifier != null)
            {
                effects.Add("grass_color_modifier", new NbtString(Effects.GrassColorModifier));
            }

            if (Effects.Music.HasValue)
            {
                BiomeEffectMusic m = Effects.Music.Value;
                NbtCompound music = new NbtCompound();
                music.Add("replace_current_music", new NbtByte(m.ReplaceCurrentMusic));
                music.Add("sound", new NbtString(m.Sound.ToString()));
                music.Add("min_delay", new NbtInt(m.MinDelay));
                music.Add("max_delay", new NbtInt(m.MaxDelay));
                effects.Add("music", music);
            }

            if (Effects.AmbientSound.HasValue)
            {
                effects.Add("ambient_sound", new NbtString(Effects.AmbientSound.Value.ToString()));
            }

            if (Effects.AdditionsSound.HasValue)
            {
                BiomeAdditionsSound m = Effects.AdditionsSound.Value;
                NbtCompound additions = new NbtCompound();
                additions.Add("sound", new NbtString(m.Sound.ToString()));
                additions.Add("tick_chance", new NbtDouble(m.TickChance));
                effects.Add("additions_sound", additions);
            }

            if (Effects.Particle.HasValue)
            {
                BiomeParticle m = Effects.Particle.Value;
                NbtCompound options = new NbtCompound();
                options.Add("type", new NbtString(m.Type));

                NbtCompound particle = new NbtCompound();
                particle.Add("probability", new NbtFloat(m.Probability));
                particle.Add("options", options);
                effects.Add("particle", particle);
            }

            element.Add("effects", effects);

            element.Add("precipitation", new NbtString(Precipitation.ToSnakeCase()));
            element.Add("scale", new NbtFloat(Scale));
            element.Add("temperature", new NbtFloat(Temperature));

            result.Add("element", element);
            return result;
        }

        protected Biome()
        {
            Id = counter++;
        }

        static Biome()
        {
            Stream s = Assembly.GetAssembly(typeof(Biome)).GetManifestResourceStream("Netherite.Resources.dimensions.nbt");
            byte[] b = new byte[s.Length];
            s.Read(b, 0, b.Length);

            byte[] r = GZipUtils.Decompress(b);
            int index = 0;
            NbtCompound c = (NbtCompound)NbtTag.Deserialize(r, ref index, true);
            NbtCompound a = (NbtCompound)c["minecraft:worldgen/biome"];
            NbtList l = (NbtList)a["value"];

            foreach(NbtTag i in l)
            {
                if(i is NbtCompound biome)
                {
                    registry.Add(new MinecraftBiome(biome));
                }
            }
        }

        public static Biome GetBiome(int id)
        {
            if (id < 0) return null;
            if (registry.Count <= id) return null;
            return registry[id];
        }
    }
}
