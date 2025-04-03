namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class WarehouseOversupplyViewModel
    {
        public List<Level> Levels { get; set; }

        public List<WarehouseOversupplyProductViewModel> Products { get; set; }

        public WarehouseOversupplyViewModel()
        {
            Products = new List<WarehouseOversupplyProductViewModel>();
        }
    }
}