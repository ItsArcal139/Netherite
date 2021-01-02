using Netherite.Auth.Properties;
using Netherite.Data.Entities;
using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Net.IO;
using Netherite.Net.Packets;
using Netherite.Net.Packets.Login.Clientbound;
using Netherite.Net.Packets.Play.Clientbound;
using Netherite.Net.Protocols;
using Netherite.Worlds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Netherite.Protocols.v754
{
    public static class BufferWriterExtension
    {
        public static void WriteChunk(this BufferWriter writer, Chunk chunk)
        {
            writer.WriteInt(chunk.X);
            writer.WriteInt(chunk.Z);
            writer.WriteBool(true);

            int mask = 0;
            BufferWriter col = new BufferWriter();
            for(int sectionY = 0; sectionY < (256 / 16); sectionY++)
            {
                if(!chunk.IsSectionEmpty(sectionY))
                {
                    mask |= (1 << sectionY);
                    writer.WriteChunkSection(chunk.Sections[sectionY]);
                }
            }
        }

        public static void WriteChunkSection(this BufferWriter writer, ChunkSection section)
        {
            
        }
    }

    public class Protocol_v754 : Protocol
    {
        public override int Version => 754;

        public override string VersionName => "1.16.4";

        static Protocol_v754()
        {
            Register(754, new Protocol_v754());
        }

        internal Protocol_v754()
        {
            RegisterDefaults();

            #region ------ Incoming packets ------

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

            RegisterOutgoing<ChatPacket>((p, writer) =>
            {
                writer.WriteChat(p.Message);
                writer.WriteByte((byte)p.Position);
                writer.WriteGuid(p.SenderGuid);
                writer.Flush(0x0e);
            });

            RegisterOutgoing<Kick>((p, writer) =>
            {
                writer.WriteChat(p.Reason);
                writer.Flush(0x19);
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

                File.WriteAllBytes("dimension_codec.nbt", NbtConvert.SerializeToBuffer(codec));

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

            #endregion
        }
    }
}
