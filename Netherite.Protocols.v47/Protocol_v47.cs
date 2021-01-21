using Netherite.Auth.Properties;
using Netherite.Data.Entities;
using Netherite.Entities;
using Netherite.Nbt;
using Netherite.Nbt.Serializations;
using Netherite.Net.IO;
using Netherite.Net.Packets;
using Netherite.Net.Packets.Login.Clientbound;
using Netherite.Net.Packets.Play;
using Netherite.Net.Packets.Play.Clientbound;
using Netherite.Net.Packets.Play.Serverbound;
using Netherite.Net.Protocols;
using Netherite.Physics;
using Netherite.Texts;
using Netherite.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Netherite.Protocols.v47
{
    public static class BufferReaderExtension
    {
        public static ItemStack ReadSlot(this BufferReader reader)
        {
            var id = reader.ReadShort();
            ItemStack item;

            if (id == -1)
            {
                item = null;
            }
            else
            {
                item = new ItemStack();
                item.Count = reader.ReadByte();
                short data = reader.ReadShort();

                item.Material = LegacyMaterial.FromID(id, (byte)data).Material;

                item.Data = (NbtCompound)reader.ReadNbt();
            }

            return item;
        }
    }

    public class Protocol_v47 : Protocol
    {
        public override int Version => 47;

        public override string VersionName => "1.8.x";

        static Protocol_v47()
        {
            Register(47, new Protocol_v47());
            LegacyMaterial.EnsureLoad();
        }

        internal Protocol_v47()
        {
            RegisterDefaults();

            #region ------ Incoming packets ------

            RegisterIncoming(PacketState.Play, 0x00, reader => new KeepAlivePacket(reader.ReadVarInt()));

            RegisterIncoming(PacketState.Play, 0x01, reader => new PlayerChat
            {
                Message = reader.ReadString()
            });

            RegisterIncoming(PacketState.Play, 0x03, reader => new PlayerOnGround
            {
                OnGround = reader.ReadBool()
            });

            RegisterIncoming(PacketState.Play, 0x04, reader =>
            {
                return new UpdatePlayerPosition
                {
                    X = reader.ReadDouble(),
                    Y = reader.ReadDouble(),
                    Z = reader.ReadDouble(),
                    OnGround = reader.ReadBool()
                };
            });

            RegisterIncoming(PacketState.Play, 0x05, reader =>
            {
                return new UpdatePlayerLook
                {
                    Yaw = reader.ReadFloat(),
                    Pitch = reader.ReadFloat(),
                    OnGround = reader.ReadBool()
                };
            });

            RegisterIncoming(PacketState.Play, 0x06, reader =>
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

            RegisterIncoming(PacketState.Play, 0x08, reader =>
            {
                return new BlockPlace
                {
                    Position = reader.ReadLocation(),
                    Face = reader.ReadByte(),
                    HeldItem = reader.ReadSlot(),
                    CursorPosition = new Vector3
                    {
                        X = reader.ReadByte(),
                        Y = reader.ReadByte(),
                        Z = reader.ReadByte()
                    }
                };
            });

            RegisterIncoming(PacketState.Play, 0x0a, reader => new AnimationIn());

            RegisterIncoming(PacketState.Play, 0x0b, reader =>
            {
                return new EntityAction
                {
                    Entity = Entity.GetById(reader.ReadVarInt()),
                    ActionID = reader.ReadVarInt(),
                    ActionParam = reader.ReadVarInt()
                };
            });

            RegisterIncoming(PacketState.Play, 0x10, reader =>
            {
                return new CreativeInventoryAction
                {
                    Slot = (byte)reader.ReadShort(),
                    ItemStack = reader.ReadSlot()
                };
            });

            RegisterIncoming(PacketState.Play, 0x15, reader =>
            {
                ClientSettingsPacket p = new ClientSettingsPacket
                {
                    Locale = reader.ReadString(),
                    ViewDistance = reader.ReadByte(),
                    ChatMode = reader.ReadByte(),
                    ChatColors = reader.ReadBool(),
                    SkinParts = (SkinPart)reader.ReadByte()
                };
                return p;
            });

            RegisterIncoming(PacketState.Play, 0x17, reader =>
            {
                return new PluginMessage
                {
                    Channel = reader.ReadString(),
                    Data = reader.ReadRemaining()
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
                writer.WriteString(p.Guid.ToString());
                writer.WriteString(p.UserName);
                writer.Flush(0x02);
            });

            RegisterOutgoing<KeepAlivePacket>((p, writer) =>
            {
                writer.WriteVarInt((int)p.Payload);
                writer.Flush(0x00);
            });

            RegisterOutgoing<JoinGame>((p, writer) =>
            {
                writer.WriteInt(p.Player.Handle);

                byte mode = (byte)p.Mode;
                if (p.IsHardcore)
                {
                    mode |= 0x8;
                }
                writer.WriteByte(mode);

                writer.WriteByte(0); // overworld
                writer.WriteByte(0); // peaceful
                writer.WriteByte(10); // max players?
                writer.WriteString("flat");
                writer.WriteBool(false);

                writer.Flush(0x01);
            });

            RegisterOutgoing<ChatPacket>((p, writer) =>
            {
                writer.WriteString(JsonConvert.SerializeObject(p.Message));
                writer.WriteByte((byte)p.Position);
                writer.Flush(0x02);
            });

            RegisterOutgoing<TimeUpdate>((p, writer) =>
            {
                writer.WriteLong(p.WorldAge);
                writer.WriteLong(p.WorldTime);
                writer.Flush(0x03);
            });

            RegisterOutgoing<PerformRespawn>((p, writer) =>
            {
                writer.WriteInt(p.Dimension);
                writer.WriteByte(p.Difficulty);
                writer.WriteByte((byte)p.Mode);
                writer.WriteString(p.WorldName.ToString());

                writer.Flush(0x07);
            });

            RegisterOutgoing<PlayerPositionAndLook>((p, writer) =>
            {
                writer.WriteDouble(p.X);
                writer.WriteDouble(p.Y);
                writer.WriteDouble(p.Z);
                writer.WriteFloat(p.Yaw);
                writer.WriteFloat(p.Pitch);
                writer.WriteByte(p.RelationFlags);
                writer.Flush(0x08);
            });

            RegisterOutgoing<HeldItemChangePacket>((p, writer) =>
            {
                writer.WriteByte(p.Slot);
                writer.Flush(0x09);
            });

            RegisterOutgoing<AnimationOut>((p, writer) =>
            {
                writer.WriteVarInt(p.Entity.Handle);
                writer.WriteByte(p.Animation);
                writer.Flush(0x0b);
            });

            RegisterOutgoing<SpawnPlayer>((p, writer) =>
            {
                writer.WriteVarInt(p.Player.Handle);
                writer.WriteGuid(p.Player.Guid);
                writer.WriteFixedPointI(p.Player.Position.X);
                writer.WriteFixedPointI(p.Player.Position.Y);
                writer.WriteFixedPointI(p.Player.Position.Z);
                writer.WriteAngle(p.Player.Yaw);
                writer.WriteAngle(p.Player.Pitch);
                writer.WriteShort(p.CurrentItem);
                PlayerMetadataWriter mw = new PlayerMetadataWriter(p.Metadata, writer);
                mw.Write();
                writer.WriteByte(127); // metadata end

                writer.Flush(0x0c);
            });

            RegisterOutgoing<DestroyEntityPacket>((p, writer) =>
            {
                writer.WriteVarInt(p.Entities.Count);
                foreach (Entity e in p.Entities)
                {
                    writer.WriteVarInt(e.Handle);
                }
                writer.Flush(0x13);
            });

            RegisterOutgoing<EntityRelativeMove>((p, writer) =>
            {
                writer.WriteVarInt(p.Entity.Handle);
                writer.WriteFixedPointB(p.Delta.X);
                writer.WriteFixedPointB(p.Delta.Y);
                writer.WriteFixedPointB(p.Delta.Z);
                writer.WriteBool(p.OnGround);
                writer.Flush(0x15);
            });

            RegisterOutgoing<EntityLook>((p, writer) =>
            {
                writer.WriteVarInt(p.Entity.Handle);
                writer.WriteAngle(p.Yaw);
                writer.WriteAngle(p.Pitch);
                writer.WriteBool(p.OnGround);
                writer.Flush(0x16);
            });

            RegisterOutgoing<EntityLookAndRelativeMove>((p, writer) =>
            {
                writer.WriteVarInt(p.Entity.Handle);
                writer.WriteFixedPointB(p.Delta.X);
                writer.WriteFixedPointB(p.Delta.Y);
                writer.WriteFixedPointB(p.Delta.Z);
                writer.WriteAngle(p.Yaw);
                writer.WriteAngle(p.Pitch);
                writer.WriteBool(p.OnGround);
                writer.Flush(0x17);
            });

            RegisterOutgoing<EntityTeleport>((p, writer) =>
            {
                writer.WriteVarInt(p.Entity.Handle);
                writer.WriteFixedPointI(p.Position.X);
                writer.WriteFixedPointI(p.Position.Y);
                writer.WriteFixedPointI(p.Position.Z);
                writer.WriteAngle(p.Yaw);
                writer.WriteAngle(p.Pitch);
                writer.WriteBool(p.OnGround);
                writer.Flush(0x18);
            });

            RegisterOutgoing<EntityHeadLook>((p, writer) =>
            {
                int handle = p.Entity.Handle;
                if (p.Entity is DummyEntity dm) handle = dm.Handle;
                writer.WriteVarInt(handle);
                writer.WriteAngle(p.Yaw);
                writer.Flush(0x19);
            });

            RegisterOutgoing<EntityMetadataPacket>((p, writer) =>
            {
                writer.WriteVarInt(p.Entity.Handle);

                var w = EntityMetadataWriter.GetWriter(p.Metadata, writer);
                w.Write();

                writer.WriteByte(127);

                writer.Flush(0x1c);
            });

            RegisterOutgoing<ChunkDataPacket>((p, writer) =>
            {
                var chunkWriter = new ChunkDataWriter(p.Chunk);
                chunkWriter.WriteTo(writer);
                writer.Flush(0x21);
            });

            RegisterOutgoing<BlockChange>((p, writer) =>
            {
                writer.WriteLongPos(p.Position);

                LegacyMaterial lm = LegacyMaterial.FromMaterial(p.State.Material);
                writer.WriteByte((byte)(lm.Id << 4 | lm.Data & 0xf));

                writer.Flush(0x23);
            });

            RegisterOutgoing<PlaySoundEffect>((p, writer) =>
            {
                writer.WriteString(p.Sound);
                writer.WriteInt((int)p.X * 8);
                writer.WriteInt((int)p.Y * 8);
                writer.WriteInt((int)p.Z * 8);
                writer.WriteFloat(p.Volume);
                writer.WriteByte((byte)(63 * p.Pitch));
                writer.Flush(0x29);
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

                writer.Flush(0x38);
            });

            RegisterOutgoing<ServerBrand>((p, writer) =>
            {
                writer.WriteString("MC|Brand");
                writer.WriteString(p.Name);

                writer.Flush(0x3f);
            });

            RegisterOutgoing<PluginMessage>((p, writer) =>
            {
                Logger.Log(
                    TranslateText.Of("Plugin channel: {0}: {1}")
                        .AddWith(LiteralText.Of(p.Channel).SetColor(TextColor.Gold))
                        .AddWith(LiteralText.Of(Encoding.UTF8.GetString(p.Data)).SetColor(TextColor.DarkGray))
                    );
                Logger.Log(
                    TranslateText.Of("Hex: {0}")
                        .AddWith(LiteralText.Of(DebugHelper.HexDump(p.Data)).SetColor(TextColor.DarkGray))
                    );

                writer.WriteString(p.Channel);
                foreach (byte b in p.Data)
                {
                    writer.WriteByte(b);
                }

                writer.Flush(0x3f);
            });

            RegisterOutgoing<Kick>((p, writer) =>
            {
                writer.WriteChat(p.Reason);
                writer.Flush(0x40);
            });

            RegisterOutgoing<DisplayTitle>((p, writer) =>
            {
                writer.WriteVarInt((int)p.Action);

                switch (p.Action)
                {
                    case DisplayTitle.PacketAction.SetTitle:
                        writer.WriteChat(p.Title);
                        break;
                    case DisplayTitle.PacketAction.SetSubtitle:
                        writer.WriteChat(p.Subtitle);
                        break;
                    case DisplayTitle.PacketAction.SetTimesAndDisplay:
                        writer.WriteInt(p.FadeIn);
                        writer.WriteInt(p.Stay);
                        writer.WriteInt(p.FadeOut);
                        break;
                }

                writer.Flush(0x45);
            });

            RegisterOutgoing<PlayerListHeaderAndFooter>((p, writer) =>
            {
                writer.WriteChat(p.Header);
                writer.WriteChat(p.Footer);
                writer.Flush(0x47);
            });

            RegisterOutgoing<ResourcePackSend>((p, writer) =>
            {
                writer.WriteString(p.Url);
                writer.WriteString(p.Hash);
                writer.Flush(0x48);
            });

            RegisterOutgoing<EntityNBT>((p, writer) =>
            {
                writer.WriteVarInt(p.Entity.Handle);
                writer.WriteNbt(p.Tag);
                writer.Flush(0x49);
            });
            #endregion
        }
    }
}
