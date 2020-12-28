namespace Netherite.Nbt.Serializations
{
    internal interface INbtSerializer
    {
        void WriteByte(NbtByte tag);
        void WriteShort(NbtShort tag);
        void WriteInt(NbtInt tag);
        void WriteLong(NbtLong tag);
        void WriteFloat(NbtFloat tag);
        void WriteDouble(NbtDouble tag);
        void WriteByteArray(NbtByteArray tag);
        void WriteString(NbtString tag);
        void WriteList(NbtList tag);
        void WriteCompound(NbtCompound tag);
        void WriteIntArray(NbtIntArray tag);
        void WriteLongArray(NbtLongArray tag);
        void Write(NbtTag tag);
    }
}
