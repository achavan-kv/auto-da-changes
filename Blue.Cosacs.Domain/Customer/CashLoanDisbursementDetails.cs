// -----------------------------------------------------------------------
// <copyright file="CashLoanDisbursementDetails.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Blue.Cosacs.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CashLoanDisbursementDetails
    {
        public string accountNo { get; set; }
        public string custId { get; set; }
        public decimal loanAmount { get; set; }
        public short disbursementType { get; set; }
        public string cardType {get; set;}
        public string chequeCardNo { get; set; }
        public string bankName {get; set; }
        public string bankAccountType { get; set; }
        public string bankBranch { get; set; }
        public string bankAccountNo { get; set; }
        public string notes {get; set; }
        public string bankReferenceNo { get; set; }
        public string bankAccountName { get; set; }
    }
}
