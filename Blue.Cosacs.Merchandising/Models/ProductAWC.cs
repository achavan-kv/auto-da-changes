using System.Collections.Generic;

namespace Blue.Cosacs.Merchandising.Models
{
    public class ProductAWCInput
    {
        public Dictionary<int, GeneralCostPrice> CostPrice { get; set; }
        public Dictionary<int, decimal> ReposessionProductPrice { get; set; }
        public Dictionary<int, int> StockOnHand { get; set; }
    }

    public class GeneralCostPrice
    {
        public int ProductId { get; set; }
        public decimal SupplierCost { get; set; }
        public decimal AverageWeightedCost { get; set; }
        public decimal LastLandedCost { get; set; }
        public string Currency { get; set; }
    }
}
