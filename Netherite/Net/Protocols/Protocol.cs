using Netherite.Net.IO;
using Netherite.Net.Packets;
using Netherite.Net.Packets.Handshake.Serverbound;
using Netherite.Net.Packets.Login;
using Netherite.Net.Packets.Login.Clientbound;
using Netherite.Net.Packets.Login.Serverbound;
using Netherite.Net.Packets.Status;
using Netherite.Net.Packets.Status.Clientbound;
using Netherite.Net.Packets.Status.Serverbound;
using Netherite.Texts;
using Netherite.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Netherite.Net.Protocols
{
    public abstract class Protocol
    {
        private Dictionary<int, Func<BufferReader, Packet>> handshakeInHandlers = new Dictionary<int, Func<BufferReader, Packet>>();
        private Dictionary<int, Func<BufferReader, Packet>> statusInHandlers = new Dictionary<int, Func<BufferReader, Packet>>();
        private Dictionary<int, Func<BufferReader, Packet>> loginInHandlers = new Dictionary<int, Func<BufferReader, Packet>>();
        private Dictionary<int, Func<BufferReader, Packet>> playInHandlers = new Dictionary<int, Func<BufferReader, Packet>>();

        private Dictionary<Type, Action<Packet, BufferWriter>> outHandlers = new Dictionary<Type, Action<Packet, BufferWriter>>();

        private static Dictionary<int, Protocol> protocols = new Dictionary<int, Protocol>();
        private static Dictionary<Type, int> protocolTypes = new Dictionary<Type, int>();

        static Protocol()
        {
            DefaultProtocol.EnsureLoad();
        }

        public ICollection<Protocol> LoadedProtocols => protocols.Values;

        public abstract int Version { get; }

        public abstract string VersionName { get; }

        internal static void EnsureLoad() { }

        public Packet Read(PacketState state, BufferReader reader)
        {
            int id = reader.ReadVarInt();

            Dictionary<int, Func<BufferReader, Packet>> map;
            switch (state)
            {
                case PacketState.Handshake:
                    map = handshakeInHandlers;
                    break;
                case PacketState.Status:
                    map = statusInHandlers;
                    break;
                case PacketState.Login:
                    map = loginInHandlers;
                    break;
                case PacketState.Play:
                    map = playInHandlers;
                    break;
                default:
                    throw new ArgumentException("state is unknown");
            }

            if(!map.ContainsKey(id))
            {
                Logger.Warn(
                    TranslateText.Of("Incoming packet {0} in protocol {1} not registered")
                        .AddWith(LiteralText.Of(id.ToString()).SetColor(TextColor.Gold))
                        .AddWith(Text.RepresentType(GetType())));
                return new UnknownPacket(reader.Buffer);
            }

            Func<BufferReader, Packet> handler = map[id];
            return handler(reader);
        }

        public async Task Write(Packet packet, BufferWriter writer)
        {
            Type t = packet.GetType();
            if (outHandlers.ContainsKey(t))
            {
                Action<Packet, BufferWriter> handler = outHandlers[t];
                await Task.Run(() =>
                {
                    handler(packet, writer);
                });
            }
            else
            {
                Logger.Warn(
                    TranslateText.Of("Outgoing packet {0} in protocol {1} not registered")
                        .AddWith(Text.RepresentType(packet.GetType()))
                        .AddWith(Text.RepresentType(GetType()))
                    );
            }
        }

        public static Protocol FromVersion(int version)
        {
            if (!protocols.ContainsKey(version))
            {
                throw new PlatformNotSupportedException($"Protocol version {version} not supported.");
            }
            return protocols[version];
        }

        public static int LatestVersion
        {
            get
            {
                int max = int.MinValue;
                foreach (int n in protocols.Keys)
                {
                    max = Math.Max(max, n);
                }
                return max;
            }
        }

        public static Protocol LatestProtocol => FromVersion(LatestVersion);

        public static bool HasVersion(int version) => protocols.ContainsKey(version);

        public static void Register<T>(int version, T protocol) where T : Protocol
        {
            protocols.Add(version, protocol);
            protocolTypes.Add(typeof(T), version);
            Logger.Info(
                TranslateText.Of("Registered protocol {0}")
                    .AddWith(Text.RepresentType(typeof(T))));
        }

        public static void Unregister(int version)
        {
            Protocol p = protocols[version];
            protocolTypes.Remove(p.GetType());
            protocols.Remove(version);
        }

        public static void Unregister<T>() where T : Protocol
        {
            Unregister(typeof(T));
        }

        public static void Unregister(Type t)
        {
            int v = -1;
            Protocol target = null;
            foreach (KeyValuePair<int, Protocol> p in protocols)
            {
                if (t.Equals(p.Value.GetType()))
                {
                    v = p.Key;
                    target = p.Value;
                }
            }

            if (v == -1 || target == null)
            {
                throw new ArgumentException($"{t.FullName} is not registered");
            }

            protocols.Remove(v);
            protocolTypes.Remove(t);
        }

        protected void RegisterIncoming<T>(PacketState state, int id, Func<BufferReader, T> handler) where T : Packet
        {
            Dictionary<int, Func<BufferReader, Packet>> map;
            switch (state)
            {
                case PacketState.Handshake:
                    map = handshakeInHandlers;
                    break;
                case PacketState.Status:
                    map = statusInHandlers;
                    break;
                case PacketState.Login:
                    map = loginInHandlers;
                    break;
                case PacketState.Play:
                    map = playInHandlers;
                    break;
                default:
                    throw new ArgumentException("state is unknown");
            }

            map.Add(id, handler);
        }

        public delegate void OutgoingPacketHandler<T>(T packet, BufferWriter writer) where T : Packet;

        protected void RegisterOutgoing<T>(OutgoingPacketHandler<T> handler) where T : Packet
        {
            outHandlers.Add(typeof(T), (p, writer) =>
            {
                // -- Why bother wrapping it...?
                handler((T)p, writer);
            });
        }

        protected void RegisterDefaults()
        {
            RegisterIncoming(PacketState.Handshake, 0x00, reader =>
            {
                return new SetProtocol
                {
                    ProtocolVersion = reader.ReadVarInt(),
                    ServerAddress = reader.ReadString(),
                    ServerPort = reader.ReadUShort(),
                    NextState = (PacketState)reader.ReadVarInt()
                };
            });

            RegisterIncoming(PacketState.Status, 0x00, reader =>
            {
                return new StatusRequest();
            });

            RegisterIncoming(PacketState.Status, 0x01, reader =>
            {
                return new PingPong
                {
                    Payload = reader.ReadLong()
                };
            });

            RegisterIncoming(PacketState.Login, 0x00, reader =>
            {
                return new LoginStart
                {
                    UserName = reader.ReadString()
                };
            });

            RegisterIncoming(PacketState.Login, 0x01, reader =>
            {
                return new EncryptionResponse
                {
                    SharedSecret = reader.ReadByteArray(),
                    VerifyToken = reader.ReadByteArray()
                };
            });

            RegisterOutgoing<StatusResponse>((p, writer) =>
            {
                writer.WriteString(JsonConvert.SerializeObject(p.Response));
                writer.Flush(0x00);
            });

            RegisterOutgoing<PingPong>((p, writer) =>
            {
                writer.WriteLong(p.Payload);
                writer.Flush(0x01);
            });

            RegisterOutgoing<LoginKick>((p, writer) =>
            {
                writer.WriteChat(p.Reason);
                writer.Flush(0x00);
            });
        }
    }
}
