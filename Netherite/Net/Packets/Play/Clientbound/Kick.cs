using System.Threading.Tasks;
using Netherite.Texts;
using Netherite.Utils;

namespace Netherite.Net.Packets
{
    public class Kick : Packet
    {
        public Text Reason { get; set; }

        public Kick(Text reason) : base()
        {
            Reason = reason;
        }

        public override async Task ClientHandleAsync(ServerConnection connection)
        {
            Logger.Info(LiteralText.Of("Kicked by the server: ").AddExtra(Reason));
            await Task.CompletedTask;
        }
    }
}
