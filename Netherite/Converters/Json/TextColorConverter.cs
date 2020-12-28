using Netherite.Texts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Converters.Json
{
    public class TextColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(TextColor).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string name = reader.ReadAsString();
            return TextColor.Of(name);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TextColor color = (TextColor)value;
            writer.WriteValue(color.Name);
        }
    }
}
