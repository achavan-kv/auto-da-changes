namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;

    public class VendorReturnProductCreateModel
    {
        public int GoodsReceiptProductId { get; set; }
  
        [Range(0, int.MaxValue)]
        public int? QuantityReturned { get; set; }

        [StringLength(100)]
        public string Comments { get; set; }
    }
}