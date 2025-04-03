namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class GoodsReceiptCreateModel
    {
        public GoodsReceiptCreateModel()
        {
            PurchaseOrders = new List<GoodsReceiptPurchaseOrderCreateModel>();
        }

        [Range(1, int.MaxValue)]
        [Required]
        public int? LocationId { get; set; }

        [Required]
        public int? ReceivedById { get; set; }

        [MaxLength(100)]
        public string VendorDeliveryNumber { get; set; }

        [MaxLength(100)]
        public string VendorInvoiceNumber { get; set; }

        [Required]
        public DateTime? DateReceived { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        public List<GoodsReceiptPurchaseOrderCreateModel> PurchaseOrders { get; set; }
    }
}
