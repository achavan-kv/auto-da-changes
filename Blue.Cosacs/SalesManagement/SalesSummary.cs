using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Blue.Cosacs.SalesManagement
{
    [Serializable]
    public sealed class SalesSummary
    {
        internal static IList<SalesSummary> GetData<T>(T source) where T : Blue.Cosacs.Procedure
        {
            using (var ctx = Context.Create())
            {
                return source.ExecuteDataSet().Tables[0].Rows.OfType<DataRow>()
                .Select(p => new SalesSummary
                {
                    SalesPerson = (int)p["SalesPerson"],
                    Amount = p["Amount"] == DBNull.Value ? 0M : decimal.Parse(p["Amount"].ToString()),
                    Area = p["Area"].ToString(),
                    BranchNo = (short)p["BranchNo"]
                })
                .ToList();
            }
        }

        public int SalesPerson { get; set; }
        public string Area { get; set; }
        public decimal Amount { get; set; }
        public short BranchNo { get; set; }
    }
}
