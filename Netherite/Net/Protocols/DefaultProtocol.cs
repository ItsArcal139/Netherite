namespace Netherite.Net.Protocols
{
    public class FallbackProtocol : Protocol
    {
        public override int Version => -1;

        public override string VersionName => "Fallback";

        static FallbackProtocol()
        {
            Register(-1, new FallbackProtocol());
        }

        internal FallbackProtocol()
        {
            RegisterDefaults();
        }

        internal new static void EnsureLoad() { }
    }
}
