namespace Netherite
{
    public class Config
    {
        public const string FilePath = "config.json";

        public int Version { get; set; }

        public bool OnlineMode { get; set; }

        public bool DebugPacket { get; set; }

        public int Port { get; set; }
    }
}
