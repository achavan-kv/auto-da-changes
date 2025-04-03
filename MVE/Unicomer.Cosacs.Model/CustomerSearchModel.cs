using System;
using System.Collections.Generic;

namespace Unicomer.Cosacs.Model
{
    public class CustomerRequest
    {
        public string CustId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
    }

    public class Customer
    {
        public string ResourceType { get; set; }
        public string Source { get; set; }
        public string CustomerID { get; set; }
        public string IdType { get; set; }
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Title { get; set; }
        public int BranchNo { get; set; }
        public List<Contact> Contact { get; set; }
        public List<Address> Address { get; set; }
    }
}
