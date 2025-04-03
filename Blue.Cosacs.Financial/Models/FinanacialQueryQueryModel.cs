namespace Blue.Cosacs.Financial.Models
{
    using System;

    public class FinanacialQueryQueryModel
    {
        public short? RunNumber { get; set; }
        public string AccountNumber { get; set; }
        public int? LocationId { get; set; }
        public string TransactionCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}