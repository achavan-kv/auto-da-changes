namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class GoodsReceiptPurchaseOrderViewModel
    {
        public GoodsReceiptPurchaseOrderViewModel()
        {
            this.Products = new List<GoodsReceiptProductViewModel>();
        }

        public int PurchaseOrderId { get; set; }

        public int VendorId { get; set; }

        public string Vendor { get; set; }

        public string VendorType { get; set; }

        public List<GoodsReceiptProductViewModel> Products { get; set; }
    }
}