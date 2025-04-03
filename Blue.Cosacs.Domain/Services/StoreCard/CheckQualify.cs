using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class CheckQualifyRequest  
	{ 
		// put your properties/fields here
        public string custid { get; set; }
	}
	
	partial class CheckQualifyResponse 
	{
        public bool qualified { get; set; }
		// put your properties/fields here
	}
}
