using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.Credit
{
	partial class CashLoanCreateAccountRequest  
	{
        public string CustId { get; set; }
        public string CountryCode { get; set; }
        public short BranchCode { get; set; }
        public int User { get; set; }
       
	}
	
	partial class CashLoanCreateAccountResponse 
	{ 
		// put your properties/fields here
	}
}
