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
        public GetAccountStatusResponse Call(GetAccountStatusRequest request)
        {
            EventStore.Instance.LogAsync(request, "AccountDetailsStoreCardStatus", EventCategory.StoreCard, new { Acctno = request.acctno });
            return new GetAccountStatusResponse
            {
                status = new StoreCardRepository().GetAccountStatus(request.acctno)
             
            };
        }
    }
}
