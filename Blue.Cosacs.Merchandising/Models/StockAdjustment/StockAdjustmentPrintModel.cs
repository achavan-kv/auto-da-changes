namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class StockAdjustmentPrintModel
    {
        public StockAdjustmentPrintModel()
        {
            Products = new List<StockAdjustmentProductPrintModel>();
        }

        public int Id { get; set; }

        public string Location { get; set; }

        public string PrimaryReason { get; set; }

        public string SecondaryReason { get; set; }

        public int? AuthorisedById { get; set; }

        public string AuthorisedBy { get; set; }

        public string AuthorisedDate { get; set; }

        public string CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public string Comments { get; set; }

        public string ReferenceNumber { get; set; }

        public DateTime? OriginalPrint { get; set; }

        public List<StockAdjustmentProductPrintModel> Products { get; set; } 
    }
}
