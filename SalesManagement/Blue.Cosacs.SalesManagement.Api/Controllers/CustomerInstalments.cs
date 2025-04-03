using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    public partial class CustomerInstalments
    {
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
        private DateTime? lastInstalmentDate;
        public DateTime? LastInstalmentDate
        {
            get
            {
                if (this.lastInstalmentDate.HasValue)
                {
                    return this.lastInstalmentDate.Value.ToLocalTime();
                }

                return this.lastInstalmentDate;
            }
            set
            {
                this.lastInstalmentDate = value;
            }
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