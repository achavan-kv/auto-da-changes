namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class GoodsReceiptDirectViewModel
    {
        public GoodsReceiptDirectViewModel()
        {
            Products = new List<GoodsReceiptDirectProduct>();
        }

        public int Id { get; set; }

        public int? LocationId { get; set; }
        
        public string Location { get; set; }

        public int? CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public int? ReceivedById { get; set; }

        public string ReceivedBy { get; set; }

        public int? ApprovedById { get; set; }

        public string ApprovedBy { get; set; }

        public int? VendorId { get; set; }

        public string Vendor { get; set; }

        public string VendorType { get; set; }

        public string VendorDeliveryNumber { get; set; }

        public string VendorInvoiceNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime DateReceived { get; set; }
        
        public DateTime? DateApproved { get; set; }

        public string Comments { get; set; }

        public List<StringKeyValue> ReferenceNumbers { get; set; }

        public List<GoodsReceiptDirectProduct> Products { get; set; }

        public string SalesLocationId { get; set; }
    }
}
