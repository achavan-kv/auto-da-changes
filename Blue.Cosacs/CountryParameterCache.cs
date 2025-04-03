using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using STL.Common;
using STL.DAL;


namespace Blue.Cosacs
{
    public static class CountryParameterCache
    {
        public static ICache Cache { get; set; }
        static CountryParameterCache()
        {
            Cache = new Cache();
        }

        public static CountryParameterCollection GetCountryParamters()
        {
            var countryCode = GetCountryCode();

            var cachedCountryParams = Cache.Get(CountryParameterCollection.CountryCacheKey(countryCode));

            if (cachedCountryParams != null && cachedCountryParams is CountryParameterCollection)
                return (CountryParameterCollection)cachedCountryParams;
            else
                return (CountryParameterCollection)
                    (Cache.Insert(CountryParameterCollection.CountryCacheKey(countryCode),
                        new CountryParameterCollection(new DCountry().GetMaintenanceParameters(null, null, countryCode))));
        }

        public static T GetCountryParameter<T>(string name)
        {
            return GetCountryParamters().GetCountryParameterValue<T>(name);
        }

        static string countryCode;

        private static string GetCountryCode()
        {
            if (countryCode == null)
            {
                using (var ctx = Context.Create())
                {
                    countryCode = ctx.Country.First().countrycode;
                }
            }

            return countryCode.ToString();
        }
    }

    public interface ICache
    {
        object Get(string key);
        object Insert(string key, object item);
    }

    public class Cache : ICache
    {
        #region ICache Members

        public object Get(string key)
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Cache[key];
            else
                return "";
        }

        public object Insert(string key, object value)
        {
            return HttpContext.Current.Cache[key] =  value;
        }

        #endregion
    }
}