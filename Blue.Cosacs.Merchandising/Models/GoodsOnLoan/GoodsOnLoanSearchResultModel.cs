namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class GoodsOnLoanSearchResultModel
    {
        public int Id { get; set; }

        public int StockLocationId { get; set; }

        public string StockLocation { get; set; }

        public string Comments { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ExpectedCollectionDate { get; set; }

        public DateTime? CollectedDate { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public string ReferenceNumber { get; set; }

        public string CustomerId { get; set; }

        public string BusinessName { get; set; }

        public string Status { get; set; }

        public decimal? TotalCost { get; set; }
    }
}
