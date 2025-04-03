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
        public GetValidateCardResponse Call(GetValidateCardRequest request)
        {
               return Context.ExecuteTx((ctx, connection, transaction) =>
               {
                   EventStore.Instance.Log(transaction,request, "PaymentValidation", EventCategory.StoreCard, new { CardNo = request.CardNo });
                   return new GetValidateCardResponse
                   {
                       StoreCardValidated = StoreCardValidation.ValidateCard(new StoreCardRepository().Validate(request.CardNo,connection,transaction,ctx))
                   };
            });
        }
    }
}
