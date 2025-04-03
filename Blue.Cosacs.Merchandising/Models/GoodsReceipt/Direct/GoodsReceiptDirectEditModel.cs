namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class GoodsReceiptDirectCreateModel
    {
        public GoodsReceiptDirectCreateModel()
        {
            Products = new List<GoodsReceiptDirectProductCreateModel>();
        }

        [Range(1, int.MaxValue)]
        [Required]
        public int? LocationId { get; set; }

        [Required]
        public int? ReceivedById { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int? VendorId { get; set; }

        public string VendorDeliveryNumber { get; set; }

        public string VendorInvoiceNumber { get; set; }

        [Required]
        public DateTime? DateReceived { get; set; }

        public string Comments { get; set; }

        public List<StringKeyValue> ReferenceNumbers { get; set; }

        public List<GoodsReceiptDirectProductCreateModel> Products { get; set; } 
    }
}
