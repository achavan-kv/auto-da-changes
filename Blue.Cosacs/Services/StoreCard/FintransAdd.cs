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
        public FintransAddResponse Call(FintransAddRequest request)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
                     {
                         EventStore.Instance.Log(transaction,request, "PrintStatementAddFee", EventCategory.StoreCard, new { custid = request.custid, acctno = request.acctno });
                         new StoreCardRepository().AddTransaction(request.acctno, Convert.ToDouble(request.value), DateTime.Now, request.code, request.custid,conn:connection,trans:transaction,context:ctx);
                         return new FintransAddResponse();
                     });
        }
    }
}
