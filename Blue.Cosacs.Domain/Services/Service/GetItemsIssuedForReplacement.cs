using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.Service
{
	partial class GetItemsIssuedForReplacementRequest  
	{ 
		// put your properties/fields here
        public int Branch { get; set; }
	}
	
	partial class GetItemsIssuedForReplacementResponse 
	{
        public List<BERItemsForReplacementView> itemsForReplacement { get; set; }
	}
}
