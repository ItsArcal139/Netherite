using Netherite.Auth.Properties;
using Netherite.Data.Entities;
using Netherite.Entities;
using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Net.Packets;
using Netherite.Net.Packets.Login.Clientbound;
using Netherite.Net.Packets.Play;
using Netherite.Net.Packets.Play.Clientbound;
using Netherite.Net.Packets.Play.Serverbound;
using Netherite.Net.Protocols;
using Netherite.Texts;
using Netherite.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Netherite.Protocols.v754
{

    public class Protocol_v754 : Protocol
    {
        public override int Version => 754;

        public override string VersionName => "1.16.4, 1.16.5";

        static Protocol_v754()
        {
            Register(754, new Protocol_v754());

            Logger.Info(
                LiteralText.Of("Loading block states for protocol 754...")
                );
            Registry.EnsureLoad();
        }

        internal Protocol_v754()
        {
            RegisterDefaults();

            #region ------ Incoming packets ------

            RegisterIncoming(PacketState.Play, 0x03, reader =>
            {
                return new PlayerChat
                {
                    Message = reader.ReadString()
                };
            });

            RegisterIncoming(PacketState.Play, 0x05, reader =>
            {
                return new ClientSettingsPacket
                {
                    Locale = reader.ReadString(),
                    ViewDistance = reader.ReadByte(),
                    ChatMode = reader.ReadVarInt(),
                    ChatColors = reader.ReadBool(),
                    SkinParts = (SkinPart)reader.ReadByte(),
                    Mainhand = reader.ReadVarInt()
                };
            });

            RegisterIncoming(PacketState.Play, 0x0b, reader =>
            {
                return new PluginMessage
                {
                    Channel = reader.ReadString(),
                    Data = reader.ReadRemaining()
                };
            });

            RegisterIncoming(PacketState.Play, 0x10, reader => new KeepAlivePacket(reader.ReadLong()));

            RegisterIncoming(PacketState.Play, 0x12, reader =>
            {
                return new UpdatePlayerPosition
                {
                    X = reader.ReadDouble(),
                    Y = reader.ReadDouble(),
                    Z = reader.ReadDouble(),
                    OnGround = reader.ReadBool()
                };
            });

            RegisterIncoming(PacketState.Play, 0x13, reader =>
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

            RegisterIncoming(PacketState.Play, 0x14, reader =>
            {
                return new UpdatePlayerLook
                {
                    Yaw = reader.ReadFloat(),
                    Pitch = reader.ReadFloat(),
                    OnGround = reader.ReadBool()
                };
            });

            RegisterIncoming(PacketState.Play, 0x15, reader =>
            {
                return new PlayerOnGround
                {
                    OnGround = reader.ReadBool()
                };
            });

            #endregion

            #region ------ Outgoing packets ------

            RegisterOutgoing<EncryptionRequest>((p, writer) =>
            {
                writer.WriteString(p.ServerID);
                writer.WriteByteArray(p.PublicKey);
                writer.WriteByteArray(p.VerifyToken);
                writer.Flush(0x01);
            });

            RegisterOutgoing<LoginSuccess>((p, writer) =>
            {
                writer.WriteGuid(p.Guid);
                writer.WriteString(p.UserName);
                writer.Flush(0x02);
            });

            RegisterOutgoing<SpawnEntity>((p, writer) =>
            {
                writer.WriteVarInt(p.Entity.Handle);
                writer.WriteGuid(p.Entity.Guid);
                writer.WriteVarInt(0); // Entity type
                writer.WriteDouble(p.Entity.Position.X);
                writer.WriteDouble(p.Entity.Position.Y);
                writer.WriteDouble(p.Entity.Position.Z);
                writer.WriteAngle(p.Entity.Pitch);
                writer.WriteAngle(p.Entity.Yaw);
                writer.WriteInt(0);   // Data
                writer.WriteShort(0); // Velocity X
                writer.WriteShort(0); // Velocity Y
                writer.WriteShort(0); // Velocity Z
                writer.Flush(0x00);
            });

            RegisterOutgoing<SpawnPlayer>((p, writer) =>
            {
                writer.WriteVarInt(p.Player.Handle);
                writer.WriteGuid(p.Player.Guid);
                writer.WriteDouble(p.Player.Position.X);
                writer.WriteDouble(p.Player.Position.Y);
                writer.WriteDouble(p.Player.Position.Z);
                writer.WriteFloat(p.Player.Yaw);
                writer.WriteFloat(p.Player.Pitch);
                writer.Flush(0x04);
            });

            RegisterOutgoing<ChatPacket>((p, writer) =>
            {
                writer.WriteChat(p.Message);
                writer.WriteByte((byte)p.Position);
                writer.WriteGuid(p.SenderGuid);
                writer.Flush(0x0e);
            });

            RegisterOutgoing<PluginMessage>((p, writer) =>
            {
                // Should be an identifier here.
                writer.WriteString(p.Channel);
                foreach (byte b in p.Data)
                {
                    writer.WriteByte(b);
                }

                writer.Flush(0x17);
            });

            RegisterOutgoing<ServerBrand>((p, writer) =>
            {
                writer.WriteIdentifier(new Identifier("brand"));
                writer.WriteString(p.Name);
                writer.Flush(0x17);
            });

            RegisterOutgoing<Kick>((p, writer) =>
            {
                writer.WriteChat(p.Reason);
                writer.Flush(0x19);
            });

            RegisterOutgoing<KeepAlivePacket>((p, writer) =>
            {
                writer.WriteLong(p.Payload);
                writer.Flush(0x1f);
            });

            RegisterOutgoing<ChunkDataPacket>((p, writer) =>
            {
                writer.WriteChunk(p.Chunk);
                writer.Flush(0x20);
            });

            RegisterOutgoing<JoinGame>((p, writer) =>
            {
                writer.WriteInt(p.Player.Handle);
                writer.WriteBool(p.IsHardcore);
                writer.WriteByte((byte)p.Mode);

                byte prev = 0xff;
                if (p.PreviousMode != null)
                {
                    prev = (byte)p.PreviousMode;
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

                NbtCompound c = (NbtCompound)p.Dimension.GetCodec()["element"];
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

                writer.Flush(0x24);
            });

            RegisterOutgoing<PlayerInfo>((p, writer) =>
            {
                writer.WriteVarInt((int)p.Action);
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

                            writer.WriteVarInt((int)player.Mode);
                            writer.WriteVarInt(player.Latency);
                            writer.WriteBool(player.DisplayName != null);
                            if (player.DisplayName != null) writer.WriteChat(player.DisplayName);
                            break;
                        case PlayerInfo.PacketAction.UpdateGameMode:
                            writer.WriteVarInt((int)player.Mode);
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

                writer.Flush(0x32);
            });

            RegisterOutgoing<PlayerPositionAndLook>((p, writer) =>
            {
                writer.WriteDouble(p.X);
                writer.WriteDouble(p.Y);
                writer.WriteDouble(p.Z);
                writer.WriteFloat(p.Yaw);
                writer.WriteFloat(p.Pitch);
                writer.WriteByte(p.RelationFlags);
                writer.WriteVarInt(p.TeleportId);
                writer.Flush(0x34);
            });

            RegisterOutgoing<ViewPosition>((p, writer) =>
            {
                writer.WriteVarInt(p.ChunkX);
                writer.WriteVarInt(p.ChunkZ);
                writer.Flush(0x40);
            });

            RegisterOutgoing<TimeUpdate>((p, writer) =>
            {
                writer.WriteLong(p.WorldAge);
                writer.WriteLong(p.WorldTime);
                writer.Flush(0x4e);
            });

            #endregion
        }
    }
}
