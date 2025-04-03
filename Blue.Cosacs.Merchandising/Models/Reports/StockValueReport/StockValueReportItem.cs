namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class StockValueReportItem
    {
        public string Level { get; set; }

        public int StockOnHandQuantity { get; set; }

        public decimal StockOnHandValue { get; set; }

        public decimal StockOnHandSalesValue { get; set; }

        public List<StockValueReportItem> Children { get; set; }
    }
}