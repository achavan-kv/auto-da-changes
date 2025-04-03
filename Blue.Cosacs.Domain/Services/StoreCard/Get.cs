using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class GetRequest  
	{
        public string Acctno { get; set; }
	}
	
	partial class GetResponse 
	{
        public StoreCardAccountResult StoreCardAccountResult { get; set; }
	}
}
