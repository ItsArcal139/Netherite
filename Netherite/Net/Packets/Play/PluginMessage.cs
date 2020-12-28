using Netherite.Entities;
using Netherite.Net.IO;
using Netherite.Utils;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Netherite.Net.Packets.Play
{
    public class PluginMessage : Packet
    {
        public string Channel { get; set; }

        public byte[] Data { get; set; }

        public override Task HandleAsync(Server server, Player player)
        {
            Logger.Log($"Plugin channel: \u00a76{Channel}: \u00a7a" + Encoding.UTF8.GetString(Data));
            Logger.Log($"Hex: " + DebugHelper.HexDump(Data));
            return Task.CompletedTask;
        }
    }
}
