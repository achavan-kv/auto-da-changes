namespace Blue.Cosacs.Merchandising.Models
{
    public class TransactionTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TransactionCode { get; set; }
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
        public bool SplitDebitByDepartment { get; set; }
        public bool SplitCreditByDepartment { get; set; }
    }
}