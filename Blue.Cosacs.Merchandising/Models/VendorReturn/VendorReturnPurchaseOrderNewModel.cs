namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class VendorReturnPurchaseOrderNewModel
    {
        public VendorReturnPurchaseOrderNewModel()
        {
            Products = new List<VendorReturnProductNewModel>();
        }

        public int PurchaseOrderId { get; set; }

        public int VendorId { get; set; }

        public string Vendor { get; set; }

        public List<VendorReturnProductNewModel> Products { get; set; }
    }
}