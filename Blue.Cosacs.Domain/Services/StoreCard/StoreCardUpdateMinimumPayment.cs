using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class StoreCardUpdateMinimumPaymentRequest  
	{ 
		// put your properties/fields here
        public string AcctNo { get; set; }
        public decimal MinimumPayment { get; set; }
        public decimal MonthlyAmount { get; set; }      // #9841 jec 27/03/12 set fixed payment
	}
	
	partial class StoreCardUpdateMinimumPaymentResponse 
	{
        public decimal calcMinPayment { get; set; }
	}
}
