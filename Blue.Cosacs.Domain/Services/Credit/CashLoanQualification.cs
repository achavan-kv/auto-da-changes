using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Blue.Cosacs.Shared.Services.Credit
{
	partial class CashLoanQualificationRequest  
	{
        public string CustId { get; set; }
        public int Branch { get; set; }

	}
	
	partial class CashLoanQualificationResponse 
	{
        public Customer Customer { get; set; }
        public List<CustTel> CustTel { get; set; }
        public CustAddress CustAddress { get; set; }
        public bool Qualified { get; set; }
        public string LoanQual { get; set; }
        //public TermsTypeTable TermsTypeTable { get; set; }
        public List<TermsTypeAllBands> TermsType { get; set; }
        public bool PendingLoan { get; set; }
        public CashLoan Cashloan { get; set; }
        public bool DAed { get; set; }
        public DataSet Referral { get; set; }
        public DateTime Dateprop { get; set; }          // #8487
        public bool LoanAllowed { get; set; }
        public decimal StampDuty { get; set; }          // #10013
        public decimal TotalCreditUsed { get; set; }    //Added RahulSonawane 10.7 CR CashLoan
        public bool IsMambuAccount { get; set; }
    }
}
