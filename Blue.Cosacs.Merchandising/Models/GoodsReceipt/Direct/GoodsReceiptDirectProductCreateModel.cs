namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;

    public class GoodsReceiptDirectProductCreateModel
    {
        public int ProductId { get; set; }

        [Range(0, int.MaxValue)]
        [Required]
        public int? QuantityReceived { get; set; }

        public string Comments { get; set; }
    }
}