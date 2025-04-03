namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class StockCountViewModel
    {
        public int Id { get; set; }

        public int LocationId { get; set; }

        public string Location { get; set; }

        public string Type { get; set; }

        public DateTime CountDate { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? CancelledById { get; set; }

        public string CancelledBy { get; set; }

        public DateTime? CancelledDate { get; set; }
        
        public int? StartedById { get; set; }

        public string StartedBy { get; set; }

        public DateTime? StartedDate { get; set; }

        public int? ClosedById { get; set; }

        public string ClosedBy { get; set; }

        public DateTime? ClosedDate { get; set; }

        public int? StockAdjustmentId { get; set; }

        public Dictionary<int, string> Hierarchy { get; set; }
    }
}
