using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Service;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.Service
{
	partial class Server 
    {
        public GetItemsIssuedForReplacementResponse Call(GetItemsIssuedForReplacementRequest request)
        {
            // TODO write your stuff here
            return new ServiceRepository().GetItemsForReplacement(Convert.ToInt16(request.Branch));
        }
    }
}
