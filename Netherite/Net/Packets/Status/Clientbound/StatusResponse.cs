using Netherite.Converters.Json;
using Netherite.Texts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Netherite.Net.Packets.Status.Clientbound
{
    public class StatusResponse : Packet
    {
        public ProtocolResponse Response { get; set; }

        public StatusResponse() : base() { }

        public StatusResponse(ProtocolResponse response) : base()
        {
            Response = response;
        }
    }


    public class ProtocolResponse
    {
        public class VersionMeta
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("protocol")]
            public int Protocol { get; set; }
        }

        public class PlayersMeta
        {
            [JsonProperty("max")]
            public int Max { get; set; }

            [JsonProperty("online")]
            public int Online { get; set; }

            [JsonProperty("sample")]
            public ICollection<PlayerSample> Samples { get; set; }
            
        }

        public class PlayerSample
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonConverter(typeof(GuidConverter))]
            [JsonProperty("id")]
            public Guid Id { get; set; }
        }

        [JsonProperty("version")]
        public VersionMeta Version { get; set; }

        [JsonProperty("players")]
        public PlayersMeta Players { get; set; }

        [JsonProperty("description")]
        public Text Description { get; set; }
    }
}
