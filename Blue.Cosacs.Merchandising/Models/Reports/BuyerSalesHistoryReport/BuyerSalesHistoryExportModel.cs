namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using FileHelpers;

    [DelimitedRecord(",")]
    public class BuyerSalesHistoryExportModel
    {
        public string Sku { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Vendor { get; set; }
        public int StockOnOrder { get; set; }
        public int StockOnHand { get; set; }
        public decimal AverageWeightedCost { get; set; }
        public decimal StockOnHandCost { get; set; }
        public decimal CashPrice { get; set; }
        public string Year { get; set; }
        public decimal April { get; set; }
        public decimal May { get; set; }
        public decimal June { get; set; }
        public decimal July { get; set; }
        public decimal August { get; set; }
        public decimal September { get; set; }
        public decimal October { get; set; }
        public decimal November { get; set; }
        public decimal December { get; set; }
        public decimal January { get; set; }
        public decimal February { get; set; }
        public decimal March { get; set; }
        public decimal YearToDate { get; set; }
    }
}