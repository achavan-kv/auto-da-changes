using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STL.Common;
using STL.DAL;

namespace Blue.Cosacs
{
    public class InitCountryParamCache: CommonObject
    {
        public void PopulateCacheCountryParams(string country)
        {
            if (Cache["Country"] == null)
            {
                var da = new DCountry();
                var dt = da.GetMaintenanceParameters(null, null, country);

                if (dt.Rows.Count > 0)
                {
                    Cache["Country"] = new CountryParameterCollection(dt);
                    StockItemCache.Invalidate(new Blue.Cosacs.Repositories.StockRepository().GetStockItemCache());
                }
            }
        }
    }
}
