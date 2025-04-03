namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class StockReceivedQueryModel
    {
        public string Division { get; set; }

        public string Department { get; set; }

        public string Class { get; set; }

        public DateTime? CreatedFrom { get; set; }

        public DateTime? CreatedTo { get; set; }

        public string Fascia { get; set; }

        public int? LocationId { get; set; }

        public int? VendorId { get; set; }

        public int? PurchaseOrderId { get; set; }
    }
}
