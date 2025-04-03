using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement
{
    public static class Consts
    {
        public const string RedisMainKey = "SalesM";
        public const string CallSummaryDashboardKey = "SalesM:CallSum";
        public const string UndeliveredAccountsDashboardKey = "SalesM:UndeliAcct";
        public const string SlowServiceRequestKey = "SalesM:SlowSr";
        public const string SalesSummaryDashboardKey = "SalesM:SalesSum";
        public const string NewCustomerAcquisitionKPIDashboardKey = "SalesM:CustomerAquisition";
        public const string CancellationKPIDashboardKey = "SalesM:Cancellations";
        public const string RewritesKPIDashboardKey = "SalesM:Rewrites";
        public const string WarrantyHitRateKPIDashboardKey = "SalesM:CsrWarrantyHitRate";
        public const string InstallationHitRateKPIDashboardKey = "SalesM:CsrInstallationHitRate";
        public const string CreditMixKPIDashboardKey = "SalesM:CreditMix";

        public const string BranchInstallationHitRateKPIDashboardKey = "SalesM:BranchInstallationHitRate";
        public const string BranchWarrantyHitRateKPIDashboardKey = "SalesM:BranchWarrantyHitRate";
    }
}
