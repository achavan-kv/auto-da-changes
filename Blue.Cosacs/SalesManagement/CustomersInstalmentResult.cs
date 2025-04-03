using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.SalesManagement
{
    public class CustomersInstalmentResult
    {
        public IList<CustomerInstalments> CustomersExactDate
        {
            get;
            set;
        }

        public IList<CustomerInstalments> CustomersWithinTheRange
        {
            get;
            set;
        }
    }
}
