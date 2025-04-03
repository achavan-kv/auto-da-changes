namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class BuyerSalesHistoryViewModel
    {
        public List<BuyerSalesHistoryProductViewModel> Products { get; set; }

        public BuyerSalesHistorySearchModel Query { get; set; }

        public BuyerSalesHistoryViewModel()
        {
            Products = new List<BuyerSalesHistoryProductViewModel>();
        }
    }
}