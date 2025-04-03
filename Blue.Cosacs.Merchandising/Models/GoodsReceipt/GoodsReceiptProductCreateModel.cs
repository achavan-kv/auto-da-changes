namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;

    public class GoodsReceiptProductCreateModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Range(0, int.MaxValue)]
        [Required]
        public int? QuantityReceived { get; set; }

        [Range(0, int.MaxValue)]
        public int? QuantityBackOrdered { get; set; }

        [Range(0, int.MaxValue)]
        public int? QuantityCancelled { get; set; }

        [StringLength(100)]
        public string ReasonForCancellation { get; set; }
    }
}