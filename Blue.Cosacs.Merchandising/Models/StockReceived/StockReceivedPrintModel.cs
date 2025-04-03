namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class StockReceivedPrintModel
    {
        public IEnumerable<IEnumerable<StockReceivedReportModel>> Results { get; set; }
        public StockReceivedQueryModel Query { get; set; }
        public List<int> ColIndicies { get; set; }
    }
}
