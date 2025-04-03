using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class CancelRequest  
	{
        public View_StoreCardHistory StoreCardHistory { get; set; }
	}
	
	partial class CancelResponse 
	{
        public List<View_StoreCardHistory> StoreCardHistory { get; set; }
        public List<View_StoreCardWithPayments> View_StoreCardWithPayments { get; set; }
	}
}
