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
        public ActivateResponse Call(ActivateRequest request)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
           {
               EventStore.Instance.Log(transaction,request, "CardActivation", EventCategory.StoreCard, new { CardNumber = request.CardNumber });
               return new StoreCardRepository().Activate(request, connection, transaction,ctx);
           });
        }
    }
}
