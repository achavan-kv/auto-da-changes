namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class StockCountHierarchyPrintModel
    {
        public StockCountHierarchyPrintModel()
        {
            Products = new List<StockCountProductViewModel>();
        }

        public string Hierarchy { get; set; }

        public List<StockCountProductViewModel> Products { get; set; }
    }
}