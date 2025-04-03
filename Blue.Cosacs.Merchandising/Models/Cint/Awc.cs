namespace Blue.Cosacs.Merchandising.Models.Cint
{
    public class ProductAwc
    {
        public int ProductId { get; set; }
        public decimal Awc { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public decimal LastLandedCost { get; set; }
    }

    public class ProductsAwc
    {
        public int ProductId { get; set; }
        public decimal Awc { get; set; }
        public decimal LastLandedCost { get; set; }
        public decimal SupplierCost { get; set; }
        public string Currency { get; set; }
        public bool Save { get; set; }
    }
}