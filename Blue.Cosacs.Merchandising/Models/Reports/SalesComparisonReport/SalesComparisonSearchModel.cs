namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class SalesComparisonSearchModel
    {
        public DateTime PeriodEnd { get; set; }

        public bool FiscalYear { get; set; }

        public int? LocationId { get; set; }

        public string Fascia { get; set; }

        public int? BrandId { get; set; }

        public List<string> Tags { get; set; }

        public string TagString
        {
            get
            {
                return Tags == null ? string.Empty : string.Join(", ", Tags);
            }
        }

        public Dictionary<string, string> Hierarchy { get; set; }

        public string HierarchyString { get; set; }

        public bool GroupByLocation { get; set; }

        public List<int> ColIndices { get; set; } 
    }
}