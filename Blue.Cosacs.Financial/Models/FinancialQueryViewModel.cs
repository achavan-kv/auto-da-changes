namespace Blue.Cosacs.Financial.Models
{
    using System;

    public class FinancialQueryViewModel
    {
        public int TransactionId { get; set; }
        public short? RunNumber { get; set; }
        public string AccountNumber { get; set; }
        public string Location { get; set; }
        public int LocationId { get; set; }
        public string TransactionCode { get; set; }
        public decimal? TransactionValue { get; set; }
        public DateTime TransactionDate { get; set; }
        public string OriginalTransactionId { get; set; }
    }
}
