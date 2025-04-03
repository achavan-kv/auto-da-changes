using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Financial;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.Financial
{
	partial class Server 
    {
        public GetPaymentsResponse Call(GetPaymentsRequest request)
        {
            return new GetPaymentsResponse
            {
                Payments = new FinancialRepository().GetPayments(request.AcctNo, request.AgreementNo)
            };
        }
    }
}
