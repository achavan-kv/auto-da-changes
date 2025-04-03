using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class CustomerResult : Customer
    {
        public bool blacklisted { get; set; }

        public List<CustAddress> CustAddress { get; set; }

        public class Parameters
        {
            public class Search
            {
                public Search()
                {
                    // Default Search Parameters
                    Address = "";
                    CustomerId = "";
                    FirstName = "";
                    Surname = "";
                    PhoneNumber = "";
                    StoreType = "%";
                    Limit = 50;
                }
                public string Address { get; set; }
                public string CustomerId { get; set; }
                public string FirstName { get; set; }
                public string Surname { get; set; }
                public string PhoneNumber { get; set; }
                public int Limit { get; set; }
                public string StoreType { get; set; }
            }

            public class AccountAssotiation
            {
                public string CustomerId { get; set; }
                public string CountryCode { get; set; }
                public short BranchNo { get; set; }
            }
        }
    }

    public class CustomerComparer : IEqualityComparer<Customer>
    {

        #region IEqualityComparer<Customer> Members

        public bool Equals(Customer x, Customer y)
        {
            return x.custid == y.custid;
        }

        public int GetHashCode(Customer obj)
        {
            var hcode = string.Format("{0}:{1}:{2}:{3}:{4}", obj.custid, obj.title, obj.firstname, obj.name, obj.alias);
            return hcode.GetHashCode();
        }

        #endregion
    }
}
