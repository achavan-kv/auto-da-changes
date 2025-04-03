using System.Collections.Generic;

namespace Blue.Cosacs.Report.Service
{
    public class ReportSqlService
    {
        public IEnumerable<ClaimsSupplierResult> GetMonthlyClaimsSummaryBySupplier(string finYear, string month, string supplier)
        {
            return MonthlyClaimsSummaryBySupplier.Fill(finYear, month, supplier);
        }

        public IEnumerable<OutstandingSRsPerProductCategoryResult>
            GetOutstandingSRsPerProductCategory(OutstandingSRsPerProductCategoryFilter searchFilter)
        {
            return OutstandingSRsPerProductCategory.Fill(searchFilter);
        }

        public IEnumerable<ServiceClaimsResult> GetServiceClaims(string dateLoggedFrom, string dateLoggedTo, string dateResolvedFrom, string dateResolvedTo,
                                                                    string supplier, string primaryCharge, string department,
                                                                    bool includeTechnicianReport, bool supplierCharged, bool fywCharged, bool ewCharged,
                                                                    short pageNumber, short pageSize)
        {

            return ServiceClaims.Fill(dateLoggedFrom, dateLoggedTo, dateResolvedFrom, dateResolvedTo,
                                        supplier, primaryCharge, department,
                                        includeTechnicianReport, supplierCharged, fywCharged, ewCharged, pageNumber, pageSize);
        }
    }
}
