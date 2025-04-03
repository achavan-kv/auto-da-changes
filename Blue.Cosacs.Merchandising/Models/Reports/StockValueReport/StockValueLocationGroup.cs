namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    using Blue.Cosacs.Merchandising.DataWarehousing.Models;

    public class StockValueLocationGroup
    {
        public string Location { get; set; }

        public string Fascia { get; set; }

        public List<StockValueReportItem> Items { get; set; }
    }
}