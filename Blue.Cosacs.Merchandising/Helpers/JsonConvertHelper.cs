namespace Blue.Cosacs.Merchandising.Helpers
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public static class JsonConvertHelper
    {
        public static T DeserializeObject<T>(string value) where T : class
        {
            return string.IsNullOrEmpty(value)
                ? null
                : JsonConvert.DeserializeObject<T>(value);
        }

        public static object DeserializeObject(string value, Type type)
        {
            return string.IsNullOrEmpty(value)
                ? null
                : JsonConvert.DeserializeObject(value, type);
        }

        public static T DeserializeObjectOrDefault<T>(string value) where T: new()
        {
            return string.IsNullOrEmpty(value) 
                ? new T()
                : JsonConvert.DeserializeObject<T>(value);
        }

        public static object DeserializeObjectOrDefault(string value, Type type)
        {
            return string.IsNullOrEmpty(value)
                ? Activator.CreateInstance(type)
                : JsonConvert.DeserializeObject(value, type);
        }

        public static string Serialize<T>(T value) where T : new()
        {
            return EqualityComparer<T>.Default.Equals(value, default(T)) 
                ? null
                : JsonConvert.SerializeObject(value);
        }

        public static string SerializeObject(object value)
        {
            return value == null
                ? null
                : JsonConvert.SerializeObject(value);
        }
    }
}
