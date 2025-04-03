using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class CreateRequest  
	{
        public StoreCardNew storeCardNew { get; set; }
	}

    partial class CreateResponse : Shared.StoreCard 
	{
        public string AccountStatus { get; set; }
        public CustAddress CustAddress { get; set; }
        public DateTime DOB { get; set; }
    }
}
