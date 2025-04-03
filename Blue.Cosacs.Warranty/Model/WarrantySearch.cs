using Blue.Data;
using System;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantySearchByProduct : PagedSearch
    {
        public string Product { get; set; }
        public decimal? PriceVATEx { get; set; }
        public short Location { get; set; }
        public DateTime? Date { get; set; }
        public string Department { get; set; }
        public short CategoryId { get; set; }
        public string WarrantyTypeCode { get; set; }

        public bool HasValidCategory
        {
            get { return (CategoryId > 0 && !string.IsNullOrEmpty(Department)); }
        }
    }
}
