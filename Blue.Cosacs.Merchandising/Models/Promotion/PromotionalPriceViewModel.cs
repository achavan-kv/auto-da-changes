namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class PromotionalPriceViewModel
    {
        public int? Id { get; set; }

        public string Fascia { get; set; }

        public string Location { get; set; }

        public int? LocationId { get; set; }

        public int ProductId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public decimal TaxRate { get; set; }

        public decimal? RegularPrice { get; set; }

        public decimal? CashPrice { get; set; }

        public decimal? DutyFreePrice { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}