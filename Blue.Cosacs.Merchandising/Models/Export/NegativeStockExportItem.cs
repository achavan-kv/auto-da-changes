namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using FileHelpers;

    [DelimitedRecord(",")]
    public class NegativeStockExportItem
    {
        public string Division { get; set; }

        public string Department { get; set; }

        public string Class { get; set; }

        public string Sku { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public string Fascia { get; set; }

        public int StockOnHandQuantity { get; set; }

        public decimal AverageWeightedCost { get; set; }

        public decimal StockOnHandValue { get; set; }

        public DateTime? DateLastReceived { get; set; }

        public string LastTransactionType { get; set; }

        public string LastTransactionId { get; set; }

        public string LastTransactionNarration { get; set; }

        public DateTime? LastTransactionDate { get; set; }
    }
}