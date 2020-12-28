using Netherite.Auth;
using Netherite.Auth.Properties;
using Netherite.Net.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netherite.Utils.Mojang
{
    public class MojangUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("legacy")]
        public bool Legacy { get; set; }

        [JsonProperty("demo")]
        public bool Demo { get; set; }

        [JsonProperty("properties")]
        public List<SkinProperties> Properties { get; set; }

        public static explicit operator GameProfile(MojangUser u)
        {
            return new GameProfile(Guid.Parse(u.Id), u.Name)
            {
                Properties = new PropertyMap
                {
                    Properties = u.Properties?.ConvertAll(p => (Property)p)
                }
            };
        }
    }

    public class SkinProperties
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        public static explicit operator Property(SkinProperties sk)
        {
            return new Property
            {
                Name = sk.Name,
                Value = sk.Value,
                Signature = sk.Signature
            };
        }
    }
}
