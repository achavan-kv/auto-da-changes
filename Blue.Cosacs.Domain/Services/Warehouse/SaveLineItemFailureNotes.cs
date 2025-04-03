using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.Warehouse
{
	partial class SaveLineItemFailureNotesRequest  
	{
        public string AcctNo { get; set; }
        public string Notes { get; set; }
        public int EmpeeNo { get; set; }
	}
	
	partial class SaveLineItemFailureNotesResponse 
	{ 
		// put your properties/fields here
	}
}
