using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Net;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Netherite.Bootstrap
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Server server = new Server();
            server.Start();

            await Task.Delay(-1);
        }
    }
}
