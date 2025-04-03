using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.StoreCard;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;

namespace Blue.Cosacs.Services.StoreCard
{
	partial class Server 
    {
        public GetCashandGoReturnResponse Call(GetCashandGoReturnRequest request)
        {
            EventStore.Instance.LogAsync(request, "CashAndGoReturn", EventCategory.CashAndGo, new { Acctno = request.AcctNo });
            var payments = new FinancialRepository().GetPayments(request.AcctNo,request.AgreementNo);
            var storecardpayment = payments.Find(p => p.storecardno.HasValue);

            var scRepos = new StoreCardRepository();
            CustDetails custdetails = null;
            StoreCardValidated scvalidation = null;

            if (storecardpayment != null && storecardpayment.storecardno.HasValue)
            {
                custdetails = scRepos.GetCustDetails(storecardpayment.storecardno.Value);
                scvalidation = StoreCardValidation.ValidateCard(scRepos.Validate(storecardpayment.storecardno.Value));
            };

            return new GetCashandGoReturnResponse()
            {
                Payments = payments,
                CustDetails = custdetails,
                StoreCardValidated = scvalidation
            };

        }
    }
}
