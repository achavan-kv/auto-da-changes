using System;
namespace Blue.Cosacs.Credit.Model
{
    public class ProposalSearchParams
    {
        public string Branch { get; set; }
        public string Fascia { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? SalesPerson { get; set; }
        public string Stage { get; set; }
        public string Source { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string CompulsoryId { get; set; }
    }
}
