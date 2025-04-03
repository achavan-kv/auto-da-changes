using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;

namespace Blue.Cosacs.SalesManagement
{
    public partial class UndeliveredCashCreditBranch
    {
        internal static List<UndeliveredCashCreditBranch> GetData(short branchNo)
        {
            using (var ctx = Context.Create())
            {
                var ds = new UndeliveredCashCreditPerBranch
                {
                    BranchNo = branchNo
                }.ExecuteDataSet();

                return ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new UndeliveredCashCreditBranch
                            {
                                CustomerAccount = p["CustomerAccount"].ToString(),
                                CustomerId = p["CustomerId"].ToString(),
                                CustomerFirstName = p["CustomerFirstName"].ToString(),
                                CustomerLastName = p["CustomerLastName"].ToString(),
                                ItemDescription = p["ItemDescription"].ToString(),
                                ItemNo = p["ItemNo"].ToString(),
                                MobileNumber = p["MobileNumber"] == DBNull.Value ? string.Empty : p["MobileNumber"].ToString(),
                                LandLinePhone = p["LandLinePhone"] == DBNull.Value ? string.Empty : p["LandLinePhone"].ToString(),
                                StatusDescription = p["StatusDescription"].ToString(),
                                DeliveryDate = p["DeliveryDate"] == DBNull.Value ? (DateTime?)null : new Nullable<DateTime>((DateTime)p["DeliveryDate"])
                            })
                    .ToList();
            }
        }

        public string CustomerAccount { get; set; }
        public string CustomerId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string ItemNo { get; set; }
        public string ItemDescription { get; set; }
        public string StatusDescription { get; set; }
        public string MobileNumber { get; set; }
        public string LandLinePhone { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}
