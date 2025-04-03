namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public enum TopSkuPerformanceType
    {
        Units = 1,
        GrossMargin = 2,
        NetSalesValue = 3
    }

    public class TopSkuSearchModel
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? LocationId { get; set; }

        public string Location { get; set; }

        public string Fascia { get; set; }

        public int PerformancePercentage { get; set; }

        public TopSkuPerformanceType PerformanceType { get; set; }

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
    }
}