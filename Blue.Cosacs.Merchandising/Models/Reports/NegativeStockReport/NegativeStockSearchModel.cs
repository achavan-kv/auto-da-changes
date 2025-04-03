namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class NegativeStockSearchModel
    {
        public DateTime PeriodEndDate { get; set; }
        
        public int? LocationId { get; set; }

        public string Fascia { get; set; }

        public string StockType { get; set; }

        public bool IsGrouped { get; set; }

        public string Division { get; set; }

        public string Department { get; set; }

        public string Class { get; set; }

        public List<string> Tags { get; set; }

        public List<int> ColIndices { get; set; } 
    }
}