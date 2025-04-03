using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.StoreCard;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.StoreCard
{
	partial class Server 
    {
        public GetValidatedCardForRecieptResponse Call(GetValidatedCardForRecieptRequest request)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
               {
                   EventStore.Instance.Log(transaction, request, "CashAndGoValidation", EventCategory.StoreCard, new { CardNo = request.CardNo });
                 
                   var SCRep = new StoreCardRepository();
                   return new GetValidatedCardForRecieptResponse()
                   {
                       StoreCardValidated = StoreCardValidation.ValidateCard(SCRep.Validate(request.CardNo,connection, transaction, ctx)),
                       CustDetails = SCRep.GetCustDetails(request.CardNo, connection, transaction, ctx)
                   };
               });
        }
    }
}
