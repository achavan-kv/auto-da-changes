using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class GetCashandGoReturnRequest  
	{
        public string AcctNo { get; set; }
        public int AgreementNo { get; set; }
	}
	
	partial class GetCashandGoReturnResponse 
	{
        public CustDetails CustDetails { get; set; }
        public List<View_FintransPayMethod> Payments { get; set; }
        public StoreCardValidated StoreCardValidated { get; set; }
	}
}
