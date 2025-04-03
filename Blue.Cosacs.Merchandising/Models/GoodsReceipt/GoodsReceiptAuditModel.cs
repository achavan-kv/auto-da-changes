namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class GoodsReceiptAuditModel
    {
        public GoodsReceiptAuditModel()
        {
            Products = new List<GoodsReceiptProductAuditModel>();
        }

        public int Id { get; set; }

        public int? LocationId { get; set; }
        
        public string Location { get; set; }

        public int? ReceivedById { get; set; }

        public string ReceivedBy { get; set; }

        public int? CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public int? ApprovedById { get; set; }

        public string ApprovedBy { get; set; }

        public string VendorDeliveryNumber { get; set; }

        public string VendorInvoiceNumber { get; set; }

        public string DateReceived { get; set; }

        public string CreatedDate { get; set; }
        
        public string DateApproved { get; set; }

        public string Comments { get; set; }

        public bool EnableBackOrderCancel { get; set; }

        public List<GoodsReceiptProductAuditModel> Products { get; set; }
    }
}
