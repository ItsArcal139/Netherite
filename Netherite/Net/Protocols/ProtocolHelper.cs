using Netherite.Nbt;

namespace Netherite.Net.Protocols
{
    public static class ProtocolHelper
    {
        private static NbtCompound CreateOverworldEntry()
        {
            NbtCompound tag = new NbtCompound();
            tag.Add("name", new NbtString("minecraft:overworld"));
            tag.Add("id", new NbtInt(0));

            NbtCompound element = new NbtCompound();
            element.Add("has_ceiling", new NbtByte(0));
            AddSharedOverworldEntries(element);

            tag.Add("element", element);
            return tag;
        }

        private static NbtCompound CreateOverworldCavesEntry()
        {
            NbtCompound tag = new NbtCompound();
            tag.Add("name", new NbtString("minecraft:overworld_caves"));
            tag.Add("id", new NbtInt(1));

            NbtCompound element = new NbtCompound();
            element.Add("has_ceiling", new NbtByte(1));
            AddSharedOverworldEntries(element);

            tag.Add("element", element);
            return tag;
        }

        private static void AddSharedOverworldEntries(NbtCompound tag)
        {
            tag.Add("piglin_safe", new NbtByte(0));
            tag.Add("natural", new NbtByte(1));
            tag.Add("ambient_light", new NbtFloat(0));
            tag.Add("infiniburn", new NbtString("minecraft:infiniburn_overworld"));
            tag.Add("respawn_anchor_works", new NbtByte(0));
            tag.Add("has_skylight", new NbtByte(1));
            tag.Add("bed_works", new NbtByte(1));
            tag.Add("has_raids", new NbtByte(1));
            tag.Add("logical_height", new NbtInt(256));
            tag.Add("shrunk", new NbtByte(0));
            tag.Add("ultrawarm", new NbtByte(0));
            tag.Add("coordinate_scale", new NbtDouble(1.0));
        }

        private static NbtCompound CreateNetherEntry()
        {
            NbtCompound tag = new NbtCompound();
            tag.Add("name", new NbtString("minecraft:the_nether"));
            tag.Add("id", new NbtInt(2));

            NbtCompound element = new NbtCompound();
            element.Add("has_ceiling", new NbtByte(1));
            element.Add("piglin_safe", new NbtByte(1));
            element.Add("natural", new NbtByte(0));
            element.Add("ambient_light", new NbtFloat(0.1f));
            element.Add("infiniburn", new NbtString("minecraft:infiniburn_nether"));
            element.Add("respawn_anchor_works", new NbtByte(1));
            element.Add("has_skylight", new NbtByte(0));
            element.Add("bed_works", new NbtByte(0));
            element.Add("fixed_time", new NbtLong(18000));
            element.Add("has_raids", new NbtByte(0));
            element.Add("logical_height", new NbtInt(128));
            element.Add("shrunk", new NbtByte(1));
            element.Add("ultrawarm", new NbtByte(1));
            element.Add("coordinate_scale", new NbtDouble(8.0));

            tag.Add("element", element);
            return tag;
        }

        private static NbtCompound CreateEndEntry()
        {
            NbtCompound tag = new NbtCompound();
            tag.Add("name", new NbtString("minecraft:the_end"));
            tag.Add("id", new NbtInt(3));

            NbtCompound element = new NbtCompound();
            element.Add("has_ceiling", new NbtByte(0));
            element.Add("piglin_safe", new NbtByte(0));
            element.Add("natural", new NbtByte(0));
            element.Add("ambient_light", new NbtFloat(0));
            element.Add("infiniburn", new NbtString("minecraft:infiniburn_end"));
            element.Add("respawn_anchor_works", new NbtByte(0));
            element.Add("has_skylight", new NbtByte(0));
            element.Add("bed_works", new NbtByte(0));
            element.Add("fixed_time", new NbtLong(6000));
            element.Add("has_raids", new NbtByte(1)); // ?
            element.Add("logical_height", new NbtInt(256));
            element.Add("shrunk", new NbtByte(0));
            element.Add("ultrawarm", new NbtByte(0));
            element.Add("coordinate_scale", new NbtDouble(1.0));

            tag.Add("element", element);
            return tag;
        }

        private static NbtCompound CreateDimensionsTag()
        {
            NbtList list = new NbtList(NbtTag.TagType.Compound);
            list.Add(CreateOverworldEntry());
            list.Add(CreateOverworldCavesEntry());
            list.Add(CreateNetherEntry());
            list.Add(CreateEndEntry());

            string dim = "minecraft:dimension_type";
            NbtCompound c = new NbtCompound();
            c.Add("type", new NbtString(dim));
            c.Add("value", list);

            NbtCompound result = new NbtCompound();
            result.Name = "";
            result.Add(dim, c);
            return result;
        }

        public static readonly NbtCompound DimensionsTag = CreateDimensionsTag();
    }
}
