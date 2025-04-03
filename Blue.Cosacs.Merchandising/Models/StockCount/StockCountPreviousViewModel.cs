namespace Blue.Cosacs.Merchandising.Models
{
    using Blue.Cosacs.Merchandising.Infrastructure;

    public class StockCountPreviousViewModel
    {
        public StockCountPreviousViewModel()
        {
            StockCounts = new PagedSearchResult<StockCountPreviousItemViewModel>();
        }

        public int ProductId { get; set; }

        public string Sku { get; set; }

        public string Description { get; set; }

        public int Count { get; set; }

        public PagedSearchResult<StockCountPreviousItemViewModel> StockCounts { get; set; } 
    }
}
