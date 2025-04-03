using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.CosacsWeb.Models
{
    public class NonStockResponse
    {
        public string Result { get; set; }
        public NonStock NonStock { get; set; }
    }

    public class NonStock
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string SKU { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public bool Active { get; set; }
        public decimal? TaxRate { get; set; }
        public int? Length { get; set; }
        public bool? IsStaffDiscount { get; set; }
        public int? DiscountRecurringPeriod { get; set; }
        public bool? Deleted { get; set; }
        public bool? CanApplyToPromotion { get; set; }
        public string VendorUPC { get; set; }
        public bool? IsFullRefund { get; set; }
    }
}
