
using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.Credit
{
	partial class CheckLoanQualificationRequest  
	{
        public string CustId { get; set; }
        public int Branch { get; set; }
	}
	
	partial class CheckLoanQualificationResponse 
	{ 
		public string LoanQualified {get; set;}
	}
}
