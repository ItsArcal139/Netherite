using Netherite.Texts;
using Newtonsoft.Json;
using System;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class ChatPacket : Packet
    {
        public Text Message { get; private set; }

        public enum ChatPosition: byte
        {
            Chat, System, GameInfo
        }

        public ChatPosition Position { get; private set; }

        public Guid SenderGuid { get; private set; }

        public ChatPacket() : base() { }

        public ChatPacket(Text message, ChatPosition pos, Guid sender) : base()
        {
            Message = message;
            Position = pos;
            SenderGuid = sender;
        }
    }

    public class DeclareRecipes : Packet
    {

    }
}
