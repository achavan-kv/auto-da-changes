using Blue.Cosacs.Payments.Models;
using Blue.Cosacs.Payments.Models.WinCosacs;
using System.Collections.Generic;

namespace Blue.Cosacs.Payments.ExternalHttpService
{
    public interface ICourtsNetWS
    {
        List<BranchInfo> GetDboInfoBranches();
    }
}
