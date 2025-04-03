using System.Collections.Generic;

namespace Blue.Cosacs.Model
{
    public class SalesKitItem
    {
        public SalesKitItem()
        {
            ItemNo = string.Empty;
            Category = 0;
            ParentId = 0;
            DiscountValue = 0;
            DiscountPercentage = 0;
            Quantity = 0;
        }
        public string ItemNo { get; set; }
        public int ParentId { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal DiscountPercentage { get; set; }
        public short? Category { get; set; }
        public SalesKitItemDiscount DiscountItem { get; set; }
    }
}
