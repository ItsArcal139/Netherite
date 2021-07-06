using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Netherite.Net;
using Netherite.Net.Packets;
using Netherite.Net.Packets.Play.Serverbound;
using Netherite.Net.Protocols;
using Netherite.Texts;
using Netherite.Utils;

namespace Netherite
{
    public class Client
    {
        public static Client Instance { get; private set; }

        public ClientNetworkManager NetworkManager { get; private set; }

        public Protocol TargetProtocol { get; set; }

        public Client(string address, int port)
        {
            Instance = this;
            NetworkManager = new ClientNetworkManager(address, port);

            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                Stop();
            };

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                Stop();
            };
        }

        public void Start()
        {
            NetworkManager.Start();
        }

        public void SendChat(string message)
        {
            if (NetworkManager.Connection.CurrentState == PacketState.Play)
            {
                _ = NetworkManager.Connection.SendPacketAsync(new PlayerChat
                {
                    Message = message
                });
            }
        }

        private bool stopped;

        public void Stop()
        {
            if (!stopped)
            {
                Logger.Info(LiteralText.Of("Stopping Netherite..."));
                NetworkManager.Stop();
                stopped = true;
            }
        }

        public async Task WaitForStopAsync()
        {
            while(NetworkManager.Connection.Connected)
            {
                await Task.Delay(10);
            }
        }
    }
}
