namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class GoodsReceiptCostsModel
    {
        public GoodsReceiptCostsModel()
        {
            PurchaseOrders = new List<GoodsReceiptCostsPurchaseOrderModel>();
        }

        public int Id { get; set; }

        public string Location { get; set; }
        public int LocationId { get; set; }

        public int ReceivedById { get; set; }

        public string ReceivedBy { get; set; }

        public int? ProcessedById { get; set; }

        public string ProcessedBy { get; set; }

        public int? ApprovedById { get; set; }

        public string ApprovedBy { get; set; }

        public string CostConfirmedBy { get; set; }

        public DateTime? CostConfirmed { get; set; }

        public bool Confirmed
        {
            get 
            {
                return CostConfirmed.HasValue;
            }
        }

        public string VendorDeliveryNumber { get; set; }

        public string VendorInvoiceNumber { get; set; }

        public string Comments { get; set; }

        public DateTime DateReceived { get; set; }
        
        public DateTime? DateApproved { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public string CreatedBy { get; set; }

        public List<GoodsReceiptCostsPurchaseOrderModel> PurchaseOrders { get; set; } 
    }
}
