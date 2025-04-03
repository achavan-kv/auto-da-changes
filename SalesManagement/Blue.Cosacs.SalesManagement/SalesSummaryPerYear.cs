using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Blue.Cosacs.SalesManagement
{
    public partial class SalesSummaryPerYear
    {
        public static IList<SalesSummaryPerYear> GetData(DateTime todaysDate)
        {
            var ds = new DataSet();//shame on me...Dataset really?

            new SalesSummaryPerYear().Fill(ds, todaysDate);

            return ds.Tables[0].Rows
                .OfType<DataRow>()
                .Select(p => new SalesSummaryPerYear
                {
                    Area = p["Matrix"].ToString(),
                    BranchNo = (short)p["BranchNo"],
                    Month = (decimal)p["Month"],
                    SalesPerson = (int)p["SalesPersonId"],
                    Today = (decimal)p["Today"],
                    Week = (decimal)p["Week"],
                    Year = (decimal)p["Week"]
                })
                .ToList();
        }

        public int SalesPerson { get; set; }
        public decimal Today { get; set; }
        public decimal Week { get; set; }
        public decimal Month { get; set; }
        public decimal Year { get; set; }
        public short BranchNo { get; set; }
        public string Area { get; set; }
    }
}
