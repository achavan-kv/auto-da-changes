namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class VendorReturnPurchaseOrderPrintModel
    {
        public VendorReturnPurchaseOrderPrintModel()
        {
            Products = new List<VendorReturnProductPrintModel>();
        }

        public int PurchaseOrderId { get; set; }

        public string Vendor { get; set; }

        public List<VendorReturnProductPrintModel> Products { get; set; }
    }
}