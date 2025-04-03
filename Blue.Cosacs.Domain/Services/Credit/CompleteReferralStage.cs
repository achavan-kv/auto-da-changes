using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.Credit
{
	partial class CompleteReferralStageRequest  
	{ 
		// put your properties/fields here
        public string CustomerId {get;set;}
        public string AccountNo { get; set; }
        public DateTime DateProp { get; set; }
        public string RefNotes { get; set; }
        public string NewNotes { get; set; }
        public bool Approved { get; set; }
        public bool rejected { get; set; }
        public bool reOpen { get; set; }
        public short branchno { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CardLimit { get; set; }
        public int User { get; set; }
          
	}
	
	partial class CompleteReferralStageResponse 
	{
        public string Errormessage { get; set; }
	}
}
