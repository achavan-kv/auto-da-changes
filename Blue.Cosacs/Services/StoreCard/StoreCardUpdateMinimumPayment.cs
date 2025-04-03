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
        public StoreCardUpdateMinimumPaymentResponse Call(StoreCardUpdateMinimumPaymentRequest request)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
         {
             EventStore.Instance.Log(transaction, request, "SaveMinPayment", EventCategory.StoreCard, new { acctno = request.AcctNo });
             return new StoreCardRepository().UpdateMinimumPayment(request,connection,transaction,ctx);
         });
          
        }
    }
}
