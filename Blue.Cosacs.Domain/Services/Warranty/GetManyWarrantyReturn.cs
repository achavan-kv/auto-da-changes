
using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.CosacsWeb.Models;

namespace Blue.Cosacs.Shared.Services.Warranty
{
	partial class GetManyWarrantyReturnRequest  
	{
       public List<WarrantyReturnList> returnInputs;
	}
	
	partial class GetManyWarrantyReturnResponse 
	{ 
		// put your properties/fields here
        public List<WarrantyReturnDetails> returnDetails;
	}
}
