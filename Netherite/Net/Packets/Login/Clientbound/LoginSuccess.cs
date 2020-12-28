using System;

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
    }


}
