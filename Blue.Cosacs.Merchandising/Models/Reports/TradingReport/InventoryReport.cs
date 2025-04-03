namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class InventoryReport
    {
        public InventoryStatisticsDetails Totals { get; set; }

        public List<InventoryStatisticsDetails> Rows { get; set; }
    }
}