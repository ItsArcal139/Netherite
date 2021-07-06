using Netherite.Texts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Converters.Json
{
    public class TextColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(TextColor) == objectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            TextColor.Of(reader.ReadAsString() ?? "gray");

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TextColor color = (TextColor)value;
            writer.WriteValue(color.Name);
        }
    }
}
