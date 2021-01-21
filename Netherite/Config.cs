namespace Netherite
{
    public class Config
    {
        public int Version { get; set; } = 1;

        public bool OnlineMode { get; set; } = true;

        public bool DebugPacket { get; set; } = false;

        public int Port { get; set; } = 25565;
    }
}
