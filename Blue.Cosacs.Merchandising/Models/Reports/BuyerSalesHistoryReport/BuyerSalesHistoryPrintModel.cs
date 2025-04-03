namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class BuyerSalesHistoryPrintModel
    {
        public List<Level> Levels { get; set; }

        public BuyerSalesHistorySearchModel Query { get; set; }

        public List<BuyerSalesHistoryProductViewModel> Products { get; set; }

        public BuyerSalesHistoryPrintModel()
        {
            Products = new List<BuyerSalesHistoryProductViewModel>();
        }
    }
}