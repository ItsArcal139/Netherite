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

        public virtual Task ClientHandleAsync(ServerConnection connection) => Task.CompletedTask;
    }

    internal class UnknownPacket : Packet
    {
        internal byte[] buffer;
        internal UnknownPacket(byte[] buffer) : base()
        {
            this.buffer = buffer;
        }
    }

    public enum PacketState
    {
        Handshake, Status, Login, Play
    }
}
