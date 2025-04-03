using System.Collections.Generic;
using System.Net;
using Blue.Cosacs.SalesManagement.Hub.Subscribers;
using Blue.Networking;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.SalesManagement
{
    public static class ExternalHttpSources
    {
        //this use to had a more generic purpose, but now is a bit redundant  
        public static T Post<T>(string url, IHttpClientJson httpClientJson) where T : class, new()
        {
            return Post<T>(url, httpClientJson, null);
        }

        public static T Post<T>(string url, IHttpClientJson httpClientJson, int? timeout) where T : class, new()
        {
            return Do<T>(url, httpClientJson, WebRequestMethods.Http.Post, timeout);
        }

        public static T Get<T>(string url, IHttpClientJson httpClientJson) where T : class, new()
        {
            return Get<T>(url, httpClientJson, null);
        }

        public static T Get<T>(string url, IHttpClientJson httpClientJson, int? timeout) where T : class, new()
        {
            return Do<T>(url, httpClientJson, WebRequestMethods.Http.Get, timeout);
        }

        private static T Do<T>(string url, IHttpClientJson httpClientJson, string method, int? timeout) where T : class, new()
        {
            var request = RequestJson<byte[]>.Create(url, method);

            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }

            var result = httpClientJson.Do<byte[], T>(request);

            return result.Body;
        }

        public static IList<Customer> GetCustomer(IHttpClientJson httpClientJson, string customerAccount = "", string[] customerId = null)
        {
            return ExternalHttpSources.Post<List<Customer>>(string.Format("/Courts.NET.WS/SalesManagement/GetCustomer?accountNuber={0}&{1}", customerAccount, ToQueryString(customerId, "customerId")), httpClientJson);
        }

        private static string ToQueryString(string[] data, string parameter)
        {
            if (data != null && data.Any())
            {
                var result = new StringBuilder();

                foreach (var item in data.Distinct())
                {
                    result.Append(parameter).Append("=").Append(System.Uri.EscapeDataString(item)).Append("&");
                }

                result.Length = result.Length - 1;

                return result.ToString();
            }

            return string.Empty;
        }
    }
}
