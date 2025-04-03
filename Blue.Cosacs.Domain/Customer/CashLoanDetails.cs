using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public class CashLoanDetails
    {
    
        public decimal loanAmount { get; set; }
        public int term { get; set; }
        public decimal instalment { get; set; }
        public decimal finInstal { get; set; }
        public decimal serviceChg { get; set; }
        public decimal insuranceChg { get; set; }
        public decimal adminChg { get; set; }
        public decimal agreementTotal { get; set; }
        public string accountNo { get; set; }
        public string custId { get; set; }
        public string termsType { get; set; }
        public string scoreBand { get; set; }
        public decimal taxRate { get; set; }
        public decimal insuranceTax { get; set; }
        public decimal adminTax { get; set; }
        public string loanStatus { get; set; }
        public string custName { get; set; }
        public int? empeenoAccept { get; set; }
        public int? empeenoDisburse { get; set; }
        public int transrefno { get; set; }
        public decimal outstBal { get; set; }
        public DateTime firstInstalDate { get; set; }
        public DateTime dateprop { get; set; }              // #8487
        public string unclearStage { get; set; }            // #8487
        public DateTime? datePrinted { get; set; }           // #8491
        public string referralReasons { get; set; }         //IP - 24/02/12 - #9598 - UAT 87
        public decimal stampDuty { get; set; }               // #10013
        public string cashLoanPurpose {get; set; }          //#19337 - CR18568
        public bool waiveAdminCharge {get; set; }
        public int? empeenoAdminChargeWaived { get; set; }
        public int? empeenoLoanAmountChanged { get; set; }
        public string Bank { get; set; }
        public string BankAccountType { get; set; }
        public string BankBranch { get; set; }
        public string BankAccountNo { get; set; }
        public string Notes { get; set; }
        public string BankReferenceNumber { get; set; }
        public string BankAccountName { get; set; }

    }
}
