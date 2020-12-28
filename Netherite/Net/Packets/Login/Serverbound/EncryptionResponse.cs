namespace Netherite.Net.Packets.Login.Serverbound
{
    public class EncryptionResponse : Packet
    {
        public byte[] SharedSecret { get; set; }
        public byte[] VerifyToken { get; set; }
    }
}
