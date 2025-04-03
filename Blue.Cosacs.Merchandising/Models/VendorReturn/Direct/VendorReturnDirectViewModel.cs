namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class VendorReturnDirectViewModel
    {
        public VendorReturnDirectViewModel()
        {
            Products = new List<VendorReturnProductViewModel>();
        }

        public int Id { get; set; }

        public string Comments { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public int CreatedById { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string ApprovedBy { get; set; }

        public int ReceiptId { get; set; }

        public string ReceiptType { get; set; }

        public int LocationId { get; set; }

        public string Location { get; set; }

        public string SalesLocationId { get; set; }

        public DateTime DateReceived { get; set; }

        public int ReceivedById { get; set; }

        public string ReceivedBy { get; set; }

        public int VendorId { get; set; }

        public string Vendor { get; set; }

        public int GoodsReceiptId { get; set; }

        public string VendorDeliveryNumber { get; set; }

        public string VendorInvoiceNumber { get; set; }

        public string ReferenceNumber { get; set; }

        public List<VendorReturnProductViewModel> Products { get; set; }
    }
}
