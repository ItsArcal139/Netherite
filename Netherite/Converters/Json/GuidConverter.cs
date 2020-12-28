using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Converters.Json
{
    public class GuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Guid).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Guid.Parse(reader.ReadAsString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Guid g = (Guid)value;
            writer.WriteValue(g.ToString());
        }
    }
}
