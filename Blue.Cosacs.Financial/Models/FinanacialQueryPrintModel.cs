namespace Blue.Cosacs.Financial.Models
{
    using System.Collections.Generic;

    public class FinanacialQueryPrintModel
    {
        public List<FinancialQueryViewModel> Results { get; set; }
        public FinanacialQueryQueryModel Query { get; set; }
    }
}