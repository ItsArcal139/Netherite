using Netherite.Auth;
using Netherite.Auth.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Netherite.Utils.Mojang
{
    public class JoinedResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string PlayerName { get; set; }

        [JsonProperty("properties")]
        public List<JoinedProperty> Properties { get; set; }

        public static explicit operator GameProfile(JoinedResponse u)
        {
            return new GameProfile(Guid.Parse(u.Id), u.PlayerName)
            {
                Properties = new PropertyMap
                {
                    Properties = u.Properties.ConvertAll(p => (Property)p)
                }
            };
        }
    }
}
