using System;

namespace Blue.Cosacs.Warranty.Model
{
    public class WarrantyLocationPrice
    {
        public int Id { get; set; }
        public int WarrantyId { get; set; }
        public string BranchType { get; set; }
        public string BranchNumber { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal? CostPriceChange { get; set; }
        public int PromotionCount { get; set; }
        public decimal? CostPricePercentageChange { get; set; }
        public decimal? RetailPriceChange { get; set; }
        public decimal? RetailPricePercentageChange { get; set; }
        public decimal? TaxInclusivePriceChange { get; set; }
        public decimal? TaxInclusivePricePercentageChange { get; set; }
        public string CostPriceChangeInfo { get; set; }
        public string RetailPriceChangeInfo { get; set; }
        public string TaxInclusivePriceChangeInfo { get; set; }
        public int? BulkEditId { get; set; }
    }
}
