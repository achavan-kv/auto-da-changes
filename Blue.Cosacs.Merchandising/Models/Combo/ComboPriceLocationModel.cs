namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class ComboPriceLocationModel
    {
        public int? LocationId { get; set; }
        public string Fascia { get; set; }
        public string LocationName { get; set; }
        public List<ComboPriceView> Prices { get; set; }
    }
}
