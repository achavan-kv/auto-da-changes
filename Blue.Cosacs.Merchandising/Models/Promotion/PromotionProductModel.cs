using System;

namespace Blue.Cosacs.Merchandising.Models
{
    public class PromotionProductModel
    {
        public int ProductId { get; set; }

        public int LocationId { get; set; }

        public int PromotionId { get; set; }

        public string Sku { get; set; }

        public string LongDescription { get; set; }

        public string SalesId { get; set; }

        public decimal? RegularPrice { get; set; }

        public decimal? CashPrice { get; set; }

        public decimal? DutyFreePrice { get; set; }

        public decimal? AverageWeightedCost { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string PromotionLocation { get; set; }

        public string PromotionName { get; set; }

        public string Fascia { get; set; }
    }
}