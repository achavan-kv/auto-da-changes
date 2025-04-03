namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class VendorReturnPurchaseOrderViewModel
    {
        public VendorReturnPurchaseOrderViewModel()
        {
            Products = new List<VendorReturnProductViewModel>();
        }

        public int PurchaseOrderId { get; set; }

        public string Vendor { get; set; }

        public int VendorId { get; set; }

        public List<VendorReturnProductViewModel> Products { get; set; }
    }
}