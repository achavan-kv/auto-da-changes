namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class PromotionReportSearchModel
    {
        public PromotionReportSearchModel Query { get; set; }

        public int? LocationId { get; set; }

        public string Fascia { get; set; }

        public string LocationName { get; set; }

        public List<int> PromotionIds { get; set; }

        public List<string> Promotions { get; set; } 
    }
}