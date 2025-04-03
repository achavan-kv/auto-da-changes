namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class StockAdjustmentViewModel
    {
        public StockAdjustmentViewModel()
        {
            Products = new List<StockAdjustmentProductViewModel>();
        }

        public int Id { get; set; }

        public int LocationId { get; set; }

        public string Location { get; set; }

        public string SalesLocationId { get; set; }

        public int PrimaryReasonId { get; set; }

        public string PrimaryReason { get; set; }

        public int SecondaryReasonId { get; set; }

        public string SecondaryReason { get; set; }

        public string Comments { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? AuthorisedDate { get; set; }

        public DateTime? OriginalPrint { get; set; }

        public int? AuthorisedById { get; set; }

        public string AuthorisedBy { get; set; }

        public string ReferenceNumber { get; set; }

        public List<StockAdjustmentProductViewModel> Products { get; set; }
    }
}
