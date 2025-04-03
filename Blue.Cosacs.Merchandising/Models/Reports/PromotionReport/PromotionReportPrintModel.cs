namespace Blue.Cosacs.Merchandising.Models
{
    using Blue.Cosacs.Merchandising.Infrastructure;
    using System;

    public class PromotionReportPrintModel
    {
        public PagedSearchResult<PromotionReportViewModel> Results { get; set; }
        public PromotionReportSearchModel Query { get; set; }
    }
}