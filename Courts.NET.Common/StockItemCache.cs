using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace STL.Common
{
    public class StockItemCache
    {
        public static void Invalidate(DataTable dt)
        {
            if (dt == null)
            {
                StaticDataSingleton.Instance().Data["StockItem"] = null;
                return;
            }
            
            var dictionary = new Dictionary<string, int>();

            foreach(DataRow dr in dt.Rows)
                dictionary.Add(dr[0].ToString(), Convert.ToInt32(dr[1])); 
            
            StaticDataSingleton.Instance().Data["StockItem"] = dictionary;
        }

        public static void Invalidate(Dictionary<string, int> cache)
        {
            StaticDataSingleton.Instance().Data["StockItem"] = cache;
        }

        public static int Get(StockItemKey indexerKey)
        {
            var cacheStockItem = StaticDataSingleton.Instance().Data["StockItem"] as Dictionary<string, int>;

            if (cacheStockItem == null)
                return 0;  //TODO throw exception
                
            var key = "";

            if (indexerKey.IsCountryParam)
            {
                var cacheCountryParam = StaticDataSingleton.Instance().Data["Country"] as CountryParameterCollection;
                key = cacheCountryParam != null && cacheCountryParam.ContainsKey(indexerKey.Key) ?
                        Convert.ToString(cacheCountryParam[indexerKey.Key]) : "";
            }
            else
            {
                key = indexerKey.Key;
            }

            return cacheStockItem.ContainsKey(key.ToUpper()) ? cacheStockItem[key.ToUpper()] : 0;
        }
    }

    public static class StockItemKeys
    {
        public static StockItemKey AdminChargeItem         = new StockItemKey(CountryParameterNames.AdminChargeItem, countryParam: true);
        public static StockItemKey InsuranceChargeItem     = new StockItemKey(CountryParameterNames.InsuranceChargeItem, countryParam: true);
        public static StockItemKey NonInterestItem         = new StockItemKey(CountryParameterNames.NonInterestItem, countryParam: true);
        public static StockItemKey ServiceItemLabour       = new StockItemKey(CountryParameterNames.ServiceItemLabour, countryParam: true);
        public static StockItemKey ServiceItemPartsCourts  = new StockItemKey(CountryParameterNames.ServiceItemPartsCourts, countryParam: true);
        public static StockItemKey ServiceItemPartsOther   = new StockItemKey(CountryParameterNames.ServiceItemPartsOther, countryParam: true);
        public static StockItemKey Tier2DiscountItemNumber = new StockItemKey(CountryParameterNames.Tier2DiscountItemNumber, countryParam: true);
        public static StockItemKey DT                      = new StockItemKey("DT");
        public static StockItemKey STAX                    = new StockItemKey("STAX");
        public static StockItemKey ADDDR                   = new StockItemKey("ADDDR");
        public static StockItemKey ADDCR                   = new StockItemKey("ADDCR");
        public static StockItemKey SD                      = new StockItemKey("SD");
        public static StockItemKey REFINCR                 = new StockItemKey("REFINCR");
        public static StockItemKey REFINDR                 = new StockItemKey("REFINDR");
        public static StockItemKey RB                      = new StockItemKey("RB");
        public static StockItemKey LOAN                    = new StockItemKey("LOAN");

        public static IEnumerable<StockItemKey> AsEnumerable()
        {
            yield return AdminChargeItem;
            yield return InsuranceChargeItem; 
            yield return NonInterestItem;       
            yield return ServiceItemLabour;      
            yield return ServiceItemPartsCourts;
            yield return ServiceItemPartsOther; 
            yield return Tier2DiscountItemNumber;
            yield return DT;
            yield return ADDDR;
            yield return ADDCR;
            yield return SD;
            yield return STAX;
            yield return REFINCR;
            yield return REFINDR;
            yield return RB;
            yield return LOAN;                  
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct StockItemKey
    {
        private string key;
        private bool countryParam;

        public StockItemKey(string key, bool countryParam = false)
        {
            this.key = key;
            this.countryParam = countryParam;
        }

        public bool IsCountryParam
        {
            get { return countryParam; }
        }

        public string Key
        {
            get { return key ?? ""; }
        }

        public override string ToString()
        {
 	        return key ?? "";
        }
    }
}
