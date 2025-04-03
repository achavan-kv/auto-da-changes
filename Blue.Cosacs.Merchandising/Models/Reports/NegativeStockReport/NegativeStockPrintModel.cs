namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class NegativeStockPrintModel
    {
        public NegativeStockSearchModel Query { get; set; }

        public List<NegativeStockExportItem> Results { get; set; }
    }
}