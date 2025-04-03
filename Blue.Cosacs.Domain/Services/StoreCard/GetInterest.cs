using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class GetInterestRequest  
	{
        public string acctno { get; set; }
	}
	
	partial class GetInterestResponse 
	{
        public double Interest { get; set; }
        public decimal? MinimumPayment { get; set; }
    }
}
