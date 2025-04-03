namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class GoodsReceiptProductViewModel
    {
        public int PurchaseOrderProductId { get; set; }

        public int ProductId { get; set; }

        public DateTime EstimatedDeliveryDate { get; set; }

        public string ProductCode { get; set; }

        public string Description { get; set; }

        public string Comments { get; set; }

        public int QuantityOrdered { get; set; }

        public int QuantityReceived { get; set; }

        public int QuantityCancelled { get; set; }

        public int QuantityBackOrdered { get; set; }

        public string ReasonForCancellation { get; set; }

        public decimal UnitCost { get; set; }

        public decimal LastLandedCost { get; set; }
    }
}