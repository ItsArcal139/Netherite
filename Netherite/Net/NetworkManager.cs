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

        internal NetworkManager(Server server, int port = 25565)
        {
            Server = server;
            Port = port;

            // Create a new socket and listen it.
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(0);

            Protocol.EnsureLoad();
            ProtocolLoader.LoadDLLs();
        }

        public void StopAccepting()
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
