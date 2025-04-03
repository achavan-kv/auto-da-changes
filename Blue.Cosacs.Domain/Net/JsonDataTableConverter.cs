using System;
using System.Data;
using Newtonsoft.Json;

namespace Blue.Cosacs.Shared.Net
{
    public class JsonDataTableConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var table = (DataTable)value;

            writer.WriteStartObject();
            {
                writer.WritePropertyName("Columns");

                // serialize metadata (column names)
                writer.WriteStartArray();

                foreach (DataColumn column in table.Columns)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("Name");
                    serializer.Serialize(writer, column.ColumnName);

                    writer.WritePropertyName("Type");
                    serializer.Serialize(writer, column.DataType.Name);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();

                writer.WritePropertyName("Rows");
                // deserialize data
                writer.WriteStartArray();

                foreach (DataRow row in table.Rows)
                {
                    writer.WriteStartArray();
                    foreach (DataColumn column in row.Table.Columns)
                        serializer.Serialize(writer, row[column]);
                    writer.WriteEndArray();
                }

                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DataTable dt;

            if (reader.TokenType == JsonToken.PropertyName)
            {
                dt = new DataTable((string)reader.Value);
                reader.Read();
            }
            else
            {
                dt = new DataTable();
            }

            reader.Read();

            // re-create columns
            if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && ((string)reader.Value) == "Columns")
            {
                reader.Read();

                while (reader.TokenType == JsonToken.StartArray)
                {
                    reader.Read();

                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        reader.Read();
                        string columnName = null;
                        Type columnType = null;
                        if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && ((string)reader.Value) == "Name")
                        {
                            reader.Read();
                            columnName = reader.Value.ToString();
                            reader.Read();
                        }

                        if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && ((string)reader.Value) == "Type")
                        {
                            reader.Read();
                            columnType = GetColumnDataType(reader.Value.ToString());
                            reader.Read();
                        }
                        if (columnName == null || columnType == null)
                            throw new JsonSerializationException("Invalid DataTable column definition.");

                        dt.Columns.Add(new DataColumn(columnName, columnType));
                        reader.Read();
                    }

                    reader.Read();
                }
            }

            if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && ((string)reader.Value) == "Rows")
            {
                reader.Read();

                if (reader.TokenType == JsonToken.StartArray)
                {
                    reader.Read();

                    // populate rows
                    while (reader.TokenType == JsonToken.StartArray)
                    {
                        reader.Read();
                        DataRow dr = dt.NewRow();

                        var i = 0;
                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            dr[i++] = reader.Value ?? DBNull.Value;
                            reader.Read();
                        }

                        dr.EndEdit();
                        dt.Rows.Add(dr);

                        reader.Read();
                    }
                    
                    reader.Read(); // EndArray
                }

                // reader.Read(); // EndObject
            }

            return dt;
        }

        private static Type GetColumnDataType(string dataType)
        {
            switch (dataType)
            {
                case "Int16":
                    return typeof(System.Int16);
                case "Int32":
                    return typeof(System.Int32);
                case "Int64":
                    return typeof(System.Int64);
                case "String":
                    return typeof(System.String);
                case "DateTime":
                    return typeof(System.DateTime);
                case "Decimal":
                    return typeof(System.Decimal);
                case "Boolean":
                    return typeof(System.Boolean);
                case "Double":
                    return typeof(System.Double);
                case "Single":
                    return typeof(System.Single);
                case "Byte":
                    return typeof(System.Byte);
                default:
                    throw new ArgumentException(string.Format("Data set column type '{0}' is not implemented for JSON deserialization.", dataType));
            }
        }

        public override bool CanConvert(Type valueType)
        {
            return (valueType == typeof(DataTable));
        }
    }
}
