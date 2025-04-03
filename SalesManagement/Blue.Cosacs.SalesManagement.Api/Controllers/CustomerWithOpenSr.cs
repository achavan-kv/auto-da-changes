using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    public sealed class CustomerWithOpenSr
    {
        public int RequestId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public Contact[] Contact { get; set; }
        public string State { get; set; }
        public string Item { get; set; }
        public short Branch { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class Contact
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}