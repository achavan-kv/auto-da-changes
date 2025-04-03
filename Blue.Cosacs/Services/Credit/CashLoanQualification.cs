using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Credit;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.Credit
{
	partial class Server 
    {
        public CashLoanQualificationResponse Call(CashLoanQualificationRequest request)
        {
            return new CreditRepository().CashLoanQualification(request.CustId,request.Branch);
        }
    }
}
