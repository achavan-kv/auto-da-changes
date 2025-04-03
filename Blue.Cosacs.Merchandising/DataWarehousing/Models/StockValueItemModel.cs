namespace Blue.Cosacs.Merchandising.DataWarehousing.Models
{
    using Percolator.AnalysisServices.Attributes;

    public class StockValueItemModel
    {
        [MapTo("ParentName")]
        public string Parent { get; set; }

        [MapTo("Name")]
        public string Name { get; set; }

        [MapTo("StockOnHandQuantity")]
        public int StockOnHandQuantity { get; set; }

        [MapTo("StockOnHandValue")]
        public decimal StockOnHandValue { get; set; }

        [MapTo("StockOnHandSalesValue")]
        public decimal StockOnHandSalesValue { get; set; }
    }
}