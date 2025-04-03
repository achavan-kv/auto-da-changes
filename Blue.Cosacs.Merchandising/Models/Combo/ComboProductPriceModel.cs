namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class ComboProductPriceModel
    {
        public ComboProductPriceModel()
        {
            RegularPrice = new List<ComboProductPriceItemModel>();
            CashPrice = new List<ComboProductPriceItemModel>();
            DutyFreePrice = new List<ComboProductPriceItemModel>();
        }

        public List<ComboProductPriceItemModel> RegularPrice { get; set; }
        public List<ComboProductPriceItemModel> CashPrice { get; set; }
        public List<ComboProductPriceItemModel> DutyFreePrice { get; set; }
    }

    public class ComboProductPriceItemModel
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
    }
}
