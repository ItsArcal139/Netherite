using Netherite.Data.Entities;
using Netherite.Nbt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Worlds.Dimensions
{
    public abstract class Dimension
    {
        public abstract bool PiglinSafe { get; }

        public abstract bool Natural { get; }

        public abstract float AmbientLight { get; }

        public abstract Identifier Infiniburn { get; }

        public abstract bool RespawnAnchorWorks { get; }

        public abstract bool HasSkylight { get; }

        public abstract bool BedWorks { get; }

        public abstract Identifier Effects { get; }

        public virtual long? FixedTime => null;

        public abstract bool HasRaids { get; }

        public abstract int LogicalHeight { get; }

        public abstract double CoordinateScale { get; }

        public abstract bool Ultrawarm { get; }

        public abstract bool HasCeiling { get; }

        public Identifier Name { get; private set; }

        public int Id { get; private set; }

        private static int counter = 0;

        private static List<Dimension> registry = new List<Dimension>();

        protected Dimension(Identifier name)
        {
            Name = name;
            Id = counter++;

            registry.Add(this);
        }

        public static readonly OverworldDimension Overworld = new OverworldDimension();
        public static readonly OverworldCavesDimension OverworldCaves = new OverworldCavesDimension();
        public static readonly NetherDimension Nether = new NetherDimension();
        public static readonly EndDimension End = new EndDimension();

        public static NbtCompound GetCodecs()
        {
            NbtCompound result = new NbtCompound();
            result.Name = "";
            result.Add("type", new NbtString("minecraft:dimension_type"));

            NbtList list = new NbtList(NbtTag.TagType.Compound);
            foreach(var dim in registry)
            {
                list.Add(dim.GetCodec());
            }

            result.Add("value", list);
            return result;
        }

        public NbtCompound GetCodec()
        {
            NbtCompound result = new NbtCompound();
            result.Add("name", new NbtString(Name.ToString()));
            result.Add("id", new NbtInt(Id));

            NbtCompound element = new NbtCompound();
            element.Add("piglin_safe", new NbtByte(PiglinSafe));
            element.Add("natural", new NbtByte(Natural));
            element.Add("ambient_light", new NbtFloat(AmbientLight));
            element.Add("infiniburn", new NbtString(Infiniburn.ToString()));
            element.Add("respawn_anchor_works", new NbtByte(RespawnAnchorWorks));
            element.Add("has_skylight", new NbtByte(HasSkylight));
            element.Add("bed_works", new NbtByte(BedWorks));
            element.Add("effects", new NbtString(Effects.ToString()));

            if(FixedTime != null)
            {
                element.Add("fixed_time", new NbtLong(FixedTime.Value));
            }

            element.Add("has_raids", new NbtByte(HasRaids));
            element.Add("logical_height", new NbtInt(LogicalHeight));
            element.Add("coordinate_scale", new NbtDouble(CoordinateScale));
            element.Add("ultrawarm", new NbtByte(Ultrawarm));
            element.Add("has_ceiling", new NbtByte(HasCeiling));

            result.Add("element", element);
            return result;
        }
    }
}
