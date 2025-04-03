namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;

    public class GoodsOnLoanProductCreateModel
    {
        [Range(1, int.MaxValue)]
        [Required]
        public int? ProductId { get; set; }

        [Required]
        public int? Quantity { get; set; }

        public string ReferenceNumber { get; set; }

        [StringLength(100)]
        public string Comments { get; set; }
    }
}