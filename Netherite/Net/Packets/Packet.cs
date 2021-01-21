using Netherite.Entities;
using Netherite.Texts;
using Netherite.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Netherite.Net.Packets
{
    public abstract class Packet
    {
        protected Packet() { }

        // Call this method to make sure the static constructor is executed
        internal static void EnsureLoaded() { }

        public virtual bool IsConstantPacket => false;

        public virtual Task HandleAsync(Server server, Player player) => Task.CompletedTask;
    }

    internal class UnknownPacket : Packet
    {
        internal UnknownPacket(byte[] buffer) : base()
        {
            var text = LiteralText.Of("Packet: ");
            foreach (byte b in buffer)
            {
                text.AddExtra(
                    LiteralText.Of($"{b:x2} ")
                );
            }
            Logger.Verbose(text);
        }
    }

    public enum PacketState
    {
        Handshake, Status, Login, Play
    }
}
