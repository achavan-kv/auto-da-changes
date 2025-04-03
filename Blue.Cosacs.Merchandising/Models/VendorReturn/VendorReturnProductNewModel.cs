namespace Blue.Cosacs.Merchandising.Models
{
    public class VendorReturnProductNewModel
    {
        public int GoodsReceiptProductId { get; set; }

        public string ProductCode { get; set; }

        public string Description { get; set; }

        public int QuantityReceived { get; set; }

        public int QuantityPreviouslyReturned { get; set; }

        public int QuantityRemaining { get; set; }

        public decimal? LastLandedCost { get; set; }
    }
}