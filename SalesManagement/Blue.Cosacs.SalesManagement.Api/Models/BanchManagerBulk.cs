using System;

namespace Blue.Cosacs.SalesManagement.Api.Models
{
    public class CustomerBulkInserCall
    {
        public string CustomerId { get; set; }
        public int SalesPersonId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string MobileNumber { get; set; }
        public string LandLinePhone { get; set; }
        public string Email { get; set; }
    }

    public class CustomerBulk
    {
        public CustomerBulkInserCall[] Customers { get; set; }
        public DateTime ToCallAt { get; set; }
        public string ReasonForCalling { get; set; }
    }
}