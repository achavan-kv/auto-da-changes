namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class PurchaseOrderProductImportModel
    {
        public int ProductId { get; set; }

        public string Comments { get; set; }

        public DateTime RequestedDeliveryDate { get; set; }

        public int QuantityOrdered { get; set; }

        public decimal PreLandedUnitCost { get; set; }

        public decimal SupplierUnitCost { get; set; }

        public decimal PreLandedExtendedCost { get; set; }

        public bool? LabelRequired { get; set; }
    }
}