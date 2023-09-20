using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApplication1.Converter
{
    public class DictionaryConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>>
    {
        public override Dictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> dictionary, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var kvp in dictionary)
            {
                writer.WritePropertyName(kvp.Key.ToString());
                JsonSerializer.Serialize(writer, kvp.Value, options);
            }

            writer.WriteEndObject();
        }
    }
}

