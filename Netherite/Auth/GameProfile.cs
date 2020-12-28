using Netherite.Auth.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Auth
{
    public class GameProfile
    {
        public Guid Guid { get; private set; }
        public string Name { get; private set; }
        public PropertyMap Properties { get; set; }

        public GameProfile(Guid guid, string name)
        {
            Properties = new PropertyMap();

            if(Guid == null && (Name == null || Name == ""))
            {
                throw new ArgumentException("Name and ID cannot both be blank.");
            }

            Guid = guid;
            Name = name;
        }

        public bool IsComplete => Guid != null && Name != null && Name != "";
    }
}
