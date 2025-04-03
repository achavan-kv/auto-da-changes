using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class GetValidatedCardForRecieptRequest  
	{
        public long CardNo { get; set; }
	}
	
	partial class GetValidatedCardForRecieptResponse 
	{
        public StoreCardValidated StoreCardValidated { get; set; }
        public CustDetails CustDetails { get; set; }
	}
}
