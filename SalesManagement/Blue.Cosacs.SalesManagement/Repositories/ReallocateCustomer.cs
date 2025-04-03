using Blue.Cosacs.SalesManagement.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.SalesManagement.Repositories
{
    public class CustomerToAllocate
    {
        public string CustomerId { get; set; }
        public int SalesPersonId { get; set; }
        public DateTime? AllocateFrom { get; set; }
        public DateTime? AllocateTo { get; set; }
        public string MobileNumber { get; set; }
        public string MobileExtension { get; set; }
        public string MobileDialCode { get; set; }
        public string LandLinePhone { get; set; }
        public string LandLineExtension { get; set; }
        public string LandLineDialCode { get; set; }
        public int CSRId { get; set; }
    }

    public class ReallocateCustomer
    {
        public CustomerToAllocate[] Customers { get; set; }
    }
}