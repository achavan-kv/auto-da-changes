namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class WarehouseOversupplyPrintModel
    {
        public List<Level> Levels { get; set; }

        public WarehouseOversupplySearchModel Query { get; set; }

        public List<WarehouseOversupplyProductViewModel> Products { get; set; }

        public WarehouseOversupplyPrintModel()
        {
            Products = new List<WarehouseOversupplyProductViewModel>();
        }
    }
}