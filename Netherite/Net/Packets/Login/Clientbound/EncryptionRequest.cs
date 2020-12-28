namespace Netherite.Net.Packets.Login.Clientbound
{
    public class EncryptionRequest : Packet
    {
        public string ServerID { get; set; }

        public byte[] PublicKey { get; set; }

        public byte[] VerifyToken { get; set; }

        public EncryptionRequest() : base() { }

        public EncryptionRequest(string serverId, byte[] publicKey, byte[] verifyToken) : base()
        {
            ServerID = serverId;
            PublicKey = publicKey;
            VerifyToken = verifyToken;
        }
    }
}
