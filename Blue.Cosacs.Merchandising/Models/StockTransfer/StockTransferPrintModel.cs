namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class StockTransferPrintModel
    {
        public StockTransferPrintModel()
        {
            this.Products = new List<StockTransferProductViewModel>();
        }

        public int Id { get; set; }

        public string SendingLocation { get; set; }

        public string ReceivingLocation { get; set; }

        public string ViaLocation { get; set; }

        public string Comments { get; set; }

        public string CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ReferenceNumber { get; set; }

        public DateTime? OriginalPrint { get; set; }

        public List<StockTransferProductViewModel> Products { get; set; }
    }
}
