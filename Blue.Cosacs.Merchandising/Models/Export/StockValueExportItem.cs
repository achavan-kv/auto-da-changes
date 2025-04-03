namespace Blue.Cosacs.Merchandising.Models
{
    using FileHelpers;

    [DelimitedRecord(",")]
    public class StockValueExportItem
    {
        public string Location { get; set; }

        public string Fascia { get; set; }

        public int HierarchyLevel { get; set; }

        public string HierarchyTag { get; set; }

        public int StockOnHandQuantity { get; set; }

        public string StockOnHandValue { get; set; }

        public string StockOnHandSalesValue { get; set; }
    }
}