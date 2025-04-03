namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class GoodsReceiptPrintModel
    {
        public GoodsReceiptPrintModel()
        {
            Products = new List<GoodsReceiptProductView>();
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

        public string DateReceived { get; set; }
        
        public string DateApproved { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public string Comments { get; set; }

        public DateTime? OriginalPrint { get; set; }

        public List<GoodsReceiptProductView> Products { get; set; } 
    }
}
