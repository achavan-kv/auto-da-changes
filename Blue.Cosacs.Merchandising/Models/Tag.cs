using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public Level Level { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public decimal? FirstYearWarrantyProvision { get; set; }

        public int? AgedAfter { get; set; }

        public List<HierarchyTagCondition> RepossessedConditions { get; set; } 
    }
}