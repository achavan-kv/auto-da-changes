using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.Branch
{
	partial class CashierTotalsSaveRequest  
	{
        public List<CashierTotalsBreakdown> breakdown;
        
        public int employee {get; set;}
        public DateTime datefrom {get; set;}
        public DateTime dateto { get; set; }
        public int authorisdedby {get;set;}
        public bool canReverse {get; set;}
        public decimal UserTotal{get; set;} 
        public decimal SystemTotal{get; set;} 
        public decimal TotalDifference{get; set;} 
        public decimal DepositTotal{get; set;}
        public short branch {get; set;}
                                		// put your properties/fields here
	}
	
	partial class CashierTotalsSaveResponse 
	{ 
		// put your properties/fields here
        public string result { get; set; }
	}
}
