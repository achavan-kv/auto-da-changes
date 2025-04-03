namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class TradingReportViewModel
    {
        public List<SalesReport> SalesReports { get; set; } 
        public List<InventoryReport> InventoryReports { get; set; }
    }
}
