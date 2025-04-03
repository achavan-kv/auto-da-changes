namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class VendorReturnDirectNewModel
    {
        public VendorReturnDirectNewModel()
        {
            Products = new List<VendorReturnProductNewModel>();
        }

        public int GoodsReceiptId { get; set; }

        public string ReceiptType { get; set; }

        public int LocationId { get; set; }

        public string Location { get; set; }

        public DateTime DateReceived { get; set; }

        public string VendorDeliveryNumber { get; set; }

        public string VendorInvoiceNumber { get; set; }

        public string ReceivedBy { get; set; }

        public string Vendor { get; set; }

        public int VendorId { get; set; }

        public List<VendorReturnProductNewModel> Products { get; set; }
    }
}
