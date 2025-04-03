namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class VendorReturnPrintModel
    {
        public VendorReturnPrintModel()
        {
            PurchaseOrders = new List<VendorReturnPurchaseOrderPrintModel>();
        }

        public int Id { get; set; }

        public GoodsReceiptViewModel GoodsReceipt { get; set; }

        public string Comments { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public int CreatedById { get; set; }

        public string FormattedTotalCost { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string ApprovedBy { get; set; }

        public string ReferenceNumber { get; set; }

        public List<VendorReturnPurchaseOrderPrintModel> PurchaseOrders { get; set; }
    }
}
