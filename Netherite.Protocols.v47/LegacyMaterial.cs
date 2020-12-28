using System;
using System.Collections.Generic;

namespace Netherite.Protocols.v47
{
    public class LegacyMaterial
    {
        private static Dictionary<Material, LegacyMaterial> map = new Dictionary<Material, LegacyMaterial>();

        public static readonly LegacyMaterial Air = new LegacyMaterial(Material.Air, 0);
        public static readonly LegacyMaterial Stone = new LegacyMaterial(Material.Stone, 1);
        public static readonly LegacyMaterial Granite = new LegacyMaterial(Material.Granite, 1, 1);
        public static readonly LegacyMaterial PolishedGranite = new LegacyMaterial(Material.PolishedGranite, 1, 2);
        public static readonly LegacyMaterial Diorite = new LegacyMaterial(Material.Diorite, 1, 3);
        public static readonly LegacyMaterial PolishedDiorite = new LegacyMaterial(Material.PolishedDiorite, 1, 4);
        public static readonly LegacyMaterial Andesite = new LegacyMaterial(Material.Andesite, 1, 5);
        public static readonly LegacyMaterial PolishedAndesite = new LegacyMaterial(Material.PolishedAndesite, 1, 6);
        public static readonly LegacyMaterial GrassBlock = new LegacyMaterial(Material.GrassBlock, 2);
        public static readonly LegacyMaterial Dirt = new LegacyMaterial(Material.Dirt, 3);
        public static readonly LegacyMaterial CoarseDirt = new LegacyMaterial(Material.CoarseDirt, 3, 1);
        public static readonly LegacyMaterial Podzol = new LegacyMaterial(Material.Podzol, 3, 2);
        public static readonly LegacyMaterial Cobblestone = new LegacyMaterial(Material.Cobblestone, 4);
        public static readonly LegacyMaterial OakPlanks = new LegacyMaterial(Material.OakPlanks, 5);
        public static readonly LegacyMaterial SprucePlanks = new LegacyMaterial(Material.SprucePlanks, 5, 1);
        public static readonly LegacyMaterial BirchPlanks = new LegacyMaterial(Material.BirchPlanks, 5, 2);
        public static readonly LegacyMaterial JunglePlanks = new LegacyMaterial(Material.JunglePlanks, 5, 3);
        public static readonly LegacyMaterial AcaciaPlanks = new LegacyMaterial(Material.AcaciaPlanks, 5, 4);
        public static readonly LegacyMaterial DarkOakPlanks = new LegacyMaterial(Material.DarkOakPlanks, 5, 5);
        public static readonly LegacyMaterial OakLog = new LegacyMaterial(Material.OakLog, 17);
        public static readonly LegacyMaterial SpruceLog = new LegacyMaterial(Material.SpruceLog, 17, 1);
        public static readonly LegacyMaterial BirchLog = new LegacyMaterial(Material.BirchLog, 17, 2);
        public static readonly LegacyMaterial JungleLog = new LegacyMaterial(Material.JungleLog, 17, 3);
        public static readonly LegacyMaterial AcaciaLog = new LegacyMaterial(Material.AcaciaLog, 162);
        public static readonly LegacyMaterial DarkOakLog = new LegacyMaterial(Material.DarkOakLog, 162, 1);

        public Material Material { get; set; }

        public short Id { get; set; }

        public byte Data { get; set; }

        private LegacyMaterial(Material modern, short id, byte data = 0)
        {
            Material = modern;
            Id = id;
            Data = data;
            map.Add(modern, this);
        }

        public static LegacyMaterial FromMaterial(Material m) => map[m];

        public static void EnsureLoad() { }
    }
}
