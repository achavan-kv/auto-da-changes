namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class PurchaseOrderProductViewModel
    {
        public int Id { get; set; }
        public string Sku { get; set; }

        public int ProductId { get; set; }

        public string Description { get; set; }

        public DateTime RequestedDeliveryDate { get; set; }

        public DateTime? EstimatedDeliveryDate { get; set; }

        public int QuantityPending { get; set; }

        public int QuantityCancelled { get; set; }

        public int QuantityOrdered { get; set; }

        public decimal UnitCost { get; set; }

        public string FormattedUnitCost { get; set; }

        public string FormattedLineCost { get; set; }

        public string FormattedPendingCost { get; set; }

        public string Comments { get; set; }

        public int TotalQuantityReceived { get; set; }

        public bool? LabelRequired { get; set; }

        public int BoxCount { get; set; }

        public int PrintQuantity { get; set; }

        public AdditionalCostPrice AdditionalCostPrice { get; set; }
}
}