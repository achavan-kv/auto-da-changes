namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class LabelModel
    {
        public string CorporateUPC { get; set; }

        public int PurchaseOrderId { get; set; }

        public DateTime DateReceived { get; set; }

        public string Sku { get; set; }

        public string Description { get; set; }

        public string ModelNumber { get; set; }

        public string Brand { get; set; }

        public string VendorName { get; set; }

        public int TotalBoxes { get; set; }

        public int QuantityToPrint { get; set; }
    }
}
