using Netherite.Converters.Nbt;
using Netherite.Data.Entities;
using Netherite.Nbt.Serializations;
using Netherite.Nbt.Serializations.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Data.Nbt
{
    public class NbtPotionEffect
    {
        public byte Id { get; set; }
        public byte Amplifier { get; set; }
        public int Duration { get; set; }
        public bool ShowParticles { get; set; }
        public bool ShowIcon { get; set; }
    }

    public class NbtItem
    {
        public byte? Slot { get; set; }

        [NbtConverter(typeof(IdentifierConverter))]
        [NbtProperty("id")]
        public Identifier Id { get; set; }

        [NbtProperty("tag")]
        public ITagData Tag { get; set; }
    }

    public class NbtExplosion
    {
        public bool Flicker { get; set; }

        public bool Trail { get; set; }

        public byte Type { get; set; }

        public int[] Colors { get; set; }

        public int[] FadeColors { get; set; }
    }

    public class NbtFirework
    {
        public byte Flight { get; set; }

        public List<NbtExplosion> Explosions { get; set; }
    }

    public class NbtGameProfile
    {
        [NbtConverter(typeof(NbtUuid_Int32Arr))]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public class Property
        {
            public class Texture
            {
                public string Signature { get; set; }

                public string Value { get; set; }
            }
        }

        public List<Property> Properties { get; set; }
    }

    public class NbtEnchantment
    {
        [NbtProperty("id")] public string Id { get; set; }

        [NbtProperty("lvl")] public short Level { get; set; }
    }
}
