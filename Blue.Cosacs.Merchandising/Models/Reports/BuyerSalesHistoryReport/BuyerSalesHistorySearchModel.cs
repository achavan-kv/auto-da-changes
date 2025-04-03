namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BuyerSalesHistorySearchModel
    {
        public int? LocationId { get; set; }

        public string Location { get; set; }

        public string Fascia { get; set; }

        public string Kpi { get; set; }

        public string ProductType { get; set; }
         public string PreviousProductType { get; set; }

        public bool? TaxInclusive { get; set; }

        public bool? DiscountInclusive { get; set; }

        public Dictionary<string, string> Hierarchy { get; set; }

        public List<int> ColIndices { get; set; } 
    }
}