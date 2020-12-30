using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using System.Collections.Generic;

namespace Netherite.Data.Nbt
{
    public class NbtBlockState
    {
        public string Name { get; set; }

        public NbtCompound Properties { get; set; }
    }

    public class NbtLevel
    {
        public int DataVersion { get; set; }

        public bool LightPopulated { get; set; }

        public int zPos { get; set; }

        public class HeightMapList
        {
            [NbtProperty("MOTION_BLOCKING")]
            public int[] MotionBlocking { get; set; }

            [NbtProperty("MOTION_BLOCKING_NO_LEAVES")]
            public int[] MotionBlocingNoLeaves { get; set; }

            [NbtProperty("OCEAN_FLOOR")]
            public int[] OceanFloor { get; set; }

            [NbtProperty("WORLD_SURFACE")]
            public int[] WorldSurface { get; set; }
        }

        public HeightMapList Heightmaps { get; set; }

        public List<NbtSection> Sections { get; set; }

        public class NbtSection
        {
            /// <summary>
            /// 4096 個方塊狀態 Palette，被編碼成 256 個 long。
            /// </summary>
            public long[] BlockStates { get; set; }

            public List<NbtBlockState> Palette { get; set; }

            public byte[] SkyLight { get; set; }

            public byte Y { get; set; }
        }

        public long LastUpdate { get; set; }

        public int[] Biomes { get; set; }

        public long InhabitedTime { get; set; }

        public int xPos { get; set; }

        public bool TerrainPopulated { get; set; }

        public List<object> TileEntities { get; set; }

        public List<object> Entities { get; set; }
    }
}
