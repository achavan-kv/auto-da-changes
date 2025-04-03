using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared.Services.CosacsConfig;

namespace Blue.Cosacs.Services.CosacsConfig
{
	partial class Server 
    {
        public CheckBranchResponse Call(CheckBranchRequest request)
        {
            return new CheckBranchResponse { BranchExists = new ConfigRepository().CheckBranch(request.BranchNo) };
        }
    }
}
