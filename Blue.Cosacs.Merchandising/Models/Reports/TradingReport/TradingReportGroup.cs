namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class TradingReportGroup
    {
        public string Name { get; set; }
        public List<SalesReport> Reports { get; set; }
    }
}