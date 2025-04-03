namespace Blue.Cosacs.Merchandising.Models
{
    public class PromotionalPrice
    {
        public string Fascia { get; set; }

        public int? LocationId { get; set; }

        public int ProductId { get; set; }

        public string SKU { get; set; }

        public string Hierarchy { get; set; }

        public decimal TaxRate { get; set; }

        public decimal NormalRegularPrice { get; set; }

        public decimal NormalCashPrice { get; set; }

        public decimal NormalDutyFreePrice { get; set; }

        public bool? ForceApplyTax { get; set; }
    }
}