namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class GoodsReceiptCostsProductModel
    {
        public int Id { get; set; }
        public int GoodsReceiptId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public DateTime GoodsReceiptDate { get; set; }
        public int QuantityReceived { get; set; }
        public decimal UnitLandedCost { get; set; }
        public decimal TotalLandedCost { get; set; }
        public DateTime? LastReceivedDate { get; set; }
        public DateTime? LandedCostLastUpdated { get; set; }
        public DateTime? LastCostsConfirmedDate { get; set; }
        public decimal Margin { get; set; }
        public string Comments { get; set; }
    }
}
