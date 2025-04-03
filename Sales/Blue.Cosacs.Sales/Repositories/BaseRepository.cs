using System.Net;
using Blue.Networking;

namespace Blue.Cosacs.Sales.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly IClock Clock;
        protected readonly IHttpClient HttpClient;

        protected BaseRepository(IClock clock, IHttpClient httpClient)
        {
            Clock = clock;
            HttpClient = httpClient;
        }

        protected T GetRemoteData<T>(string url, int userId) where T : class, new()
        {
            var jsonClient = new HttpClientJsonAuth(HttpClient, Clock, userId.ToString());
            var request = RequestJson<byte[]>.Create(url, WebRequestMethods.Http.Get);
            var ret = jsonClient.Do<byte[], T>(request).Body;

            return ret;
        }

        protected T PostRemoteData<T>(string url, int userId) where T : class, new()
        {
            var jsonClient = new HttpClientJsonAuth(HttpClient, Clock, userId.ToString());
            var request = RequestJson<byte[]>.Create(url, WebRequestMethods.Http.Post);
            var ret = jsonClient.Do<byte[], T>(request).Body;

            return ret;
        }
    }
}
