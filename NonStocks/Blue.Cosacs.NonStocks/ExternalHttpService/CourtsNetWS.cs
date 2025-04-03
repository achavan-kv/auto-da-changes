using Blue.Cosacs.NonStocks.Models;
using Blue.Cosacs.NonStocks.Models.WinCosacs;
using Blue.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Blue.Cosacs.NonStocks.ExternalHttpService
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

        //// TODO: Fix this, ExternalHttpSources.Get<T> not working :(
        //public List<HierarchyInfo> GetDboInfoHierarchy()
        //{
        //    var winCosacsHierarchy = ExternalHttpSources
        //        .Get<List<HierarchyInfo>>("/Courts.NET.WS/Hierarchy/Hierarchies", httpClientJson);

        //    return winCosacsHierarchy;
        //}

        public List<HierarchyInfo> GetDboInfoHierarchy()
        {
            var winCosacsHierarchy = new List<HierarchyInfo>();
            using (WebClient client = new WebClient())
            {
                client.Headers["User-Agent"] =
                "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
                "(compatible; MSIE 6.0; Windows NT 5.1; " +
                ".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                
                // TODO: Fix this, ExternalHttpSources.Get<T> not working :(
                //       It just doesn't work in your head...
                var hostName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                // Download data
                byte[] bytesArr = client.DownloadData(hostName + "/Courts.NET.WS/Hierarchy/Hierarchies");

                var jsonFileString = System.Text.Encoding.Default.GetString(bytesArr);
                var stringReader = new StringReader(jsonFileString);
                winCosacsHierarchy = (List<HierarchyInfo>)new Newtonsoft.Json.JsonSerializer()
                    .Deserialize(stringReader, (new List<HierarchyInfo>()).GetType());
            }

            return winCosacsHierarchy;
        }

        public SetupEodResult Fact2000SetupEODFile(object jsonObject)
        {
            var str = new System.Text.StringBuilder();

            new Newtonsoft.Json.JsonSerializer().Serialize(new StringWriter(str), jsonObject);
            var bodyRequestString = str.ToString();

            var setupEodResult = ExternalHttpSources
                .Get<SetupEodResult>(
                    "/Courts.NET.WS/Fact2000ImportFiles/SetupEODFile",
                    bodyRequestString,
                    httpClientJson);

            return setupEodResult;
        }

        public CodeMaintenanceUpdateResult UpdateCodeMaintenance(List<NonStockModel> nonStocks)
        {
            var str = new System.Text.StringBuilder();

            new Newtonsoft.Json.JsonSerializer().Serialize(new StringWriter(str), nonStocks);
            var bodyRequestString = str.ToString();

            var codeMaintenanceResult = ExternalHttpSources
               .Get<CodeMaintenanceUpdateResult>("/Courts.NET.WS/CodeMaintenance/UpdateCodeMaintenance",
               bodyRequestString,
               httpClientJson);

            return null;
        }

        public class SetupEodResult
        {
            public string systemDrive {get;set;}
        }

        public class CodeMaintenanceUpdateResult
        {
            public string result { get; set; }
        }

        public CourtsNetWS.CodeMaintenanceUpdateResult UpdateCodeMaintenance()
        {
            throw new NotImplementedException();
        }
    }
}
