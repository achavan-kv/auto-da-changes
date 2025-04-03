namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class GoodsReceiptPurchaseOrderAuditModel
    {
        public GoodsReceiptPurchaseOrderAuditModel()
        {
            this.Products = new List<GoodsReceiptProductAuditModel>();
        }

        public int PurchaseOrderId { get; set; }

        public int VendorId { get; set; }

        public string Vendor { get; set; }

        public List<GoodsReceiptProductAuditModel> Products { get; set; }
    }
}