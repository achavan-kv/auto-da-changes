namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class AppliedPromo
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal? PromoRegularPrice { get; set; }

        public decimal? PromoCashPrice { get; set; }

        public decimal? PromoDutyFreePrice { get; set; }
    }

    public class ApplicablePromo
    {
        public decimal? Price { get; set; }
        public string PriceType { get; set; }

        public decimal? ValueDiscount { get; set; }
        public decimal? PercentDiscount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}