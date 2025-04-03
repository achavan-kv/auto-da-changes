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
        public CreateResponse Call(CreateRequest request)
        {

            return Context.ExecuteTx((ctx, connection, transaction) =>
           {
               EventStore.Instance.Log(transaction,request.storeCardNew, "CreateStoreCard", EventCategory.StoreCard, new { CustId = request.storeCardNew.CustId, AcctNo = request.storeCardNew.AcctNo });
               return new StoreCardRepository().Create(request,connection,transaction,ctx);
           });
        }
    }
}
