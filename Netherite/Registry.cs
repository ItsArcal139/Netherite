using Netherite.Nbt;
using Netherite.Worlds.Dimensions;
using Netherite.Worlds.Biomes;
using Netherite.Nbt.Serializations;
using System.IO;
using Netherite.Utils;

namespace Netherite
{
    public class Registry
    {
        public NbtCompound GetDimensionCodec()
        {
            NbtCompound result = new NbtCompound();
            result.Name = "";
            result.Add("minecraft:dimension_type", Dimension.GetCodecs());
            result.Add("minecraft:worldgen/biome", Biome.GetCodecs());

            byte[] b = File.ReadAllBytes("dimensions.nbt");
            byte[] r = GZipUtils.Decompress(b);
            File.WriteAllBytes("dimensions_d.nbt", r);
            int index = 0;
            return (NbtCompound)NbtTag.Deserialize(r, ref index, true);
        }
    }
}
