using System;
using System.Collections.Generic;
using Netherite.Auth.Properties;
using Netherite.Data.Entities;
using Netherite.Nbt;
using Netherite.Net.Packets;
using Netherite.Net.Packets.Login.Clientbound;
using Netherite.Net.Packets.Play;
using Netherite.Net.Packets.Play.Clientbound;
using Netherite.Net.Packets.Play.Serverbound;
using Netherite.Net.Protocols;
using Netherite.Protocols.v755;
using Netherite.Utils;

namespace Netherite.Protocols.v755
{
    public class Protocol_v755 : Protocol
    {
        public override int Version => 755;

        public override string VersionName => "1.17";

        static Protocol_v755()
        {
            Register(755, new Protocol_v755());
            Logger.Info("Loading block states for 1.17...");
            v755.Registry.EnsureLoad();
        }

        internal Protocol_v755()
        {
            RegisterDefaults();

            if (Server.Instance == null)
            {
                RegisterClientPackets();
            }
            else
            {
                RegisterServerPackets();
            }
        }

        internal void RegisterClientPackets()
        {
            throw new NotImplementedException();
        }

        internal void RegisterServerPackets()
        {
            #region ------ Incoming packets ------

            RegisterIncoming(ProtocolRole.Server, PacketState.Play, 0x03, reader =>
            {
                return new PlayerChat
                {
                    Message = reader.ReadString()
                };
            });

            RegisterIncoming(ProtocolRole.Server, PacketState.Play, 0x05, reader =>
            {
                return new ClientSettingsPacket
                {
                    Locale = reader.ReadString(),
                    ViewDistance = reader.ReadByte(),
                    ChatMode = reader.ReadVarInt(),
                    ChatColors = reader.ReadBool(),
                    SkinParts = (SkinPart) reader.ReadByte(),
                    Mainhand = reader.ReadVarInt()
                };
            });

            RegisterIncoming(ProtocolRole.Server, PacketState.Play, 0x0b, reader =>
            {
                return new PluginMessage
                {
                    Channel = reader.ReadString(),
                    Data = reader.ReadRemaining()
                };
            });

            RegisterIncoming(ProtocolRole.Server, PacketState.Play, 0x10,
                reader => new KeepAlivePacket(reader.ReadLong()));

            RegisterIncoming(ProtocolRole.Server, PacketState.Play, 0x12, reader =>
            {
                return new UpdatePlayerPosition
                {
                    X = reader.ReadDouble(),
                    Y = reader.ReadDouble(),
                    Z = reader.ReadDouble(),
                    OnGround = reader.ReadBool()
                };
            });

            RegisterIncoming(ProtocolRole.Server, PacketState.Play, 0x13, reader =>
            {
                return new UpdatePlayerPositionAndLook
                {
                    X = reader.ReadDouble(),
                    Y = reader.ReadDouble(),
                    Z = reader.ReadDouble(),
                    Yaw = reader.ReadFloat(),
                    Pitch = reader.ReadFloat(),
                    OnGround = reader.ReadBool()
                };
            });

            RegisterIncoming(ProtocolRole.Server, PacketState.Play, 0x14, reader =>
            {
                return new UpdatePlayerLook
                {
                    Yaw = reader.ReadFloat(),
                    Pitch = reader.ReadFloat(),
                    OnGround = reader.ReadBool()
                };
            });

            RegisterIncoming(ProtocolRole.Server, PacketState.Play, 0x15, reader =>
            {
                return new PlayerOnGround
                {
                    OnGround = reader.ReadBool()
                };
            });

            #endregion

            #region ------ Outgoing packets ------

            RegisterOutgoing<EncryptionRequest>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteString(p.ServerID);
                writer.WriteByteArray(p.PublicKey);
                writer.WriteByteArray(p.VerifyToken);
                writer.Flush(0x01);
            });

