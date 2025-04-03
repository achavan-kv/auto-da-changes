namespace Blue.Cosacs.Merchandising.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class VendorReturnCreateModel
    {
        public VendorReturnCreateModel()
        {
            VendorReturnProducts = new List<VendorReturnProductCreateModel>();
        }

        [Required]
        public int? GoodsReceiptId { get; set; }
        
        [StringLength(500)]
        public string Comments { get; set; }

        [StringLength(50)]
        public string ReferenceNumber { get; set; }

        public List<VendorReturnProductCreateModel> VendorReturnProducts { get; set; }
    }
}
