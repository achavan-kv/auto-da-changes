namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class GoodsReceiptViewModel
    {
        public GoodsReceiptViewModel()
        {
            PurchaseOrders = new List<GoodsReceiptPurchaseOrderViewModel>();
        }

        public int Id { get; set; }

        public int? LocationId { get; set; }
        
        public string Location { get; set; }

        public string SalesLocationId { get; set; }

        public int? ReceivedById { get; set; }

        public string ReceivedBy { get; set; }

        public int? CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public int? ApprovedById { get; set; }

        public string ApprovedBy { get; set; }

        public string VendorDeliveryNumber { get; set; }

        public string VendorInvoiceNumber { get; set; }

        public DateTime? DateReceived { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public DateTime? DateApproved { get; set; }

        public int? CostConfirmedById { get; set; }

        public string CostConfirmedBy { get; set; }

        public DateTime? CostConfirmed { get; set; }

        public string Comments { get; set; }

        public bool EnableBackOrderCancel { get; set; }

        public List<GoodsReceiptPurchaseOrderViewModel> PurchaseOrders { get; set; }

        public string VendorType { get; set; }
    }
}
