using System;
namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantyCalculatedPrice : WarrantyPrice
    {
        public decimal? TaxAmount { get; set; }
        public DateTime? PromotionStart { get; set; }
        public DateTime? PromotionEnd { get; set; }
        public decimal? PromotionPercentageDiscount { get; set; }
        public decimal? PromotionRetailPrice { get; set; }
    }
}
