using System;
using System.Data;
using Newtonsoft.Json;

namespace Blue.Cosacs.Shared.Net
{
    public class JsonDataSetConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dataSet = (DataSet)value;
            var converter = new JsonDataTableConverter();

            writer.WriteStartObject();

            foreach (DataTable table in dataSet.Tables)
            {
                writer.WritePropertyName(table.TableName);
                converter.WriteJson(writer, table, serializer);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var ds = new DataSet();
            var converter = new JsonDataTableConverter();
            reader.Read();

            while (reader.TokenType == JsonToken.PropertyName)
            {
                var dt = (DataTable)converter.ReadJson(reader, typeof(DataTable), null, serializer);
                ds.Tables.Add(dt);
                reader.Read();
            }

            return ds;
        }

        public override bool CanConvert(Type valueType)
        {
            return (valueType == typeof(DataSet));
        }
    }
}
