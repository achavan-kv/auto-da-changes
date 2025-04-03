using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Branch;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.Branch
{
	partial class Server 
    {
        public GetBranchListResponse Call(GetBranchListRequest request)
        {
            return new GetBranchListResponse()
            {
                BranchList = new BranchRepository().GetBranchList()
            };
        }
    }
}
