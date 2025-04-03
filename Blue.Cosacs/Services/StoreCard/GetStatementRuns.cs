using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.StoreCard;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.StoreCard
{
	partial class Server 
    {
        public GetStatementRunsResponse Call(GetStatementRunsRequest request)
        {

            return new GetStatementRunsResponse()
            {
                StatementRuns = new StoreCardRepository().GetStatementRuns()
            };
 
        }
    }
}
