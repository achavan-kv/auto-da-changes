using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using STL.Common;
using STL.DAL;


namespace STL.WS
{
    public static class CachedItems
    {
        public static CountryParameterCollection GetCountryParamters(string countryCode)
        {
            var cachedCountryParams = HttpContext.Current.Cache[CountryParameterCollection.CountryCacheKey(countryCode)];

            if (cachedCountryParams != null && cachedCountryParams is CountryParameterCollection)
                return (CountryParameterCollection)cachedCountryParams;
            else
                return (CountryParameterCollection)
                    (HttpContext.Current.Cache[CountryParameterCollection.CountryCacheKey(countryCode)] =
                        new CountryParameterCollection(new DCountry().GetMaintenanceParameters(null, null, countryCode)));
        }
    }
}