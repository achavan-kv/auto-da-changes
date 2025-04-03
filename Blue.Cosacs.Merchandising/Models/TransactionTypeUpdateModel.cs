namespace Blue.Cosacs.Merchandising.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TransactionTypeUpdateModel
    {
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
        
        [MaxLength(10)]
        [StringLength(3, MinimumLength = 3)]
        public string TransactionCode { get; set; }
        
        [MaxLength(30)]
        [RegularExpression("([0-9]+)")]
        public string DebitAccount { get; set; }

        [MaxLength(30)]
        [RegularExpression("([0-9]+)")] 
        public string CreditAccount { get; set; }
        
        public bool SplitDebitByDepartment { get; set; }
        
        public bool SplitCreditByDepartment { get; set; }
    }
}