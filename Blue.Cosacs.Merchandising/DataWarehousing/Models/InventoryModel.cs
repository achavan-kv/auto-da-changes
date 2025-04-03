namespace Blue.Cosacs.Merchandising.DataWarehousing.Models
{
    using Percolator.AnalysisServices.Attributes;

    public class InventoryModel
    {
        [MapTo("ParentName")]
        public string Category { get; set; }
        [MapTo("Name")]
        public string Name { get; set; }

        [MapTo("Stock Value")]
        public decimal StockValue { get; set; }

        [MapTo("StockFraction")]
        public decimal StockFraction { get; set; }
    }
}