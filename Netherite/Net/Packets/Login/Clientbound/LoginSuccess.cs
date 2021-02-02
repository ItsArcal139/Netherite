using System;
using System.Threading.Tasks;

namespace Netherite.Net.Packets.Login.Clientbound
{
    public class LoginSuccess : Packet
    {
        public Guid Guid { get; set; }

        public string UserName { get; set; }

        public LoginSuccess() : base() { }

        public LoginSuccess(Guid guid, string userName) : base()
        {
            Guid = guid;
            UserName = userName;
        }

        public override async Task ClientHandleAsync(ServerConnection connection)
        {
            connection.CurrentState = PacketState.Play;
            await Task.CompletedTask;
        }
    }
}
