using Netherite.Entities;
using Netherite.Net.IO;
using Netherite.Texts;
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
            Logger.Log(
                TranslateText.Of("Plugin channel: {0}: {1}")
                    .AddWith(LiteralText.Of(Channel).SetColor(TextColor.Gold))
                    .AddWith(LiteralText.Of(Encoding.UTF8.GetString(Data)).SetColor(TextColor.DarkGray))
                );
            Logger.Log(
                TranslateText.Of("Hex: {0}")
                    .AddWith(LiteralText.Of(DebugHelper.HexDump(Data)).SetColor(TextColor.DarkGray))
                );
            return Task.CompletedTask;
        }
    }
}
