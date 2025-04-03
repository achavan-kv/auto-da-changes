namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class VendorReturnNewModel
    {
        public VendorReturnNewModel()
        {
            PurchaseOrders = new List<VendorReturnPurchaseOrderNewModel>();
        }

        public int GoodsReceiptId { get; set; }

        public string ReceiptType { get; set; }

        public int LocationId { get; set; }

        public string Location { get; set; }

        public DateTime DateReceived { get; set; }

        public string VendorDeliveryNumber { get; set; }

        public string VendorInvoiceNumber { get; set; }

        public string ReceivedBy { get; set; }

        public List<VendorReturnPurchaseOrderNewModel> PurchaseOrders { get; set; }
    }
}
