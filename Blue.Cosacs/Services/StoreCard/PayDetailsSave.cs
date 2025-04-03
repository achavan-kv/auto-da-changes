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
        public PayDetailsSaveResponse Call(PayDetailsSaveRequest request)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
            {
                EventStore.Instance.Log(transaction, request.StorecardPaymentDetails, "SavePaymentDetails", EventCategory.StoreCard, new { acctno = request.StorecardPaymentDetails.acctno });
                new StoreCardRepository().SavePaymentDetails(request,connection,transaction,ctx);
                return new PayDetailsSaveResponse();
            });
            
        }
    }
}
