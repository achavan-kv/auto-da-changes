namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class GoodsReceiptCostsPurchaseOrderModel
    {
        public GoodsReceiptCostsPurchaseOrderModel()
        {
            Products = new List<GoodsReceiptCostsProductModel>();
        }

        public int Id { get; set; }

        public string Vendor { get; set; }
        
        public int VendorId { get; set; }

        public List<GoodsReceiptCostsProductModel> Products { get; set; } 
    }
}
