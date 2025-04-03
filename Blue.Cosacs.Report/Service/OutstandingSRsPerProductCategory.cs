using System.Collections.Generic;
using System.Data;
using System.Linq;
using Blue.Cosacs.Report.Service;

namespace Blue.Cosacs.Report
{
    public partial class OutstandingSRsPerProductCategory
    {
        internal static IList<OutstandingSRsPerProductCategoryResult> Fill(OutstandingSRsPerProductCategoryFilter searchFilter)
        {
            var ds = new DataSet();
            var ws = new OutstandingSRsPerProductCategory();
            var retList = new List<OutstandingSRsPerProductCategoryResult>();

            ws.Fill(ds, searchFilter.DateFrom, searchFilter.DateTo, searchFilter.CurrentDate, searchFilter.Status,
                searchFilter.Supplier, searchFilter.Technician, searchFilter.WarrantyType);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                retList = ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new OutstandingSRsPerProductCategoryResult()
                    {
                        ProductCategory = p["ProductCategory"].ToString(),
                        DaysOutstandingBand01 = p["DaysOutstandingBand01"].ToString(),
                        DaysOutstandingBand02 = p["DaysOutstandingBand02"].ToString(),
                        DaysOutstandingBand03 = p["DaysOutstandingBand03"].ToString(),
                        DaysOutstandingBand04 = p["DaysOutstandingBand04"].ToString(),
                        ServiceRequestsBand01 = p["ServiceRequestsBand01"].ToString(),
                        ServiceRequestsBand02 = p["ServiceRequestsBand02"].ToString(),
                        ServiceRequestsBand03 = p["ServiceRequestsBand03"].ToString(),
                        ServiceRequestsBand04 = p["ServiceRequestsBand04"].ToString()
                    })
                    .ToList();
            }

            return retList;
        }
    }
}
