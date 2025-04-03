
using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Credit;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.Credit
{
	partial class Server 
    {
        public CheckLoanQualificationResponse Call(CheckLoanQualificationRequest request)
        {
            return new CreditRepository().CheckLoanQualification(request.CustId, request.Branch);
        }
    }
}
