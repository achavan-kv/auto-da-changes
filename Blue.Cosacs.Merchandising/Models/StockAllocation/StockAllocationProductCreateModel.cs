namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;

    public class StockAllocationProductCreateModel
    {
        [Range(1, int.MaxValue)]
        [Required]
        public int? ProductId { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int? ReceivingLocationId { get; set; }

        [Required]
        public int? Quantity { get; set; }
    }
}