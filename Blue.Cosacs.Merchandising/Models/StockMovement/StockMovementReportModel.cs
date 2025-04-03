namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class StockMovementReportModel
    {
        public StockMovementReportModel()
        {
            Tags = new List<string>();
        }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Class { get; set; }
        public string TransactionId { get; set; }
        public int ProductId { get; set; }
        public string LongDescription { get; set; }
        public string BrandName { get; set; }
        public string ProductTags { get; set; }
        public List<string> Tags { get; set; }
        public string SKU { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Narration { get; set; }
        public int Quantity { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? DateUTC { get; set; }
        public DateTime? DateProcessed { get; set; }
        public DateTime? DateProcessedUTC { get; set; }
        public int? UserId { get; set; }
        public string User { get; set; }
        public int StockLevel { get; set; }
        public bool? IsDirect { get; set; }
        public bool IsHeader { get; set; }
    }
}
