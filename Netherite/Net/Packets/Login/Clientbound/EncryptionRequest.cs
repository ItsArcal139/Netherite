using System.Threading.Tasks;

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

        public override async Task ClientHandleAsync(ServerConnection connection)
        {
            // For now we just disconnect from the server,
            // because we have currently no access to the access token
            await connection.DisconnectAsync();
        }
    }
}
