using System;

namespace Blue.Cosacs.NonStocks.Models
{
    public class NonStockLinkSearch
    {
        public string Name { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        // Blue.Data -> IPagedSearch
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public bool HasFilter()
        {
            if (!string.IsNullOrWhiteSpace(Name) || DateFrom != null || DateTo != null)
            {
                return true;
            }

            return false;
        }
    }
}
