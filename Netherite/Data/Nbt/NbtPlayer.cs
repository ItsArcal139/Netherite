using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Nbt.Serializations.Converters;
using Netherite.Worlds;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Data.Nbt
{
    public class NbtGameModeConverter<T, V> : NbtConverter where T : INbtValue<V>
    {
        public override object FromNbt(NbtTag tag)
        {
            if (!(tag is T i))
            {
                throw new ArgumentException($"Excepted a {typeof(T)}, found {tag.GetType()} when converting to GameMode");
            }
            return (GameMode)(i.Value as byte?);
        }

        public override NbtTag ToNbt(object obj)
        {
            return new NbtInt((int)(GameMode)obj);
        }
    }

    public class NbtUuid_Int32Arr : NbtConverter
    {
        public override object FromNbt(NbtTag tag)
        {
            if (!(tag is NbtIntArray ia))
            {
                throw new ArgumentException($"Excepted a NbtIntArray, found {tag.GetType()} when converting int[] to Guid");
            }
            int[] array = ia.Value;

            // Perform my magic here
            byte[] raw = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                int val = array[i];
                byte[] bs = BitConverter.GetBytes(val);
                Array.Copy(bs, 0, raw, i * 4, 4);
            }

            Array.Reverse(raw, 0, 4);
            Array.Reverse(raw, 4, 2);
            Array.Reverse(raw, 6, 2);

            string uuid = "";
            for (int i = 0; i < 16; i++)
            {
                uuid += string.Format("{0:x2}", raw[i]);
                if (i == 3 || i == 5 || i == 7 || i == 9)
                {
                    uuid += "-";
                }
            }

            return Guid.Parse(uuid);
        }

        public override NbtTag ToNbt(object obj)
        {
            if (!(obj is Guid g))
            {
                throw new ArgumentException($"Excepted a Guid, found {obj.GetType()} when converting Guid to int[]");
            }

            byte[] raw = g.ToByteArray();

            Array.Reverse(raw, 0, 4);
            Array.Reverse(raw, 4, 2);
            Array.Reverse(raw, 6, 2);

            return new NbtIntArray(new[] {
                BitConverter.ToInt32(raw, 0),
                BitConverter.ToInt32(raw, 4),
                BitConverter.ToInt32(raw, 8),
                BitConverter.ToInt32(raw, 12)
             });
        }
    }

    public class NbtPlayer
    {
        public int DataVersion { get; set; }

        public int Dimension { get; set; }

        [NbtConverter(typeof(NbtGameModeConverter<NbtInt, int>))]
        [NbtProperty("playerGameType")]
        public GameMode Mode { get; set; }

        public int Score { get; set; }

        public int SelectedItemSlot { get; set; }

        public string SpawnDimension { get; set; }

        public int SpawnX { get; set; }

        public int SpawnY { get; set; }

        public int SpawnZ { get; set; }

        public bool SpawnForced { get; set; }

        public bool Sleeping { get; set; }

        public short SleepTimer { get; set; }

        [NbtProperty("foodLevel")]
        public int FoodLevel { get; set; }

        public float FoodExhaustionLevel { get; set; }

        public float FoodSaturationLevel { get; set; }

        public int FoodTickTimer { get; set; }

        public int XpLevel { get; set; }

        public float XpP { get; set; }

        public int XpTotal { get; set; }

        public int XpSeed { get; set; }

        public List<NbtItem> Inventory { get; set; }

        public List<NbtItem> EnderItems { get; set; }

        public class AbilityData
        {
            [NbtProperty("walkSpeed")]
            public float WalkSpeed { get; set; } = 0.1f;

            [NbtProperty("flySpeed")]
            public float FlySpeed { get; set; } = 0.05f;

            [NbtProperty("mayfly")]
            public bool MayFly { get; set; }

            [NbtProperty("flying")]
            public bool Flying { get; set; }

            [NbtProperty("invulnerable")]
            public bool Invulnerable { get; set; }

            [NbtProperty("mayBuild")]
            public bool MayBuild { get; set; }

            [NbtProperty("instabuild")]
            public bool InstaBuild { get; set; }
        }

        [NbtProperty("abilities")]
        public AbilityData Abilities { get; set; }

        public class PosCompound
        {
            [NbtProperty("x")]
            public double X { get; set; }

            [NbtProperty("y")]
            public double Y { get; set; }

            [NbtProperty("z")]
            public double Z { get; set; }
        }

        [NbtProperty("enteredNetherPosition")]
        public PosCompound EnteredNetherPos { get; set; }

        public class Vehicle
        {
            [NbtConverter(typeof(NbtUuid_Int32Arr))]
            public Guid Attach { get; set; }

            public NbtCompound Entity { get; set; }
        }

        public NbtCompound ShoulderEntityLeft { get; set; }

        public NbtCompound ShoulderEntityRight { get; set; }

        [NbtProperty("seenCredits")]
        public bool SeenCredits { get; set; }

        public class RecipeBookData
        {
            [NbtProperty("recipes")]
            public List<string> Recipes { get; set; }

            [NbtProperty("toBeDisplayed")]
            public List<string> ToBeDisplayed { get; set; }

            [NbtProperty("isFilteringCraftable")]
            public bool IsFilteringCraftable { get; set; }

            [NbtProperty("isGuiOpen")]
            public bool IsGuiOpen { get; set; }

            [NbtProperty("isFurnaceFilteringCraftable")]
            public bool IsFurnaceFilteringCraftable { get; set; }

            [NbtProperty("isFurnaceGuiOpen")]
            public bool IsFurnaceGuiOpen { get; set; }
        }
    }
}
