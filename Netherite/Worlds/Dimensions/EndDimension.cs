using Netherite.Data.Entities;

namespace Netherite.Worlds.Dimensions
{
    public class EndDimension : Dimension
    {
        public override bool PiglinSafe => false;
        public override bool Natural => false;
        public override float AmbientLight => 0.0f;
        public override Identifier Infiniburn => new Identifier("infiniburn_end");
        public override bool RespawnAnchorWorks => false;
        public override bool HasSkylight => false;
        public override bool BedWorks => false;
        public override Identifier Effects => new Identifier("the_end");
        public override long? FixedTime => 6000;
        public override bool HasRaids => false;
        public override int LogicalHeight => 256;
        public override double CoordinateScale => 1.0;
        public override bool Ultrawarm => false;
        public override bool HasCeiling => false;

        // 1.17
        public override int Height => 256;
        public override int MinY => 0;

        internal EndDimension() : base(new Identifier("the_end")) { }
    }
}
