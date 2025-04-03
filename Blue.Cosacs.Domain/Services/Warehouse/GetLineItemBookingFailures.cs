using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.Warehouse
{
	partial class GetLineItemBookingFailuresRequest  
	{ 
		// put your properties/fields here
        public int? Branch { get; set; }
        public int Salesperson { get; set; }
	}
	
	partial class GetLineItemBookingFailuresResponse 
	{
        public List<BookingFailuresView> Failures { get; set; }
	}
}
