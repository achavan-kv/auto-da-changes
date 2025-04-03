using System;
using Blue.Cosacs.Warranty.Model;
using Blue.Data;

namespace Blue.Cosacs.Warranty.Promotions
{
    public class Filter : PagedSearch
    {
        public int? Id { get; set; }
        public int? WarrantyId { get; set; }
        public string Warranty { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
		public string BranchType { get; set; }
        public string BranchNumber { get; set; }
    }
}
