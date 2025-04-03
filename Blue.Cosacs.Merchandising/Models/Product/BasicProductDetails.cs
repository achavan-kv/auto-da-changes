namespace Blue.Cosacs.Merchandising.Models
{
    public class BasicProductDetails
    {
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public string LongDescription { get; set; }
        public bool? Warrantable { get; set; }
    }
}