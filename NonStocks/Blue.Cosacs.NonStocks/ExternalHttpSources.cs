using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Blue.Networking;
using System.Collections.Specialized;

namespace Blue.Cosacs.NonStocks
{
    public static class ExternalHttpSources
    {
        public static T Get<T>(string url, IHttpClientJson httpClientJson) where T : class, new()
        {
            var result = httpClientJson.Do<byte[], T>(RequestJson<byte[]>.Create(url, WebRequestMethods.Http.Post));

            return result.Body;
        }

        public static T Get<T>(string url, string body, IHttpClientJson httpClientJson) where T : class, new()
        {
            var tmpRequest = RequestJson<byte[]>.Create(url, WebRequestMethods.Http.Post);

            if (body != null && body.Length > 0)
            {
                tmpRequest.Body = Encoding.UTF8.GetBytes(body);
            }
            var result = httpClientJson.Do<byte[], T>(tmpRequest);

            return result.Body;
        }
    }
}
