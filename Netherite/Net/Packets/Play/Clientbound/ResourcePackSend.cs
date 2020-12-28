namespace Netherite.Net.Packets.Play.Clientbound
{
    public class ResourcePackSend : Packet
    {
        public string Url { get; set; }
        public string Hash { get; set; }

        public ResourcePackSend(string url, string hash) : base()
        {
            Url = url;
            Hash = hash;
        }
    }
}
