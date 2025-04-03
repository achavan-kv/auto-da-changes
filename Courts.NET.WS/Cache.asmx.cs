using Blue.Cosacs.Repositories;
using System;
using System.Web.Services;

namespace STL.WS
{
    public class Cache
    {
        [WebMethod]
        public DateTime? Get(string key)
        {
            var table = key.Split('-')[0];
            return new CacheRepository().CheckChange(table);
        }
    }
}