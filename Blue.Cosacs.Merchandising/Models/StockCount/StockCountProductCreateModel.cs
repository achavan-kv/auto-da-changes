namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;

    public class StockCountProductUpdateModel
    {
        [Range(1, int.MaxValue)]
        [Required]
        public int? Id { get; set; }

        [Required]
        public int? Count { get; set; }

        [Required]
        public int? SystemAdjustment { get; set; }

        public string Comments { get; set; }
    }
}