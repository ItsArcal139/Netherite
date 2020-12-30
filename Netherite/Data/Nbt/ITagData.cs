using Netherite.Converters.Nbt;
using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Nbt.Serializations.Converters;
using System.Collections.Generic;

namespace Netherite.Data.Nbt
{
    public interface ITagData
    {
        public int Damage { get; set; }

        public bool Unbreakable { get; set; }

        public List<string> CanDestroy { get; set; }

        public int CustomModelData { get; set; }

        public List<NbtEnchantment> Enchantments { get; set; }

        public List<NbtEnchantment> StoredEnchantments { get; set; }

        public int RepairCost { get; set; }

        public class DisplayData
        {
            [NbtConverter(typeof(ColorConverter))]
            public Color Color { get; set; }

            public string Name { get; set; }

            public List<string> Lore { get; set; }

            public int MapColor { get; set; }
        }

        [NbtProperty("display")]
        public DisplayData Display { get; set; }
    }

    public interface IBlockTagData : ITagData
    {
        public List<string> CanPlaceOn { get; set; }

        public NbtCompound BlockEntityTag { get; set; }

        public NbtCompound BlockStateTag { get; set; }
    }

    public interface IPotionTagData : ITagData
    {
        public List<NbtPotionEffect> CustomPotionEffects { get; set; }

        public string Potion { get; set; }

        [NbtConverter(typeof(ColorConverter))]
        public Color CustomPotionColor { get; set; }
    }

    public interface ICrossbowTagData : ITagData
    {
        public bool Charged { get; set; }

        public List<NbtItem> ChargedProjectiles { get; set; }
    }

    public interface IBookTagData : ITagData
    {
        [NbtProperty("resolved")]
        public bool Resolved { get; set; }

        [NbtProperty("generation")]
        public int Generation { get; set; }

        [NbtProperty("author")]
        public string Author { get; set; }

        [NbtProperty("title")]
        public string Title { get; set; }

        [NbtProperty("pages")]
        public List<string> Pages { get; set; }
    }

    public interface ISkullTagData : ITagData
    {
        [NbtProperty("SkullOwner")]
        public string SkullOwnerString { get; set; }

        public NbtGameProfile SkullOwner { get; set; }
    }

    public interface IExplotionTagData : ITagData
    {
        public NbtExplosion Explosion { get; set; }
    }

    public interface IFireworkTagData : ITagData
    {
        public NbtFirework Fireworks { get; set; }
    }

    public interface ISpawnEntityTagData : ITagData
    {
        public NbtCompound EntityTag { get; set; }
    }

    public interface IFishBucketTagData : ITagData, ISpawnEntityTagData
    {
        public int BucketVariantTag { get; set; }
    }

    public interface IMapTagData : ITagData
    {
        [NbtProperty("map")]
        public int Map { get; set; }

        [NbtProperty("map_scale_direction")]
        public int MapScaleDirection { get; set; }

        public class Decoration
        {
            [NbtProperty("id")]
            public string Id { get; set; }

            [NbtProperty("type")]
            public byte Type { get; set; }

            [NbtProperty("x")]
            public double X { get; set; }

            [NbtProperty("z")]
            public double Z { get; set; }

            [NbtProperty("rot")]
            public double Rotation { get; set; }
        }
    }

    public interface ISuspiciousStewTagData : ITagData
    {
        public class Effect
        {
            public byte EffectId { get; set; }
            public int EffectDuration { get; set; }
        }

        public List<Effect> Effects { get; set; }
    }

    public interface IDebugStickTagData : ITagData
    {
        public NbtCompound DebugProperty { get; set; }
    }

    public interface ICompassTagData : ITagData
    {
        public byte LodestoneTracked { get; set; }

        public string LodestoneDimension { get; set; }

        public class Vector3I
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
        }

        public Vector3I LodestonePos { get; set; }
    }

    public interface IBundleTagData : ITagData
    {
        public List<NbtItem> Items { get; set; }
    }
}
