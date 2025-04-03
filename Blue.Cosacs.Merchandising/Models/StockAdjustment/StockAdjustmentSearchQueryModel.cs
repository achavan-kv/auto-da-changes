namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class StockAdjustmentSearchQueryModel
    {               
        public int? PrimaryReasonId { get; set; }

        public int? SecondaryReasonId { get; set; }

        public int? LocationId { get; set; }

        public int? MinStockAdjustmentId { get; set; }

        public int? MaxStockAdjustmentId { get; set; }

        public DateTime? MinCreatedDate { get; set; }

        public DateTime? MaxCreatedDate { get; set; }

        public int? Approved { get; set; }       
    }
}
