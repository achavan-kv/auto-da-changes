namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockAdjustmentSearchResultModel
    {
        public int Id { get; set; }

        public int StockAdjustmentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Location { get; set; }

        public int LocationId { get; set; }

        public string PrimaryReason { get; set; }

        public int PrimaryReasonId { get; set; }

        public decimal? TotalCost { get; set; }

        public string SecondaryReason { get; set; }
       
        public int SecondaryReasonId { get; set; }

        public string ReferenceNumber { get; set; }

        public string Comments { get; set; }

        public string Status { get; set; }
    }
}
