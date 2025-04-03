namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;

    public class StockAdjustmentReasonCreateModel
    {
        [Required]
        public string PrimaryReason { get; set; }
    }
}
