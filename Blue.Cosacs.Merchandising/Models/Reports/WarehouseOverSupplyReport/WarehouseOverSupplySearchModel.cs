namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class WarehouseOversupplySearchModel
    {
        public int? WarehouseId { get; set; }

        public string Warehouse { get; set; }

        public string Fascia { get; set; }

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

        public List<int> ColIndices { get; set; } 
    }
}