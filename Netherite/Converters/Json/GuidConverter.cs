using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Converters.Json
{
    public class GuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(Guid) == objectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            Guid.Parse(reader.ReadAsString() ?? string.Empty);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Guid g = (Guid)value;
            writer.WriteValue(g.ToString());
        }
    }
}
