namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class StockTransferViewModel
    {
        public StockTransferViewModel()
        {
            this.Products = new List<StockTransferProductViewModel>();
        }

        public int Id { get; set; }

        public int SendingLocationId { get; set; }

        public string SendingLocation { get; set; }

        public int ReceivingLocationId { get; set; }

        public string ReceivingLocation { get; set; }

        public int? ViaLocationId { get; set; }

        public string ViaLocation { get; set; }

        public string Comments { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public string ReferenceNumber { get; set; }

        public List<StockTransferProductViewModel> Products { get; set; }

        public string SendingLocationSalesId { get; set; }
        
        public string ViaLocationSalesId { get; set; }

        public string ReceivingLocationSalesId { get; set; }
    }
}
