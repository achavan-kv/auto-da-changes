using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Blue.Cosacs.SalesManagement
{
    [Serializable]
    public sealed class CustomersLastPurchase
    {
        internal IList<CustomersLastPurchase> GetData(DateTime howOldCash, DateTime beginningOfRangeCash, DateTime howOldCredit, DateTime beginningOfRangeCredit, int numberOfRecordsToReturn)
        {
            using (var ctx = Context.Create())
            {
                var ds = new GetCustomersLastPurchase
                {
                    beginningOfRangeCash = beginningOfRangeCash,
                    beginningOfRangeCredit = beginningOfRangeCredit,
                    howOldCash = howOldCash,
                    howOldCredit = howOldCredit,
                    numberOfRecordsToReturn = numberOfRecordsToReturn
                }.ExecuteDataSet();

                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0].Rows.OfType<DataRow>()
                        .Select(p => new CustomersLastPurchase
                        {
                            CashAccount = (bool)p["CashAccount"],
                            CustomerId = p["CustomerId"].ToString(),
                            FirstName = p["FirstName"].ToString(),
                            LastName = p["LastName"].ToString(),
                            DateLastPaid = (DateTime)p["DateLastPaid"],
                            SalesPerson = (int)p["SalesPerson"],
                            CustomerBranch = (short)p["CustomerBranch"],
                            Email = p["Email"] == DBNull.Value ? string.Empty : p["Email"].ToString(),
                            MobileNumber = p["MobileNumber"] == DBNull.Value ? string.Empty : p["MobileNumber"].ToString(),
                            LandLinePhone = p["LandLinePhone"] == DBNull.Value ? string.Empty : p["LandLinePhone"].ToString()
                        })
                        .ToList();
                }
            }

            return new List<CustomersLastPurchase>();
        }

        public string CustomerId
        {
            get;
            set;
        }

        public DateTime DateLastPaid
        {
            get;
            set;
        }
        public int SalesPerson
        {
            get;
            set;
        }
        public string FirstName
        {
            get;
            set;
        }
        public string LastName
        {
            get;
            set;
        }
        public bool CashAccount
        {
            get;
            set;
        }
        public short CustomerBranch { get; set; }
        public string MobileNumber { get; set; }
        public string LandLinePhone { get; set; }
        public string Email
        {
            get;
            set;
        }
    }
}
