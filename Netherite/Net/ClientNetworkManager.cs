using Netherite.Net.Protocols;
using Netherite.Texts;
using Netherite.Utils;
using System;
using System.Net;
using System.Net.Sockets;

namespace Netherite.Net
{
    public class ClientNetworkManager
    {
        private Socket socket;

        public string Address { get; private set; }

        public int Port { get; private set; }

        public ServerConnection Connection { get; private set; }

        public bool Connected { get; private set; }

        public ClientNetworkManager(string address, int port)
        {
            Address = address;
            Port = port;

            try
            {
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    LiteralText.Of("Error whilist initiating Netherite Client: ")
                        .AddExtra(LiteralText.Of(ex.Message).SetColor(TextColor.Red)));
                Logger.Error(LiteralText.Of("Netherite Client will now quit."));
                Environment.Exit(0);
            }

            Protocol.EnsureLoad();
            ProtocolLoader.LoadProtocols();
        }

        public void Start()
        {
            try
            {
                socket.Connect(new DnsEndPoint(Address, Port));
                Connection = new ServerConnection(socket);
                Connection.StartLoop();
                _ = Connection.LoginAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(
                    LiteralText.Of("Error whilist connecting to the server: ")
                        .AddExtra(LiteralText.Of(ex.Message).SetColor(TextColor.Red)));
                Logger.Error(LiteralText.Of("Netherite Client will now quit."));
                Environment.Exit(0);
            }
        }

        private bool stopped = false;
        public void Stop()
        {
            if (!stopped)
            {
                stopped = true;
                _ = Connection.DisconnectAsync();
                socket.Disconnect(false);
                socket.Dispose();
            }
        }
    }
}
