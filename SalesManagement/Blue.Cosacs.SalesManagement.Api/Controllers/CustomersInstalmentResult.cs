using System.Collections.Generic;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
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