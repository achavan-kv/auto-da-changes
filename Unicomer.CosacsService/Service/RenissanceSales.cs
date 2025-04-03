using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STL.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Unicomer.CosacsService
{
    public class RenissanceSales
    {
        public RenissanceSales() { }
        public string GetRenissanceSaleData()
        {
            DRenissanceSale dr = new DRenissanceSale();
            DataSet dsRenissance = dr.GetRenissanceSalesDetails(null, null);
            string strJson = string.Empty;

            if (dsRenissance.Tables[0].Rows.Count > 0)
            {
                strJson = JsonConvert.SerializeObject(dsRenissance, Formatting.Indented);
            }

            return strJson;
        }
        public void ReceiveRenissanceSaleData()
        {
            DRenissanceSale dr = new DRenissanceSale();
            dr.ReceiveRenissanceSaleDataFlag(null, null);
        }

    }
}
