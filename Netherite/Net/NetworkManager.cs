﻿using Netherite.Net.Protocols;
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
        private int Port { get; set; }

        private CancellationTokenSource cts = new CancellationTokenSource();

        public ICollection<PlayerConnection> Connections { get; } = new List<PlayerConnection>();

        public Server Server { get; set; }

        internal NetworkManager(Server server, int port)
        {
            Server = server;
            Port = port;

            // Create a new socket and listen it.
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(0);

            Protocol.EnsureLoad();
            _ = ProtocolLoader.LoadProtocolsAsync();
        }
        
        public void Stop()
        {
            StopAccepting();
            foreach(var p in Server.OnlinePlayers)
            {
                p.Client.DisconnectAsync("Server stopping");
            }
        }

        private void StopAccepting()
        {
            cts.Cancel();
        }

        public void StartLoop()
        {
            Task.Run(async () => {
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
