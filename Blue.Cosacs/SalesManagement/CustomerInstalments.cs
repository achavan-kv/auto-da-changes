using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.SalesManagement
{
    public partial class CustomerInstalments
    {
        public IList<CustomerInstalments> GetData(DateTime from, DateTime to)
        {
            var ds = new Blue.Cosacs.CustomersInstalments
            {
                From = from,
                To = to
            }.ExecuteDataSet();

            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0].Rows.OfType<DataRow>()
                    .Select(p => new CustomerInstalments
                    {
                        CustomerBranch = (short)p["CustomerBranch"],
                        CustomerId = p["CustomerId"].ToString(),
                        Email = p["Email"] == DBNull.Value ? string.Empty : p["Email"].ToString(),
                        FirstName = p["FirstName"].ToString(),
                        LastInstalmentDate = p["LastInstalmentDate"] == DBNull.Value ? null : new Nullable<DateTime>((DateTime)p["LastInstalmentDate"]),
                        LastName = p["LastName"].ToString(),
                        MobileNumber = p["MobileNumber"] == DBNull.Value ? null : p["MobileNumber"].ToString(),
                        LandLinePhone = p["LandLinePhone"] == DBNull.Value ? null : p["LandLinePhone"].ToString(),
                        SalesPerson = (int)p["SalesPerson"],
                        AccountNumber = p["AccountNumber"].ToString()
                    })
                    .ToList();
            }

            throw new Exception();
        }

        public string CustomerId
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
        public DateTime? LastInstalmentDate
        {
            get;
            set;
        }
        public int SalesPerson
        {
            get;
            set;
        }
        public string MobileNumber { get; set; }
        public string LandLinePhone { get; set; }
        public short CustomerBranch { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; set; }
    }
}
