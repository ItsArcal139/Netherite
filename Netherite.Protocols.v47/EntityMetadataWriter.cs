using Netherite.Entities;
using Netherite.Net.IO;
using Netherite.Net.Packets.Play.Serverbound;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Protocols.v47
{
    public static class EntityMetadataWriterExtension
    {
        public static void Write(this PlayerMetadata meta, BufferWriter writer)
        {
            PlayerMetadataWriter pw = new PlayerMetadataWriter(meta, writer);
            pw.Write();
        }

        public static void Write(this LivingEntityMetadata meta, BufferWriter writer)
        {
            LivingEntityMetadataWriter lem = new LivingEntityMetadataWriter(meta, writer);
            lem.Write();
        }

        public static void Write(this EntityMetadata meta, BufferWriter writer)
        {
            EntityMetadataWriter ew = new EntityMetadataWriter(meta, writer);
            ew.Write();
        }
    }

    public class EntityMetadataWriter
    {
        public EntityMetadata Metadata { get; private set; }
        protected BufferWriter writer;

        public EntityMetadataWriter(EntityMetadata metadata, BufferWriter writer)
        {
            Metadata = metadata;
            this.writer = writer;
        }

        protected enum FieldType : byte
        {
            Byte, Short, Int, Float, String, Slot, [Obsolete] Int3, Float3
        }

        protected byte MakeFieldPrefix(byte type, byte index) => (byte)((type & 0b111) << 5 | index & 0x1f);

        protected byte MakeFieldPrefix(FieldType type, byte index) => MakeFieldPrefix((byte)type, index);

        public virtual void Write()
        {
            byte state = 0;
            if (Metadata.IsOnFire) state |= 0x1;
            if (Metadata.IsCrouching) state |= 0x2;
            if (Metadata.IsSprinting) state |= 0x8;
            if (Metadata.IsInvisible) state |= 0x20;

            writer.WriteByte(MakeFieldPrefix(FieldType.Byte, 0));
            writer.WriteByte(state);

            writer.WriteByte(MakeFieldPrefix(FieldType.Float, 1));
            writer.WriteFloat(Metadata.AirTicks);

            writer.WriteByte(MakeFieldPrefix(FieldType.Byte, 4));
            writer.WriteBool(Metadata.Silent);
        }

        public static EntityMetadataWriter GetWriter(EntityMetadata metadata, BufferWriter writer)
        {
            if (metadata is PlayerMetadata pm) return new PlayerMetadataWriter(pm, writer);
            if (metadata is LivingEntityMetadata lem) return new LivingEntityMetadataWriter(lem, writer);
            return new EntityMetadataWriter(metadata, writer);
        }
    }

    public class LivingEntityMetadataWriter : EntityMetadataWriter
    {
        public new LivingEntityMetadata Metadata { get; set; }

        public LivingEntityMetadataWriter(LivingEntityMetadata metadata, BufferWriter writer) : base(metadata, writer)
        {
            Metadata = metadata;
        }

        public override void Write()
        {
            base.Write();

            writer.WriteByte(MakeFieldPrefix(FieldType.String, 2));
            writer.WriteString(Metadata.CustomName?.ToPlainText() ?? "");

            writer.WriteByte(MakeFieldPrefix(FieldType.Byte, 3));
            writer.WriteBool(Metadata.CustomNameVisible);

            writer.WriteByte(MakeFieldPrefix(FieldType.Float, 6));
            writer.WriteFloat(Metadata.Health);

            writer.WriteByte(MakeFieldPrefix(FieldType.Int, 7));
            writer.WriteInt(Metadata.PotionEffectColor.RGB);

            writer.WriteByte(MakeFieldPrefix(FieldType.Byte, 8));
            writer.WriteBool(Metadata.IsPotionEffectAmbient);

            writer.WriteByte(MakeFieldPrefix(FieldType.Byte, 9));
            writer.WriteByte((byte)Metadata.ArrowCount);

            writer.WriteByte(MakeFieldPrefix(FieldType.Byte, 15));
            writer.WriteBool(true);
        }
    }

    public class PlayerMetadataWriter : LivingEntityMetadataWriter
    {
        public new PlayerMetadata Metadata { get; set; }

        public PlayerMetadataWriter(PlayerMetadata metadata, BufferWriter writer) : base(metadata, writer)
        {
            Metadata = metadata;
        }

        public override void Write()
        {
            base.Write();

            writer.WriteByte(MakeFieldPrefix(FieldType.Byte, 10));
            writer.WriteByte((byte)Metadata.EnabledParts);

            writer.WriteByte(MakeFieldPrefix(FieldType.Byte, 16));
            writer.WriteByte((byte)(Metadata.EnabledParts.HasFlag(SkinPart.Cape) ? 2 : 0));

            writer.WriteByte(MakeFieldPrefix(FieldType.Float, 17));
            writer.WriteFloat(Metadata.Absorption);

            writer.WriteByte(MakeFieldPrefix(FieldType.Int, 18));
            writer.WriteInt(Metadata.Score);
        }
    }
}
