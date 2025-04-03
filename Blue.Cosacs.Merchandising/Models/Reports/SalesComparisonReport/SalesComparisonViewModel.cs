namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SalesComparisonViewModel
    {
        public SalesComparisonViewModel()
        {
            HierarchyTags = new List<string>();
        }

        public int Id { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Class { get; set; }
        public int ProductId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string SalesId { get; set; }
        public string Fascia { get; set; }
        public string Sku { get; set; }
        public string LongDescription { get; set; }
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public string Tags { get; set; }
        public List<string> HierarchyTags { get; set; }
        public int StockOnHand { get; set; }
        public int StockOnOrder { get; set; }
        public int? StockRequested { get; set; }
        public decimal CurrentRegularPrice { get; set; }
        public decimal CurrentCashPrice { get; set; }
        public decimal PromotionalRegularPrice { get; set; }
        public decimal PromotionalCashPrice { get; set; }
        public SalesComparisonSalesModel ThisYear { get; set; }
        public SalesComparisonSalesModel LastYear { get; set; }
    }
}