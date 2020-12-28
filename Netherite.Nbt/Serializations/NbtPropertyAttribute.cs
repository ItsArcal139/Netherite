using System;

namespace Netherite.Nbt.Serializations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NbtPropertyAttribute : Attribute
    {
        public string Name { get; set; }

        public NbtPropertyAttribute(string name)
        {
            Name = name;
        }
    }
}
