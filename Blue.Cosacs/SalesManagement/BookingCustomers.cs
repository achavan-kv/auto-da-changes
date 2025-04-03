using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.SalesManagement
{
    public class BookingCustomers
    {
        public string CustomerId { get; set; }
        public DateTime OrderedOn { get; set; }
        public DateTime? DeliveryConfirmedDate { get; set; }
    }
}
