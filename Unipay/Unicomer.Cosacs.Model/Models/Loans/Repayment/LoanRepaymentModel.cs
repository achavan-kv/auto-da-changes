using System;
using System.Collections.Generic;

namespace Unicomer.Cosacs.Model.Models.Loans.Repayment
{
    public class LoanRepaymentResponseModel
    {
        public LoanRepaymentModel Data { get; set; }
        public ApiResult Result { get; set; }
    }

    public class LoanRepaymentModel
    {
        public string EncodedKey { get; set; }
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ValueDate { get; set; }
        public DateTime BookingDate { get; set; }
        public string ParentAccountKey { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public RepaymentAffectedAmount AffectedAmounts { get; set; }
        public RepaymentTax Taxes { get; set; }
        public RepaymentAccountBalance AccountBalances { get; set; }
        public string UserKey { get; set; }
        public RepaymentTerm Terms { get; set; }
        public RepaymentTransactionDetail TransactionDetails { get; set; }
        public List<object> Fees { get; set; }
        public RepaymentCurrency Currency { get; set; }
    }
}
