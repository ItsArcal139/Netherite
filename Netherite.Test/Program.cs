using Netherite.Nbt;
using Netherite.Net.IO;
using System;
using System.IO;

namespace Netherite.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] buf = File.ReadAllBytes("chunkdata.bin");
            BufferReader reader = new BufferReader(buf);

            Console.WriteLine(reader.ReadInt());
            Console.WriteLine(reader.ReadInt());

            int mask = reader.ReadVarInt();
            Console.WriteLine(mask);

            NbtCompound heightmap = (NbtCompound)reader.ReadNbt();
            Console.WriteLine(heightmap.ToString());

            int biomes = reader.ReadVarInt();
            Console.WriteLine(biomes + " biomes");
            for (int i = 0; i < biomes; i++)
            {
                reader.ReadVarInt();
            }

            reader.ReadVarInt();
            for (int i = 0; i < 16; i++)
            {
                int flag = 1 << i;
                if ((mask & flag) == flag)
                {
                    // Read data
                    Console.WriteLine("------");
                    Console.WriteLine("chunkY: " + i);
                    Console.WriteLine("blocks: " + reader.ReadShort());
                    Console.WriteLine("bit/block: " + reader.ReadByte());

                    Console.Write("Palette: ");
                    int palettes = reader.ReadVarInt();
                    for (int j = 0; j < palettes; j++)
                    {
                        Console.Write(reader.ReadVarInt() + " ");
                    }
                    Console.WriteLine();

                    int datas = reader.ReadVarInt();
                    Console.WriteLine(datas + " items in long[]");
                    for (int j = 0; j < datas; j++)
                    {
                        reader.ReadLong();
                    }

                    for (int j = 0; j < 4096; j++)
                    {
                        reader.ReadByte();
                    }
                }
            }

            int entities = reader.ReadVarInt();
            Console.WriteLine(entities + " block entities");
            for (int i = 0; i < entities; i++)
            {
                reader.ReadNbt();
            }

            byte[] rem = reader.ReadRemaining();
            Console.WriteLine(rem.Length + " bytes remaining");
        }
    }
}
