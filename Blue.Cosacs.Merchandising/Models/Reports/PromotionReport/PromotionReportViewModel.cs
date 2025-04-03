namespace Blue.Cosacs.Merchandising.Models
{
    using System;

    public class PromotionReportViewModel
    {
        public int ProductId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string Fascia { get; set; }
        public string Sku { get; set; }
        public string LongDescription { get; set; }
        public int Quantity { get; set; }
        public DateTime PromotionStartDate { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public decimal? GrossTotal { get; set; }
        public decimal? NetTotal { get; set; }
        public decimal? AverageWeightedCost { get; set; }
        public decimal? PromotionalMargin { get; set; }
        public decimal? PromotionalTotal { get; set; }
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public decimal? CashPrice { get; set; }
        public decimal? CashTotal { get; set; }
        public decimal? CashMargin { get; set; }   
    }
}