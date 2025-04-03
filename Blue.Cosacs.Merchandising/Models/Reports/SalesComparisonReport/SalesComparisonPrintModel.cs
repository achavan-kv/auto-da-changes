namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class SalesComparisonPrintModel
    {
        public IEnumerable<IEnumerable<SalesComparisonViewModel>> Results { get; set; }
        public SalesComparisonSearchModel Query { get; set; }
        public string Location { get; set; }
        public string Brand { get; set; }
    }
}
