namespace Blue.Cosacs.Merchandising.Models
{
    public class StockLunrModel
    {
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public string LongDescription { get; set; }
        public decimal AverageWeightedCost { get; set; }
        public int StockOnHand { get; set; }
        public int StockOnOrder { get; set; }
        public int StockAvailable { get; set; }
    }
}