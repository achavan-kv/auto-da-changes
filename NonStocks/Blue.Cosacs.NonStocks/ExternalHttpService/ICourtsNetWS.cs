using Blue.Cosacs.NonStocks.Models;
using Blue.Cosacs.NonStocks.Models.WinCosacs;
using System.Collections.Generic;

namespace Blue.Cosacs.NonStocks.ExternalHttpService
{
    public interface ICourtsNetWS
    {
        List<BranchInfo> GetDboInfoBranches();
        List<HierarchyInfo> GetDboInfoHierarchy();
        CourtsNetWS.SetupEodResult Fact2000SetupEODFile(object jsonFileContent);
        CourtsNetWS.CodeMaintenanceUpdateResult UpdateCodeMaintenance(List<NonStockModel> nonStocks);
    }
}
