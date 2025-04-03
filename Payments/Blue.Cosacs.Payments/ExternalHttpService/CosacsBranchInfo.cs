using Blue.Cosacs.Payments.Models.WinCosacs;
using Blue.Networking;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Blue.Cosacs.Payments.ExternalHttpService
{
    public class CosacsBranchInfo
    {
        private CosacsBranchInfo() { }

        private static ReadOnlyCollection<BranchInfo> _singletonCosacsBranches = null;

        public static ReadOnlyCollection<BranchInfo> Get()
        {
            if (_singletonCosacsBranches == null)
            {
                var courtsNetWS = ObjectFactory.GetInstance<ICourtsNetWS>();
                var tmpBranchInfo = courtsNetWS.GetDboInfoBranches();
                if (tmpBranchInfo != null && tmpBranchInfo.Count > 0)
                {
                    _singletonCosacsBranches = new ReadOnlyCollection<BranchInfo>(tmpBranchInfo);
                }
            }

            return _singletonCosacsBranches;
        }

    }
}
