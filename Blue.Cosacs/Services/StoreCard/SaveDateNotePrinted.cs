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
        public SaveDateNotePrintedResponse Call(SaveDateNotePrintedRequest request)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
         {
             EventStore.Instance.Log(transaction, request, "AgreementPrint", EventCategory.StoreCard, new { acctno = request.AcctNo });
             new StoreCardRepository().SaveDateNotedPrinted(request,connection,transaction,ctx);
             return new SaveDateNotePrintedResponse();
         });
        }
    }
}
