using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.SalesManagement
{
    public sealed class SalesKPI
    {
        private static SalesKpiDto<T> GetData<T>(DataRow row, Func<object, T> parser)
        {
            return new SalesKpiDto<T>
                       {
                           CSR = int.Parse(row["CSR"].ToString()),
                           BranchNo = int.Parse(row["BranchNo"].ToString()),
                           FirstWeek = (DateTime)row["FirstWeek"],
                           Total = parser(row["Total"]),
                           WeekNo = int.Parse(row["WeekNo"].ToString())
                       };
        }

        internal static Hashtable GetSalesKPIData(DateTime today)
        {
            var returnValue = new Hashtable();

            //CsrWarrantyHitRate
            using (var ctx = Context.Create())
            {
                var warrantyByCsr = new CsrWarrantyHitRate
                {
                    Today = today,
                    CalculatePerCsr = true
                }.ExecuteDataSet();

                var warrantyDataByCsr = warrantyByCsr.Tables[0].Rows.OfType<DataRow>()
                        .Select(p => GetData<decimal>(p, value => Convert.ToDecimal(value)))
                        .ToList();

                if (warrantyDataByCsr.Any())
                {
                    returnValue.Add("SalesM:SalesKpiFirstWeek", warrantyDataByCsr.First().FirstWeek);
                }
                else
                {
                    returnValue.Add("SalesM:SalesKpiFirstWeek", null);
                }

                returnValue.Add("SalesM:CsrWarrantyHitRate", warrantyDataByCsr);

                var warrantyByBranch = new CsrWarrantyHitRate
                {
                    Today = today,
                    CalculatePerCsr = false
                }.ExecuteDataSet();

                var warrantyDataByBranch = warrantyByBranch.Tables[0].Rows.OfType<DataRow>()
                        .Select(p => GetData<decimal>(p, value => Convert.ToDecimal(value)))
                        .ToList();

                returnValue.Add("SalesM:BranchWarrantyHitRate", warrantyDataByBranch);

                //CsrInstallationHitRate
                var installationByCSR = new CsrInstallationHitRate
                {
                    Today = today,
                    CalculatePerCsr = true
                }.ExecuteDataSet();

                var installationDataByCSR = installationByCSR.Tables[0].Rows.OfType<DataRow>()
                        .Select(p => GetData<decimal>(p, value => Convert.ToDecimal(value)))
                        .ToList();

                returnValue.Add("SalesM:CsrInstallationHitRate", installationDataByCSR);

                var installationByBranch = new CsrInstallationHitRate
                {
                    Today = today,
                    CalculatePerCsr = false
                }.ExecuteDataSet();

                var instalationDataByBranch = installationByBranch.Tables[0].Rows.OfType<DataRow>()
                        .Select(p => GetData<decimal>(p, value => Convert.ToDecimal(value)))
                        .ToList();

                returnValue.Add("SalesM:BranchInstallationHitRate", instalationDataByBranch);

                // New Customer Acquisition
                var newCustomerAcquisition = new NewCustomerAquisition
                {
                    Today = today
                }.ExecuteDataSet();

                var newCustomerAcquisitionData = newCustomerAcquisition.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => GetData<int>(p, value => int.Parse(value.ToString())))
                    .ToList();

                returnValue.Add("SalesM:CustomerAquisition", newCustomerAcquisitionData);

                //Cancellations
                var cancellations = new GetCancellations
                {
                    Today = today
                }.ExecuteDataSet();

                var cancellationsData = cancellations.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => GetData<int>(p, value => int.Parse(value.ToString())))
                .ToList();

                returnValue.Add("SalesM:Cancellations", cancellationsData);

                //Rewrites
                var rewrites = new GetRewrites
                {
                    Today = today
                }.ExecuteDataSet();

                var rewritesData = rewrites.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => GetData<decimal>(p, value => Convert.ToDecimal(value)))
                        .ToList();

                returnValue.Add("SalesM:Rewrites", rewritesData);

                //creditMix
                var creditMix = new CsrCreditMix
                {
                    Today = today
                }.ExecuteDataSet();

                var creditMixData = creditMix.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => GetData<decimal>(p, value => Convert.ToDecimal(value)))
                        .ToList();

                returnValue.Add("SalesM:CreditMix", creditMixData);
            }

            return returnValue;
        }
    }
}
