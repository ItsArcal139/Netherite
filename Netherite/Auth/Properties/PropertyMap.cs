using Netherite.Net.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Netherite.Auth.Properties
{
    public class PropertyMap
    {
        [JsonProperty("properties")]
        public List<Property> Properties { get; set; } = new List<Property>();
    }

    public class Property
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonIgnore]
        public bool IsSigned => Signature != null;

        public async Task<byte[]> ToArrayAsync()
        {
            var isSigned = this.Signature != null;
            BufferWriter writer = new BufferWriter();
            writer.WriteString(Name);
            writer.WriteString(Value);
            writer.WriteBool(isSigned);
            if (isSigned)
                writer.WriteString(Signature);

            await Task.CompletedTask;
            return writer.ToBuffer();
        }

        public byte[] ToArray()
        {
            BufferWriter writer = new BufferWriter();
            writer.WriteString(Name);
            writer.WriteString(Value);
            writer.WriteBool(Signature != null);
            if (Signature != null)
                writer.WriteString(Signature);

            return writer.ToBuffer();
        }
    }
}
