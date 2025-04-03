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
        public SearchResponse Call(SearchRequest request)
        {
            EventStore.Instance.LogAsync(request, "Search", EventCategory.StoreCard, new { acctno = request.AcctNo, cardnumber = request.StoreCardNo });
            var r = new StoreCardRepository().Search(request);
            return new SearchResponse() { View_StoreCard = r };
        }
    }
}
