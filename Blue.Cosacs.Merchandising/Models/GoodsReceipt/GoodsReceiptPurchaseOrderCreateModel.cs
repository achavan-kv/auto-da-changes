namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class GoodsReceiptPurchaseOrderCreateModel
    {
        public GoodsReceiptPurchaseOrderCreateModel()
        {
            this.Products = new List<GoodsReceiptProductCreateModel>();
        }

        public int Id { get; set; }

        public List<GoodsReceiptProductCreateModel> Products { get; set; }
    }
}