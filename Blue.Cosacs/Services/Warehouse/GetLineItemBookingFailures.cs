using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Warehouse;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.Warehouse
{
	partial class Server 
    {
        public GetLineItemBookingFailuresResponse Call(GetLineItemBookingFailuresRequest request)
        {
            // TODO write your stuff here
            return new WarehouseRepository().GetBookingFailures(request.Branch, request.Salesperson);       // #10618 - add Salesperson dropdown
        }
    }
}
