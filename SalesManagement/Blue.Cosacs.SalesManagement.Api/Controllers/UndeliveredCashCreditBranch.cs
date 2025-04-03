using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    public class UndeliveredCashCreditBranch
    {
        public string CustomerAccount { get; set; }
        public string CustomerId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string ItemNo { get; set; }
        public string ItemDescription { get; set; }
        public string StatusDescription { get; set; }
        public string MobileNumber { get; set; }
        public string LandLinePhone { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}