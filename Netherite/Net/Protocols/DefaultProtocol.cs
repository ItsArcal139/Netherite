namespace Netherite.Net.Protocols
{
    public class DefaultProtocol : Protocol
    {
        public override int Version => -1;

        public override string VersionName => "Pre-login";

        static DefaultProtocol()
        {
            Register(-1, new DefaultProtocol());
        }

        internal DefaultProtocol()
        {
            RegisterDefaults();
        }

        internal new static void EnsureLoad() { }
    }
}
