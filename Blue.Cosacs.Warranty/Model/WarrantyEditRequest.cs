using System;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantyEditRequest
    {
        public PriceEditRequest CostPrice { get; set; }
        public PriceEditRequest RetailPrice { get; set; }
        public PriceEditRequest TaxInclusivePrice { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
