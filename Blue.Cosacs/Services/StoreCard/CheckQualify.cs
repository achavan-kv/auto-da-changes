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
        public CheckQualifyResponse Call(CheckQualifyRequest request)
        {
            EventStore.Instance.Log(request, "CustomerDetailsQualifyCheck", EventCategory.StoreCard, new { Custid = request.custid});
            return new StoreCardRepository().CheckQualified (request);
          
        }
    }
}
