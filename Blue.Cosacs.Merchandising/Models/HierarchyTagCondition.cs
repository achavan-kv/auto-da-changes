using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Merchandising.Models
{
    public class HierarchyTagCondition
    {
        public int Id { get; set; }

        public int HierarchyTagId { get; set; }

        public decimal? Percentage { get; set; }

        public int RepossessedConditionId { get; set; }

        public string ConditionName { get; set; }
    }
}