            RegisterOutgoing<LoginSuccess>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteGuid(p.Guid);
                writer.WriteString(p.UserName);
                writer.Flush(0x02);
            });

            RegisterOutgoing<SculkVibrationSignal>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteLongPos(p.Source);
                writer.WriteIdentifier(p.DestinationType);

                if (p.DestinationType.Key == "block")
                {
                    writer.WriteLongPos(p.TargetPosition);
                }
                else if (p.DestinationType.Key == "entity")
                {
                    writer.WriteVarInt(p.TargetEntity.Handle);
                }

                writer.Flush(0x05);
            });

            // All the packet ID are shifted by 1 after ID 0x5.

            RegisterOutgoing<BlockChange>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteLongPos(p.Position);
                writer.WriteVarInt(v755.Registry.IdState.Find(t => t.Item2 == p.State.ToString()).Item1);
                writer.Flush(0x0c);
            });

            RegisterOutgoing<ChatPacket>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteChat(p.Message);
                writer.WriteByte((byte) p.Position);
                writer.WriteGuid(p.SenderGuid);
                writer.Flush(0x0f);
            });

            RegisterOutgoing<PluginMessage>(ProtocolRole.Server, (p, writer) =>
            {
                // Should be an identifier here.
                writer.WriteString(p.Channel);
                foreach (byte b in p.Data)
                {
                    writer.WriteByte(b);
                }

                writer.Flush(0x18);
            });

            RegisterOutgoing<ServerBrand>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteIdentifier(new Identifier("brand"));
                writer.WriteString(p.Name);
                writer.Flush(0x18);
            });

            RegisterOutgoing<Kick>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteChat(p.Reason);
                writer.Flush(0x1a);
            });

            RegisterOutgoing<UnloadChunkPacket>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteInt(p.Chunk.X);
                writer.WriteInt(p.Chunk.Z);
                writer.Flush(0x1d);
            });

            RegisterOutgoing<KeepAlivePacket>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteLong(p.Payload);
                writer.Flush(0x20);
            });

            RegisterOutgoing<ChunkDataPacket>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteChunk(p.Chunk);
                writer.Flush(0x21);
            });

            RegisterOutgoing<JoinGame>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteInt(p.Player.Handle);
                writer.WriteBool(p.IsHardcore);
                writer.WriteByte((byte) p.Mode);

                byte prev = 0xff;
                if (p.PreviousMode != null)
                {
                    prev = (byte) p.PreviousMode;
                }

                writer.WriteByte(prev);

                // world count & names
                writer.WriteVarInt(p.Worlds.Count);
                foreach (Identifier id in p.Worlds)
                {
                    writer.WriteIdentifier(id);
                }

                // dimension registry, type
                var codec = Server.Instance.Registry.GetDimensionCodec();
                writer.WriteNbt(codec);

                NbtCompound c = (NbtCompound) p.Dimension.GetCodec()["element"];
                c.Name = "";
                writer.WriteNbt(c);

                // world name
                writer.WriteIdentifier(p.WorldName);

                writer.WriteLong(p.Seed);
                writer.WriteVarInt(p.MaxPlayers);
                writer.WriteVarInt(p.ViewDistance);
                writer.WriteBool(p.ReducedDebugInfo);
                writer.WriteBool(p.EnableRespawnScreen);
                writer.WriteBool(p.IsDebugWorld);
                writer.WriteBool(p.IsFlatWorld);

                writer.Flush(0x25);
            });

            RegisterOutgoing<PlayerInfo>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteVarInt((int) p.Action);
                writer.WriteVarInt(p.Players.Count);

                foreach (PlayerInfo.Meta player in p.Players)
                {
                    writer.WriteGuid(player.Profile.Guid);
                    switch (p.Action)
                    {
                        case PlayerInfo.PacketAction.AddPlayer:
                            writer.WriteString(player.Profile.Name);

                            List<Property> props = player.Profile.Properties.Properties;
                            writer.WriteVarInt(props.Count);

                            foreach (Property prop in props)
                            {
                                bool signed = player.Profile.Properties.Properties[0].IsSigned;
                                writer.WriteVarInt(signed ? 4 : 3);

                                writer.WriteString(prop.Name);
                                writer.WriteString(prop.Value);
                                writer.WriteBool(prop.IsSigned);

                                if (prop.IsSigned) writer.WriteString(prop.Signature);
                            }

                            writer.WriteVarInt((int) player.Mode);
                            writer.WriteVarInt(player.Latency);
                            writer.WriteBool(player.DisplayName != null);
                            if (player.DisplayName != null) writer.WriteChat(player.DisplayName);
                            break;
                        case PlayerInfo.PacketAction.UpdateGameMode:
                            writer.WriteVarInt((int) player.Mode);
                            break;
                        case PlayerInfo.PacketAction.UpdateLatency:
                            writer.WriteVarInt(player.Latency);
                            break;
                        case PlayerInfo.PacketAction.UpdateDisplayName:
                            writer.WriteBool(player.DisplayName != null);
                            if (player.DisplayName != null) writer.WriteChat(player.DisplayName);
                            break;
                        case PlayerInfo.PacketAction.RemovePlayer:
                            break;
                    }
                }

                writer.Flush(0x33);
            });

            RegisterOutgoing<PlayerPositionAndLook>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteDouble(p.X);
                writer.WriteDouble(p.Y);
                writer.WriteDouble(p.Z);
                writer.WriteFloat(p.Yaw);
                writer.WriteFloat(p.Pitch);
                writer.WriteByte(p.RelationFlags);
                writer.WriteVarInt(p.TeleportId);
                writer.Flush(0x35);
            });

            RegisterOutgoing<ViewPosition>(ProtocolRole.Server, (p, writer) =>
            {
                writer.WriteVarInt(p.ChunkX);
                writer.WriteVarInt(p.ChunkZ);
                writer.Flush(0x41);
            });

            #endregion
        }
    }
}