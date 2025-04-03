namespace Blue.Cosacs.Merchandising.Models
{
    public class StockAllocationStockInfoModel
    {
        public int ProductId { get; set; }
        public int LocationId { get; set; }
        public int? QuantityAvailable { get; set; }
        public decimal? VendorUnitCost { get; set; }
    }
}