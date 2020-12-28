using Netherite.Auth;
using Netherite.Net;
using Netherite.Net.Packets.Play.Clientbound;
using Netherite.Texts;
using Netherite.Worlds;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Entities
{
    public class Player : Entity, ICommandSender
    {
        public GameProfile Profile { get; set; }

        public PlayerConnection Client { get; set; }

        public new PlayerMetadata Metadata { get; set; } = new PlayerMetadata
        {
            Health = 20
        };

        public Player(World w, GameProfile profile, PlayerConnection client) : base(w)
        {
            Profile = profile;
            Client = client;
        }

        public new Guid Guid => Profile.Guid;

        public string Name => Profile.Name;

        public void SendMessage(Text text)
        {
            _ = Client.SendPacketAsync(new ChatPacket(text, ChatPacket.ChatPosition.System, Guid.NewGuid()));
        }

        public void SendMessage(string message)
        {
            SendMessage(LiteralText.Of(message));
        }
    }
}
