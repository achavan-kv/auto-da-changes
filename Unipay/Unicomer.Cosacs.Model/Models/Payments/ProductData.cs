namespace Unicomer.Cosacs.Model.Models.Payments
{
    public class ProductData
    {
        public string UPC { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PriceTax { get; set; }
        public decimal UnitDiscount { get; set; }
        public string Description { get; set; }
    }
}
