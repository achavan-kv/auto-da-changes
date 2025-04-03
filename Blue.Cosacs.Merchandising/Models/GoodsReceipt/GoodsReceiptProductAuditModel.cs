namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class GoodsReceiptProductAuditModel
    {
        public int PurchaseOrderProductId { get; set; }

        public int ProductId { get; set; }

        public string EstimatedDeliveryDate { get; set; }

        public string ProductCode { get; set; }

        public string Description { get; set; }

        public string Comments { get; set; }

        public int QuantityOrdered { get; set; }

        public int QuantityReceived { get; set; }

        public int QuantityCancelled { get; set; }

        public int QuantityBackOrdered { get; set; }

        public string ReasonForCancellation { get; set; }

        public decimal UnitCost { get; set; }
    }
}