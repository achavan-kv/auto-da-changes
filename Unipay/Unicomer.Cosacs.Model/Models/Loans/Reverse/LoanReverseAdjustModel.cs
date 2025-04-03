using System;
using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Loans.Reverse
{
    public class LoanReverseAdjustResponseModel
    {
        public LoanReverseAdjustModel Data { get; set; }
        public ApiResult Result { get; set; }
    }

    public class LoanReverseAdjustModel
    {
        public string EncodedKey { get; set; }
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ValueDate { get; set; }
        public DateTime BookingDate { get; set; }
        public string Notes { get; set; }
        public string ParentAccountKey { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public LoanReverseAffectedAmount AffectedAmounts { get; set; }
        public LoanReverseAffectedAmountTax Taxes { get; set; }
        public LoanReverseAccountBalance AccountBalances { get; set; }
        public string UserKey { get; set; }
        public string AdjustmentTransactionKey { get; set; }
        public LoanReverseTerm Terms { get; set; }
        public LoanReverseTransactionDetail TransactionDetails { get; set; }
        public List<object> Fees { get; set; }
        public Currency Currency { get; set; }
    }
}