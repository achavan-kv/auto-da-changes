using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Net
{
    public class DefaultServiceClientAsync : IServiceClientAsync
    {
        public DefaultServiceClientAsync() : this(STL.Common.Static.Config.Url) { }

        public DefaultServiceClientAsync(string baseUrl)
        {
            client = new JsonServiceClient(baseUrl);
        }

        private readonly JsonServiceClient client;

        public void Call<TRequest, TResponse>(TRequest request, Action<TResponse> onSuccess)
        {
            client.Call<TRequest, TResponse>(GetUrl<TRequest>(), request, onSuccess, null);
        }

        public void Call<TRequest, TResponse>(TRequest request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
        {
            client.Call<TRequest, TResponse>(GetUrl<TRequest>(), request, onSuccess, onError);
        }

        private string GetUrl<TRequest>()
        {
            var type = typeof(TRequest);
            var name = string.Format("{0}.{1}", type.Namespace, type.Name);
            return "servicestack.ashx/json/syncreply/" + name;
        }
    }
}
