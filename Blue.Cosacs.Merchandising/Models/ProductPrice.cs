namespace Blue.Cosacs.Merchandising.Models
{
    public class ProductPrice
    {
        public decimal? NormalCashPrice { get; set; }

        public decimal? NormalRegularPrice { get; set; }

        public decimal? NormalDutyFreePrice { get; set; }

        public decimal? TaxRate { get; set; }

        public bool? ForceApplyTax { get; set; }
    }
}