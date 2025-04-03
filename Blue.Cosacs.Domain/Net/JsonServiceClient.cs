using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Blue.Cosacs.Shared.Net
{
    public class JsonServiceClient 
    {
        public const string ContentType = "application/json";
        public string BaseUri { get; set; }
        private readonly AsyncServiceClient client;

        public JsonServiceClient(string baseUri): this()
        {
            this.BaseUri = WithTrailingSlash(baseUri);
        }

        public JsonServiceClient()
        {
            this.client = new AsyncServiceClient
            {
                ContentType = ContentType,
                //StreamSerializer = JsonSerializer.SerializeToStream,
                //StreamDeserializer = JsonSerializer.DeserializeFromStream
                StreamDeserializer = JsonNetStreamDeserializer,
                StreamSerializer = JsonNetStreamSerializer
            };
        }

        private static string WithTrailingSlash(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (path[path.Length - 1] != '/')
                return path + "/";

            return path;
        }

        private static void JsonNetStreamSerializer(object dto, Stream toStream)
        {
            using (var writer = new StreamWriter(toStream))
                CreateSerializer().Serialize(writer, dto);
        }

        public static Newtonsoft.Json.JsonSerializer CreateSerializer()
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Converters.Insert(0, new JsonDataSetConverter());
            serializer.Converters.Insert(0, new JsonDataTableConverter());
            return serializer;
        }

        private static object JsonNetStreamDeserializer(Type type, Stream fromStream)
        {
            using (var reader = new StreamReader(fromStream))
                return CreateSerializer().Deserialize(reader, type);
        }

        private string GetUrl(string relativeOrAbsoluteUrl)
        {
            return relativeOrAbsoluteUrl.StartsWith("http:")
                || relativeOrAbsoluteUrl.StartsWith("https:")
                     ? relativeOrAbsoluteUrl
                     : this.BaseUri + relativeOrAbsoluteUrl;
        }

        public void Call<TRequest, TResponse>(string relativeOrAbsoluteUrl, TRequest request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            this.client.SendAsync(GetUrl(relativeOrAbsoluteUrl), request, onSuccess, onError);
        }
    }
}
