namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class GoodsReceiptDirectPrintModel
    {
        public GoodsReceiptDirectPrintModel()
        {
            Products = new List<GoodsReceiptDirectProductPrintModel>();
        }

        public int Id { get; set; }

        public string Location { get; set; }

        public int ReceivedById { get; set; }

        public string ReceivedBy { get; set; }

        public int? ProcessedById { get; set; }

        public string ProcessedBy { get; set; }

        public int? ApprovedById { get; set; }

        public string ApprovedBy { get; set; }

        public string VendorDeliveryNumber { get; set; }

        public string VendorInvoiceNumber { get; set; }

        public DateTime DateReceived { get; set; }
        
        public DateTime? DateApproved { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public string Comments { get; set; }

        public DateTime? OriginalPrint { get; set; }

        public string Vendor { get; set; }

        public string Currency { get; set; }

        public List<GoodsReceiptDirectProductPrintModel> Products { get; set; } 
    }
}
