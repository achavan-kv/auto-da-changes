using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.SalesManagement.Hub.Subscribers
{
    [Serializable]
    public sealed partial class Customer
    {
        public int SalesPerson { get; set; }
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string LandLinePhone { get; set; }
        public string CustomerAccount { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public bool ReceiveSms { get; set; }
        //public bool ReceiveEmails { get; set; }
    }
}
