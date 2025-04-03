using System;

namespace Blue.Cosacs.Sales.Models
{
    [Serializable]
    public class CustomerDto
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PostCode { get; set; }
        public string Title { get; set; }
        public string TownOrCity { get; set; }
        public string CustomerId { get; set; }
        public string Email { get; set; }
        public string HomePhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public bool IsSalesCustomer { get; set; }
    }
}
