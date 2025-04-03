using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Credit;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.Credit
{
	partial class Server 
    {
        public UpdateCashLoanBlockedResponse Call(UpdateCashLoanBlockedRequest request)
        {
            return new CreditRepository().UpdateCashLoanBlocked(request.CustId, request.BlockedStatus);
        }
    }
}
