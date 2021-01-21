using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Net;
using Netherite.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Netherite.Bootstrap
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Info("Starting Netherite server...");
            Server server = new Server();
            server.Start();

            Task.Run(() =>
            {
                while (true)
                {
                    server.DispatchCommand(Console.ReadLine());
                }
            });

            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            {
                string[] lines = e.ExceptionObject.ToString().Split('\n');
                foreach(string line in lines)
                {
                    Logger.Error(line);
                }

                Environment.Exit(1);
            };

            Task.WaitAll(new Task[] { server.WaitForStopAsync() });
            Logger.Info("Bye!");
        }
    }
}
