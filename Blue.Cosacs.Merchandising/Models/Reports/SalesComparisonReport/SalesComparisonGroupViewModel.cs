namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class SalesComparisonGroupViewModel
    {
        public SalesComparisonGroupViewModel()
        {
            Items = new List<SalesComparisonViewModel>();
            Children = new List<SalesComparisonGroupViewModel>();
        }

        public string Name { get; set; }
        public SalesComparisonSalesModel ThisYearTotals { get; set; }
        public SalesComparisonSalesModel LastYearTotals { get; set; }
        public List<SalesComparisonGroupViewModel> Children { get; set; }
        public List<SalesComparisonViewModel> Items { get; set; }
    }
}