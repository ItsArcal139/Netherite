using Netherite.Nbt.Serializations;
using System;
using System.IO;

namespace Netherite.Nbt.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] buf = NbtConvert.SerializeToBuffer(new Person
            {
                Name = "Kaka",
                Age = 18,
                Couple = new Person
                {
                    Name = "Rinka",
                    Age = 9
                }
            });

            Person p = NbtConvert.Deserialize<Person>(buf);
        }
    }

    public class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Person Couple { get; set; }
    }
}
