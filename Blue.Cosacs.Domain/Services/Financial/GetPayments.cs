using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.Financial
{
	partial class GetPaymentsRequest  
	{
        public string AcctNo { get; set; }
        public int AgreementNo { get; set; }
	}
	
	partial class GetPaymentsResponse 
	{ 
		public List<View_FintransPayMethod> Payments { get; set; }
	}
}
