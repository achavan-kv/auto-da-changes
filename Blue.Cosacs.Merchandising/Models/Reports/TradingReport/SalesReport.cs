namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class SalesReport
    {
        public SalesStatisticsDetails Totals { get; set; }

        public List<SalesStatisticsDetails> Rows { get; set; }
    }
}