using Blue.Cosacs.Sales.Models;
using System.Collections.Generic;

namespace Blue.Cosacs.Sales.Repositories
{
    public interface ISalesLookupRepository
    {
        IEnumerable<BranchDetails> GetBranches(int userId);
        BranchDetails GetBranches(int userId, short branchNo);
        //HiLo GetHiLoGeneratedId(string module, int userId);
        string GetBranchName(short id, IEnumerable<BranchDetails> branchList);
        string GetBranchName(int userId, short id);
        string GetBranchFascia(int userId, int id);
    }
}