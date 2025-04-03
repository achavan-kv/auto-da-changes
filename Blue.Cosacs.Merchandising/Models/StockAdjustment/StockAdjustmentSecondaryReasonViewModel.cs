namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;

    public class StockAdjustmentSecondaryReasonViewModel
    {
        [Range(1, int.MaxValue)]
        public int? Id { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int? PrimaryReasonId { get; set; }

        [Required]
        public string SecondaryReason { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string TransactionCode { get; set; }

        [Required]
        [MaxLength(30)]
        [RegularExpression("([0-9]+)")]
        public string DebitAccount { get; set; }

        public bool SplitDebitByDepartment { get; set; }

        [Required]
        [MaxLength(30)]
        [RegularExpression("([0-9]+)")]
        public string CreditAccount { get; set; }

        public bool SplitCreditByDepartment { get; set; }

        public bool DefaultForCountAdjustment { get; set; }
    }
}
