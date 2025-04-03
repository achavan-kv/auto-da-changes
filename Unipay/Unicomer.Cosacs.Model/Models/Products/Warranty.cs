namespace Unicomer.Cosacs.Model.Models.Products
{
    public class Warranty
    {
        public string UPC { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public string Period { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
