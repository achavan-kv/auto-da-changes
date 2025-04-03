namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;

    public class StockRequisitionOnOrderViewModel
    {
        public StockRequisitionOnOrderViewModel()
        {
            this.StockItems = new List<StockRequisitionOnOrderProductViewModel>();
        }

        public int Id { get; set; }

        public string Sku { get; set; }

        public string Description { get; set; }

        public int LocationId { get; set; }

        public string Location { get; set; }

        public List<StockRequisitionOnOrderProductViewModel> StockItems { get; set; }
    }
}
