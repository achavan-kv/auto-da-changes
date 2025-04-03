using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class GetDeliveryDetailsRequest  
	{ 
		// put your properties/fields here
        public string AcctNo {get; set;}
        public int Agrmtno {get; set;}
	}
	
	partial class GetDeliveryDetailsResponse 
	{ 
		// put your properties/fields here 
        public List<view_LineDetails> Details = new List<view_LineDetails>();
         
	}
}
