using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class FintransAddRequest  
	{
        public string acctno { get; set; }
        public decimal value { get; set; }
        public string branch { get; set; }
        public string code { get; set; }
        public string custid { get; set; }
		// put your properties/fields here
	}
	
	partial class FintransAddResponse 
	{ 
		// put your properties/fields here
        public string result { get; set; }
	}
}
