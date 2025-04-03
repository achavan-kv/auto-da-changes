using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    public class CustomerLastPurchase
    {
        public string CustomerId
        {
            get;
            set;
        }
        private DateTime dateLastPaid;
        public DateTime DateLastPaid
        {
            get
            {
                return this.dateLastPaid.ToLocalTime();
            }
            set
            {
                this.dateLastPaid = value;
            }
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
        public string MobileNumber { get; set; }
        public string LandLinePhone { get; set; }
        public short CustomerBranch { get; set; }
        public string Email
        {
            get;
            set;
        }
    }
}