using STL.Common.Services.Model;
using System.Collections.Generic;
using System.Data;

namespace Blue.Cosacs.Shared.Services.Warranty
{
	partial class GetWarrantyRenewalsRequest  
	{
		public string AccountNumber { get; set; }
	}
	
	partial class GetWarrantyRenewalsResponse 
	{
		public DataSet WarrantyRenewal { get; set; }
	}
}
