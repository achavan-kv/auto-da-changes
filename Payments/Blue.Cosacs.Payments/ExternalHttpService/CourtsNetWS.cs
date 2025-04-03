using Blue.Cosacs.Payments.Models;
using Blue.Cosacs.Payments.Models.WinCosacs;
using Blue.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Blue.Cosacs.Payments.ExternalHttpService
{
    public class CourtsNetWS : ICourtsNetWS
    {
        private readonly IHttpClientJson httpClientJson;

        public CourtsNetWS(IHttpClientJson httpClientJson)
        {
            this.httpClientJson = httpClientJson;
        }

        public List<BranchInfo> GetDboInfoBranches()
        {
            var winCosacsBrancheLookup = ExternalHttpSources
                .Get<List<BranchInfo>>("/Courts.NET.WS/DBOInfo/Branches", httpClientJson);
            return winCosacsBrancheLookup;
        }

    }
}
