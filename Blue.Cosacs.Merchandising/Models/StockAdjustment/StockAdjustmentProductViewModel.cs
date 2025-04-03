namespace Blue.Cosacs.Merchandising.Models
{
    public class StockAdjustmentProductViewModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Sku { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public string Comments { get; set; }

        public string ReferenceNumber { get; set; }

        public decimal AverageWeightedCost { get; set; }

        public string DepartmentCode { get; set; }
    }
}