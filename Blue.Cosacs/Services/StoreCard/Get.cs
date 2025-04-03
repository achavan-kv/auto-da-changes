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
        public GetResponse Call(GetRequest request)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
               {
                   EventStore.Instance.Log(transaction,request, "ViewStoreCard", EventCategory.StoreCard, new { Acctno = request.Acctno });
                   return new GetResponse()
                   {
                       StoreCardAccountResult = new StoreCardRepository().GetStoreCardAccountResult(request.Acctno,connection,transaction,ctx)
                   };
               });
        }
    }
}
