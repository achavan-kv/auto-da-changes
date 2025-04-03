namespace Blue.Cosacs.Merchandising.Models
{
    public class GoodsReceiptDirectProductPrintModel
    {
        public int Id { get; set; }
        public int GoodsReceiptDirectId { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public string Description { get; set; }
        public int QuantityReceived { get; set; }
        public decimal UnitLandedCost { get; set; }
        public string Comments { get; set; }
        public string Currency { get; set; }
    }
}
