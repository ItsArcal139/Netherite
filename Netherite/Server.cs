using Netherite.Entities;
using Netherite.Net;
using Netherite.Net.Packets;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Netherite.Worlds;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using Netherite.Texts;
using Netherite.Utils;

namespace Netherite
{
    public class Server
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public Registry Registry { get; private set; }

        public NetworkManager NetworkManager { get; internal set; }

        public static Server Instance { get; private set; }

        public List<World> Worlds { get; private set; } = new List<World>();

        public bool OnlineMode => Config.OnlineMode;

        public int Port => Config.Port;

        public Config Config { get; set; }

        public Server()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Already created a server");
            }

            Instance = this;

            if(!File.Exists(Config.FilePath))
            {
                File.WriteAllText(Config.FilePath, "{}");
            }
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Config.FilePath));

            Registry = new Registry();

            NetworkManager = new NetworkManager(this, Port);

            Worlds.Add(new World("World"));

            AppDomain.CurrentDomain.ProcessExit += (o, e) =>
            {
                Stop();
            };

            Console.CancelKeyPress += (o, e) =>
            {
                e.Cancel = true;
                Stop();
            };
        }

        public List<Player> OnlinePlayers
        {
            get
            {
                var result = NetworkManager.Connections.ToList().FindAll(c =>
                {
                    return c.Connected && c.CurrentState == PacketState.Play;
                }).ConvertAll(a => a.Player).FindAll(a => a != null);

                return result;
            }
        }

        public Player GetPlayer(string name) => OnlinePlayers.Find(p => p.Name == name);

        public Player GetPlayer(Guid guid) => OnlinePlayers.Find(p => p.Guid == guid);

        internal async Task BroadcastPacket(Packet packet)
        {
            List<Task> tasks = new List<Task>();
            foreach (var player in OnlinePlayers)
            {
                tasks.Add(player.Client.SendPacketAsync(packet));
            }
            await Task.WhenAll(tasks);
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    Tick();
                    await Task.Delay(50);
                }
            });

            NetworkManager.StartLoop();
        }

        public void Tick()
        {
            foreach (var world in Worlds)
            {
                world.Tick();
            }

            foreach (var player in OnlinePlayers)
            {
                player.Tick();
            }
        }

        public void SaveConfig()
        {
            string content = JsonConvert.SerializeObject(Config);
            File.WriteAllText(Config.FilePath, content);
        }

        private bool stopped = false;

        public void Stop()
        {
            if (!stopped)
            {
                Logger.Info(LiteralText.Of("Stopping Netherite..."));
                cts.Cancel();
                NetworkManager.Stop();
                string content = JsonConvert.SerializeObject(Config);
                File.WriteAllText("config.json", content);
                stopped = true;
            }
        }

        public async Task WaitForStopAsync()
        {
            while (!stopped)
            {
                await Task.Delay(1);
            }
        }

        public void DispatchCommand(string command)
        {
            Logger.Info("Console issued a command: " + command);
        }
    }
}
