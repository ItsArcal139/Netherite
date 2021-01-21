using Netherite.Net.Protocols;
using Netherite.Texts;
using Netherite.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Netherite.Net
{
    public class NetworkManager
    {
        private Socket socket;

        public int Port { get; private set; }

        private CancellationTokenSource cts = new CancellationTokenSource();

        public ICollection<PlayerConnection> Connections { get; } = new List<PlayerConnection>();

        public Server Server { get; set; }

        internal NetworkManager(Server server, int port)
        {
            Server = server;
            Port = port;

            // Create a new socket and listen it.
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, port));
                socket.Listen(0);
            } catch(SocketException ex)
            {
                Logger.Error(
                    LiteralText.Of("Error whilist initiating Netherite: ")
                        .AddExtra(LiteralText.Of(ex.Message).SetColor(TextColor.Red)));
                Logger.Error(LiteralText.Of("Netherite will now quit."));
                Environment.Exit(0);
            }

            Logger.Info(LiteralText.Of($"Listening on port {port}"));

            Protocol.EnsureLoad();
            ProtocolLoader.LoadProtocols();
        }

        public void Stop()
        {
            StopAccepting();
            List<Task> tasks = new List<Task>();
            foreach (var p in Server.OnlinePlayers)
            {
                tasks.Add(p.Client.DisconnectAsync(
                    TranslateText.Of("[Netherite] %s")
                        .AddWith(
                            LiteralText.Of("Server stopping.")
                        )));
            }

            Task.WaitAll(tasks.ToArray());
            socket.Close();
            socket.Dispose();
        }

        private void StopAccepting()
        {
            cts.Cancel();
        }

        public void StartLoop()
        {
            Task.Run(async () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    Socket s = await socket.AcceptAsync();
                    PlayerConnection conn = new PlayerConnection(Server, s);
                    conn.Disconnected += () =>
                    {
                        Connections.Remove(conn);
                    };
                    conn.StartLoop();
                    Connections.Add(conn);
                }
            });
        }
    }
}
