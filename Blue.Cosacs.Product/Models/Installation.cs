
namespace Blue.Cosacs.Stock.Models
{
    public class Installation
    {
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string ItemDescription { get; set; }
        public string ItemDescription2 { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? UnitPriceHP { get; set; }
        public decimal? UnitPriceCash { get; set; }
        public decimal? TaxAmount { get; set; }
        public double TaxRate { get; set; }
        public string IUPC { get; set; }
        public int ItemID { get; set; }
    }
}
