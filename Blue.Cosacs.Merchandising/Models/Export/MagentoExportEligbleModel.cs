namespace Blue.Cosacs.Merchandising.Models
{
    using Blue.Cosacs.Merchandising.Helpers;

    using FileHelpers;

    [DelimitedRecord(",")]
    public class MagentoExportEligbleModel
    {
        [FieldTitle("ISOCountryCode")]
        public string IsoCountryCode { get; set; }
        [FieldTitle("Product Code")]
        public string ProductCode { get; set; }
        [FieldTitle("Desc 01")]
        public string Desc01 { get; set; }
        [FieldTitle("Desc 02")]
        public string Desc02 { get; set; }
        [FieldTitle("CashPriceInclTax")]
        public string CashPriceIncTax { get; set; }
        [FieldTitle("PromoCashPriceInclTax")]
        public string PromoCashPriceInclTax { get; set; }
        [FieldTitle("PromoStartDate")]
        public string PromoStartDate { get; set; }
        [FieldTitle("PromoEndDate")]
        public string PromoEndDate { get; set; }
        [FieldTitle("StockQty")]
        public int StockQty { get; set; }
        [FieldTitle("MagMgStock")]
        public int MagMgStock { get; set; }
    }
}