using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.StoreCard;
using Blue.Cosacs.Repositories;
using System.Data.SqlClient;
using Blue.Cosacs.Shared;

namespace Blue.Cosacs.Services.StoreCard
{
    partial class Server
    {
        public CancelResponse Call(CancelRequest request)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
         {
             EventStore.Instance.Log(transaction,request.StoreCardHistory, "CancelCard", EventCategory.StoreCard, new { Acctno = request.StoreCardHistory.Acctno, CardNumber = request.StoreCardHistory.CardNumber });
             return new StoreCardRepository().CancelCard(request.StoreCardHistory,ctx,connection,transaction);
         });
        }
    }
}
