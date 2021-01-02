using Netherite.Data.Entities;

namespace Netherite.Worlds.Dimensions
{
    public class NetherDimension : Dimension
    {
        public override bool PiglinSafe => true;
        public override bool Natural => false;
        public override float AmbientLight => 0.1f;
        public override Identifier Infiniburn => new Identifier("infiniburn_nether");
        public override bool RespawnAnchorWorks => true;
        public override bool HasSkylight => false;
        public override bool BedWorks => false;
        public override Identifier Effects => new Identifier("the_nether");
        public override long? FixedTime => 18000;
        public override bool HasRaids => false;
        public override int LogicalHeight => 128;
        public override double CoordinateScale => 8.0;
        public override bool Ultrawarm => true;
        public override bool HasCeiling => true;

        internal NetherDimension() : base(new Identifier("the_nether")) { }
    }
}
