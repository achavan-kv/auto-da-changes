namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockCountSearchQueryModel
    {               
        public int? LocationId { get; set; }

        public int? MinStockCountId { get; set; }

        public int? MaxStockCountId { get; set; }

        public DateTime? MinScheduledDate { get; set; }

        public DateTime? MaxScheduledDate { get; set; }

        public DateTime? MinStartedDate { get; set; }

        public DateTime? MaxStartedDate { get; set; }

        public string Status { get; set; }
        
        public string Type { get; set; }       
    }
}
