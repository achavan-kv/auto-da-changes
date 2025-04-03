using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using Blue.Cosacs.Repositories;
using System.Globalization;


namespace Blue.Cosacs
{
    public static class MainCache
    {

        public static void SetCulture(HttpContext req)
        {
            if (req.Cache["Culture"] == null)
            {
                var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

                var countrymaintenance = new ConfigRepository().CultureCode();
                
                var culturename = countrymaintenance.Where(c => c.CodeName == "Culture").First().Value;
                var culture = cultures.Where(c => c.DisplayName == culturename).FirstOrDefault();
                var currencysymbol = countrymaintenance.Where(c => c.CodeName == "currencysymbolforprint").First().Value;
                
                if (culture == null)
                {
                    culture = new CultureInfo("en");
                }

                if (!string.IsNullOrEmpty(currencysymbol))
                {
                    culture.NumberFormat.CurrencySymbol = currencysymbol;
                }

                req.Cache["Culture"] = culture;

            }
        }

   
    }
}
