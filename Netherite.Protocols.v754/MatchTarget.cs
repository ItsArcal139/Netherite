namespace Netherite.Protocols.v754
{
    public struct MatchTarget
    {
        public short Base { get; set; }
        public short Numeric { get; set; }

        public MatchTarget(short @base, short numeric)
        {
            Base = @base;
            Numeric = numeric;
        }
    }
}
