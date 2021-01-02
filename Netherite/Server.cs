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

namespace Netherite
{
    public class Server
    {
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

            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

            Registry = new Registry();

            NetworkManager = new NetworkManager(this, Port);

            Worlds.Add(new World("World"));

            AppDomain.CurrentDomain.ProcessExit += (o, e) =>
            {
                this.Stop();
            };
        }

        public List<Player> OnlinePlayers
        {
            get
            {
                var connections = NetworkManager.Connections.ToList();
                connections.RemoveAll(c => !c.Connected);

                var result = connections.ConvertAll(a => a.Player);
                result.RemoveAll(a => a == null);
                return result;
            }
        }

        public Player GetPlayer(string name) => OnlinePlayers.Find(p => p.Name == name);

        public Player GetPlayer(Guid guid) => OnlinePlayers.Find(p => p.Guid == guid);

        internal async Task BroadcastPacket(Packet packet)
        {
            List<Task> tasks = new List<Task>();
            foreach (var conn in NetworkManager.Connections)
            {
                tasks.Add(conn.SendPacketAsync(packet));
            }
            await Task.WhenAll(tasks);
        }

        public void Start()
        {
            NetworkManager.StartLoop();
        }

        public void Stop()
        {
            string content = JsonConvert.SerializeObject(Config);
            File.WriteAllText("config.json", content);
        }
    }
}
