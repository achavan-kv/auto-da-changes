namespace Blue.Cosacs.Merchandising.Models
{
    public class SalesComparisonSalesModel
    {
        public int Quantity { get; set; }
        public decimal GrossValue { get; set; }
        public decimal NetValue { get; set; }
        public decimal GrossMarginValue { get; set; }
        public decimal NetMarginValue { get; set; }
        public decimal GrossMarginPercent { get; set; }
        public decimal NetMarginPercent { get; set; }
    }
}
