using Netherite.Attributes;
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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Netherite.Net.Protocols
{
    public enum ProtocolRole
    {
        Server, Client
    }

    public abstract class Protocol
    {
        private readonly Dictionary<int, Func<BufferReader, Packet>> serverHandshakeInHandlers = new();
        private readonly Dictionary<int, Func<BufferReader, Packet>> serverStatusInHandlers = new();
        private readonly Dictionary<int, Func<BufferReader, Packet>> serverLoginInHandlers = new();
        private readonly Dictionary<int, Func<BufferReader, Packet>> serverPlayInHandlers = new();
        private readonly Dictionary<Type, Action<Packet, BufferWriter>> serverOutHandlers = new();

        private readonly Dictionary<int, Func<BufferReader, Packet>> clientHandshakeInHandlers = new();
        private readonly Dictionary<int, Func<BufferReader, Packet>> clientStatusInHandlers = new();
        private readonly Dictionary<int, Func<BufferReader, Packet>> clientLoginInHandlers = new();
        private readonly Dictionary<int, Func<BufferReader, Packet>> clientPlayInHandlers = new();
        private readonly Dictionary<Type, Action<Packet, BufferWriter>> clientOutHandlers = new();

        private static readonly Dictionary<int, Protocol> Protocols = new();

        static Protocol()
        {
            FallbackProtocol.EnsureLoad();
        }

        public ICollection<Protocol> LoadedProtocols => Protocols.Values;

        public abstract int Version { get; }

        public abstract string VersionName { get; }

        internal static void EnsureLoad() { }

        public Packet Read(ProtocolRole type, PacketState state, BufferReader reader)
        {
            int id = reader.ReadVarInt();

            Dictionary<int, Func<BufferReader, Packet>> map;
            switch (state)
            {
                case PacketState.Handshake:
                    map = type == ProtocolRole.Server ? serverHandshakeInHandlers : clientHandshakeInHandlers;
                    break;
                case PacketState.Status:
                    map = type == ProtocolRole.Server ? serverStatusInHandlers : clientStatusInHandlers;
                    break;
                case PacketState.Login:
                    map = type == ProtocolRole.Server ? serverLoginInHandlers : clientLoginInHandlers;
                    break;
                case PacketState.Play:
                    map = type == ProtocolRole.Server ? serverPlayInHandlers : clientPlayInHandlers;
                    break;
                default:
                    throw new ArgumentException($"Unknown state {state}");
            }

            if (!map.ContainsKey(id))
            {
                Logger.Warn(
                    TranslateText.Of("Incoming packet {0} in protocol {1} not registered")
                        .AddWith(LiteralText.Of("0x" + id.ToString("x2")).SetColor(TextColor.Gold))
                        .AddWith(Text.RepresentType(GetType())));
                return new UnknownPacket(reader.Buffer);
            }

            Func<BufferReader, Packet> handler = map[id];
            return handler(reader);
        }

        public async Task Write(ProtocolRole type, Packet packet, BufferWriter writer)
        {
            Type t = packet.GetType();
            var handlers = type == ProtocolRole.Server ? serverOutHandlers : clientOutHandlers;
            if (handlers.ContainsKey(t))
            {
                Action<Packet, BufferWriter> handler = handlers[t];
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
            if (!Protocols.ContainsKey(version))
            {
                throw new PlatformNotSupportedException($"Protocol version {version} not supported.");
            }
            return Protocols[version];
        }

        public static int LatestVersion
        {
            get
            {
                int max = int.MinValue;
                foreach (int n in Protocols.Keys)
                {
                    if (n > 0x40000000) continue;
                    max = Math.Max(max, n);
                }
                return max;
            }
        }

        public static int LatestSnapshot
        {
            get
            {
                int max = int.MinValue;
                foreach (int n in Protocols.Keys)
                {
                    if (n <= 0x40000000) continue;
                    max = Math.Max(max, n);
                }
                return max;
            }
        }

        public bool IsSnapshot => Version > 0x40000000;

        public static Protocol LatestProtocol => FromVersion(LatestVersion);

        public static bool HasVersion(int version) => Protocols.ContainsKey(version);

        public static void Register<T>(int version, T protocol) where T : Protocol
        {
            Protocols.Add(version, protocol);
            Logger.Log(
                TranslateText.Of("Registered protocol {0}")
                    .AddWith(Text.RepresentType(typeof(T))));
        }

        public static void Unregister(int version)
        {
            Protocol p = Protocols[version];
            Protocols.Remove(version);
        }

        public static void Unregister<T>() where T : Protocol
        {
            Unregister(typeof(T));
        }

        public static void Unregister(Type t)
        {
            int v = -1;
            Protocol target = null;
            foreach (var p in Protocols.Where(p => t == p.Value.GetType()))
            {
                v = p.Key;
                target = p.Value;
            }

            if (v == -1 || target == null)
            {
                throw new ArgumentException($"{t.FullName} is not registered");
            }

            Protocols.Remove(v);
        }

        private void CheckUsingPreview(Type t)
        {
            var attr = t.GetCustomAttribute<PreReleaseAttribute>();
            if (attr != null)
            {
                Logger.Warn(
                    TranslateText.Of(
                        "Protocol %s %s registered a packet %s, which is not in a stable release of Minecraft."
                    ).AddWith(
                        LiteralText.Of(VersionName).SetColor(TextColor.Gold)
                    ).AddWith(
                        LiteralText.Of($"({Version})").SetColor(TextColor.DarkGray)
                    ).AddWith(
                        LiteralText.Of(t.Name).SetColor(TextColor.Red)
                    ));
            }
        }

        protected void RegisterIncoming<T>(ProtocolRole type, PacketState state, int id, Func<BufferReader, T> handler) where T : Packet
        {
            Type t = typeof(T);
            CheckUsingPreview(t);

            var map = state switch
            {
                PacketState.Handshake => type == ProtocolRole.Server ? serverHandshakeInHandlers : clientHandshakeInHandlers,
                PacketState.Status => type == ProtocolRole.Server ? serverStatusInHandlers : clientStatusInHandlers,
                PacketState.Login => type == ProtocolRole.Server ? serverLoginInHandlers : clientLoginInHandlers,
                PacketState.Play => type == ProtocolRole.Server ? serverPlayInHandlers : clientPlayInHandlers,
                _ => throw new ArgumentException("state is unknown")
            };

            map.Add(id, handler);
        }

        public delegate void OutgoingPacketHandler<T>(T packet, BufferWriter writer) where T : Packet;

        protected void RegisterOutgoing<T>(ProtocolRole type, OutgoingPacketHandler<T> handler) where T : Packet
        {
            Type t = typeof(T);
            CheckUsingPreview(t);

            (type == ProtocolRole.Server ? serverOutHandlers : clientOutHandlers).Add(typeof(T), (p, writer) =>
            {
                // -- Why bother wrapping it...?
                handler((T)p, writer);
            });
        }

        protected void RegisterDefaults()
        {
            if(Server.Instance != null)
            {
                RegisterServerDefaults();
            } else
            {
                RegisterClientDefaults();
            }
        }

        private void RegisterServerDefaults()
        {
            #region -- Server handlers --
            RegisterIncoming(ProtocolRole.Server, PacketState.Handshake, 0x00, reader =>
            {
                return new SetProtocol
                {
                    ProtocolVersion = reader.ReadVarInt(),
                    ServerAddress = reader.ReadString(),
                    ServerPort = reader.ReadUShort(),
                    NextState = (PacketState)reader.ReadVarInt()
                };
            });

            RegisterIncoming(ProtocolRole.Server, PacketState.Status, 0x00, reader =>
            {
                return new StatusRequest();
            });

            RegisterIncoming(ProtocolRole.Server, PacketState.Status, 0x01, reader =>
            {
                return new PingPong
                {
                    Payload = reader.ReadLong()
                };
            });

            RegisterIncoming(ProtocolRole.Server, PacketState.Login, 0x00, reader =>
            {
                return new LoginStart
                {
                    UserName = reader.ReadString()
                };
            });

            RegisterIncoming(ProtocolRole.Server, PacketState.Login, 0x01, reader =>
            {
                return new EncryptionResponse
                {
                    SharedSecret = reader.ReadByteArray(),
                    VerifyToken = reader.ReadByteArray()
                };
            });

            RegisterOutgoing<StatusResponse>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteString(JsonConvert.SerializeObject(p.Response));
                writer.Flush(0x00);
            });

            RegisterOutgoing<PingPong>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteLong(p.Payload);
                writer.Flush(0x01);
            });

            RegisterOutgoing<LoginKick>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteChat(p.Reason);
                writer.Flush(0x00);
            });
            #endregion
        }

        private void RegisterClientDefaults()
        {
            #region -- Client handlers --
            RegisterOutgoing<SetProtocol>(ProtocolRole.Client, (p, writer) =>
            {
                writer.WriteVarInt(p.ProtocolVersion);
                writer.WriteString(p.ServerAddress);
                writer.WriteShort(p.ServerPort);
                writer.WriteVarInt((int)p.NextState);
                writer.Flush(0x00);
            });

            RegisterOutgoing<StatusRequest>(ProtocolRole.Client, (p, writer) =>
            {
                writer.Flush(0x00);
            });

            RegisterOutgoing<LoginStart>(ProtocolRole.Client, (p, writer) =>
            {
                writer.WriteString(p.UserName);
                writer.Flush(0x00);
            });
            #endregion
        }
    }
}
