namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class NegativeStockReportModel
    {
        public string Division { get; set; }

        public string Department { get; set; }

        public string Class { get; set; }

        public int ProductId { get; set; }

        public string Sku { get; set; }

        public string Description { get; set; }

        public int LocationId { get; set; }

        public string Location { get; set; }

        public string Fascia { get; set; }

        public int StockOnHandQuantity { get; set; }

        public decimal AverageWeightedCost { get; set; }

        public decimal StockOnHandValue
        {
            get
            {
                return StockOnHandQuantity * AverageWeightedCost;
            }
        }

        public DateTime? DateLastReceived { get; set; }

        public string LastTransactionType { get; set; }

        public string LastTransactionId { get; set; }

        public string LastTransactionNarration { get; set; }

        public DateTime? LastTransactionDate { get; set; }
    }
}