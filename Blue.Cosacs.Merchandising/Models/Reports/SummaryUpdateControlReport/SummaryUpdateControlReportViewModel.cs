namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class SummaryUpdateControlReportViewModel
    {
        public List<SummaryUpdateControlReportView> Rows { get; set; }
        public InventoryModel OpeningInventory { get; set; }
        public InventoryModel ClosingInventory { get; set; }
    }
}