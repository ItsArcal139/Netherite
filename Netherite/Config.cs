namespace Netherite
{
    public class Config
    {
        public const string FilePath = "config.json";

        public int Version { get; set; }

        public bool OnlineMode { get; set; } = true;

        public bool DebugPacket { get; set; } = false;

        public int Port { get; set; } = 25565;
    }
}
