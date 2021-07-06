using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Net;
using Netherite.Utils;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Netherite.Bootstrap
{
    class Program
    {
        static void RunClient()
        {
            Logger.Info("Starting Netherite Client...");
            Logger.Warn("*** Client features are experimental! ***");
            Logger.Warn("Things may not work since it's pre-alpha!");

            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            {
                string[] lines = e.ExceptionObject.ToString().Split('\n');
                foreach (string line in lines)
                {
                    Logger.Error(line);
                }

                Environment.Exit(1);
            };

            Client client = new Client("172.20.10.3", 25566);
            client.Start();

            Task.Run(() => {
                while(true)
                {
                    client.SendChat(Console.ReadLine());
                }
            });

            Task.WaitAll(new Task[] { client.WaitForStopAsync() });
            Logger.Info("Bye!");
        }

        static void Main(string[] args)
        {
            if(OSHelper.IsRosetta2)
            {
                Logger.Warn("*** Netherite is being translated by Rosetta 2 ***");
                Logger.Warn("The .NET debugger currently might not work perfectly on this environment!");
                Logger.Warn("The process might exit with error: bad instruction 4.");
                Logger.Warn("Using the command line is encouraged.");
                Logger.Warn("---");
            } else if(OSHelper.IsMacOS && OSHelper.IsArm64)
            {
                Logger.Warn("*** Netherite is being run on Apple Silicon ***");
                Logger.Warn("This is still in beta, some error might appear.");
                Logger.Warn("---");
            }

            if (args.Length > 0 && args[0] == "--client")
            {
                RunClient();
                return;
            }

            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            {
                Logger.Error("An unhandled exception occurred. Netherite will quit.");
                string[] lines = e.ExceptionObject.ToString()?.Split('\n') ?? new string[0];
                foreach (string line in lines)
                {
                    Logger.Verbose(line);
                }

                Environment.Exit(1);
            };

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

            Task.WaitAll(new Task[] { server.WaitForStopAsync() });
            Logger.Info("Bye!");
        }
    }
}
