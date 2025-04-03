using System;
using Blue.Solr;

namespace Blue.Cosacs.Api.Customer.Models
{
    public class CustomerDetailRequest
    {
        public string CustomerId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public DateTime? DOB { get; set; }
        public string HomePhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string TownOrCity { get; set; }
        public string PostCode { get; set; }
        public int Start { get; set; }
        public int Rows { get; set; }
        public decimal? AvailableSpend { get; set; }
        public DateTime? DateLastBought { get; set; }
        public int? CustomerBranch { get; set; }
        public int? SalesPersonId { get; set; }
        public string CustomerSource { get; set; }
        public string[] FacetFields { get; set; }
        public Range[] Ranges { get; set; }
        public string Sort { get; set; }
    }
}
