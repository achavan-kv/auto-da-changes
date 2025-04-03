namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;
    
    public class NegativeStockLocationModel
    {
        public string Location { get; set; }

        public string Fascia { get; set; }

        public List<NegativeStockReportModel> Items { get; set; }
    }
}