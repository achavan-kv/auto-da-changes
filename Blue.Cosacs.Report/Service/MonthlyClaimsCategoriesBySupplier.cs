using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Blue.Cosacs.Report.Service;

namespace Blue.Cosacs.Report
{
    public partial class MonthlyClaimsCategoriesBySupplier
    {

        internal static IList<ClaimsProductCategoryResult> Fill(string finYear, string month, string supplier)
        {
            var ds = new DataSet();
            var ws = new MonthlyClaimsCategoriesBySupplier();
            var retList = new List<ClaimsProductCategoryResult>();

            ws.Fill(ds, finYear, month, supplier);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new ClaimsProductCategoryResult()
                    {
                        Name = p["Category"].ToString(),
                        CurrentMonthParts = Convert.ToDecimal(p["Parts"]),
                        CurrentMonthLabour = Convert.ToDecimal(p["Labour"]),
                        YearToDateParts = Convert.ToDecimal(p["PartsYtd"]),
                        YearToDateLabour = Convert.ToDecimal(p["LabourYtd"])
                    })
                    .ToList();
            }

            return retList;
        }

    }
}
