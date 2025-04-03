
using System;
using System.Collections.Generic;
using System.Text;
using STL.Common.Services.Model;
using System.Data;

namespace Blue.Cosacs.Shared.Services.Warranty
{
	partial class GetAvailableWarrantiesRequest  
	{
        public string CustomerId { get; set; }
	}
	
	partial class GetAvailableWarrantiesResponse 
	{
        public WarrantyResult[] WarrantyResult { get; set; }
        public DataTable items {get; set;}
	}
}
