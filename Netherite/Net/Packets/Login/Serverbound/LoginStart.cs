using System;

namespace Netherite.Net.Packets.Login.Serverbound
{
    public class LoginStart : Packet
    {
        public string UserName { get; set; }
    }


}
