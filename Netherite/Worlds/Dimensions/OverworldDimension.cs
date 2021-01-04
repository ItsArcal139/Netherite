using Netherite.Data.Entities;

namespace Netherite.Worlds.Dimensions
{
    public class OverworldDimension : Dimension
    {
        public override bool PiglinSafe => false;
        public override bool Natural => true;
        public override float AmbientLight => 0.0f;
        public override Identifier Infiniburn => new Identifier("infiniburn_overworld");
        public override bool RespawnAnchorWorks => false;
        public override bool HasSkylight => true;
        public override bool BedWorks => true;
        public override Identifier Effects => new Identifier("overworld");
        public override bool HasRaids => true;
        public override int LogicalHeight => 256;
        public override double CoordinateScale => 1.0;
        public override bool Ultrawarm => false;
        public override bool HasCeiling => false;

        // 1.17
        public override int Height => 256;
        public override int MinY => 0;

        internal OverworldDimension() : base(new Identifier("overworld")) { }
    }
}
