namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class StockMovementPrintModel
    {
        public IEnumerable<IEnumerable<StockMovementReportModel>> Results { get; set; }
        public StockMovementQueryModel Query { get; set; }
        public List<int> ColIndicies { get; set; }
        public string Location { get; set; }
    }
}
