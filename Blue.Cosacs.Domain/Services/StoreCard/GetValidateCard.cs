using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class GetValidateCardRequest  
	{
        public long CardNo { get; set; }
	}
	
	partial class GetValidateCardResponse 
	{
        public StoreCardValidated StoreCardValidated { get; set; }
	}
}
