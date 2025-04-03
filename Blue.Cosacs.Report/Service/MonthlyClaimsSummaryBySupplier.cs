using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Blue.Cosacs.Report.Service;

namespace Blue.Cosacs.Report
{
    public partial class MonthlyClaimsSummaryBySupplier
    {

        internal static IList<ClaimsSupplierResult> Fill(string finYear, string month, string supplier)
        {
            var ds = new DataSet();
            var ws = new MonthlyClaimsSummaryBySupplier();
            var retList = new List<ClaimsSupplierResult>();

            ws.Fill(ds, finYear, month, supplier);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new ClaimsSupplierResult()
                    {
                        Name = p["Supplier"].ToString(),
                        CurrentMonth = Convert.ToDecimal(p["CurrentMonth"]),
                        YearToDate = Convert.ToDecimal(p["YearToDate"])
                    })
                    .ToList();
            }

            foreach (var supplierResult in retList)
            {
                supplierResult.ClaimsProductCategories = MonthlyClaimsCategoriesBySupplier.Fill(finYear, month, supplierResult.Name);
            }
       
            return retList;
        }

    }
}
