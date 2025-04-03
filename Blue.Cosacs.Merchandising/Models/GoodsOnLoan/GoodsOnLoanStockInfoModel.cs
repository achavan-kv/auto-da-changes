namespace Blue.Cosacs.Merchandising.Models
{
    public class GoodsOnLoanStockInfoModel
    {
        public int ProductId { get; set; }
        public int LocationId { get; set; }
        public decimal? AverageWeightedCost { get; set; }
    }
}