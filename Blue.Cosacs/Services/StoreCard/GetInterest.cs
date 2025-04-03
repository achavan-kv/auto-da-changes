using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.StoreCard;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.StoreCard
{
	partial class Server 
    {
        public GetInterestResponse Call(GetInterestRequest request)
        {
            return new GetInterestResponse 
            { 
                Interest = new StoreCardRepository().GetInterest(request.acctno), 
                MinimumPayment = new StoreCardRepository().GetMinimumPayment(request.acctno) 
            };
        }
    }
}
