using Netherite.Auth.Properties;
using Newtonsoft.Json;

namespace Netherite.Utils.Mojang
{
    public class JoinedProperty
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        public static explicit operator Property(JoinedProperty sk)
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
