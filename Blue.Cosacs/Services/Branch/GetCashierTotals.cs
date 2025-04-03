using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Branch;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.Branch
{
	partial class Server 
    {
        public GetCashierTotalsResponse Call(GetCashierTotalsRequest request)
        {
            return  new CashierTotalRepository().LoadCashierTotals(request.DateFrom, request.Empeeno, request.dateto);
        }
    }
}
