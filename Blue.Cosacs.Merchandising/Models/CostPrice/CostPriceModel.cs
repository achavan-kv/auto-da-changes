using System;

namespace Blue.Cosacs.Merchandising.Models
{
    public class CostPriceModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public decimal SupplierCost { get; set; }

        public decimal LastLandedCost { get; set; }

        public string SupplierCurrency { get; set; }

        public decimal AverageWeightedCost { get; set; }
        
        public DateTime AverageWeightedCostUpdated { get; set; }

        public DateTime LastLandedCostUpdated { get; set; }
    }
}
