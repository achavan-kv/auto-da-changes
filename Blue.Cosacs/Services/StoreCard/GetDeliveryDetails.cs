using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.StoreCard;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.StoreCard
{
	partial class Server 
    {
        public GetDeliveryDetailsResponse Call(GetDeliveryDetailsRequest request)
        {
           
            var Response = new GetDeliveryDetailsResponse();
            try
            {
                Response =
                new StoreCardRepository().GetDeliveryDetails(request.AcctNo, request.Agrmtno);
            }
            catch
            { //don't care as not important to return anything. 
            }
             
            return Response;
             
        }
    }
}
