using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Glaucous.Client.Api;
using Blue.Networking;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    public class SummaryTableDataController : ApiController
    {
        private readonly IClock clock;
        private readonly HttpClientJson httpClientJson;
        private readonly ISalesManagementRepository repository;

        public SummaryTableDataController(IClock clock, HttpClientJson httpClientJson, ISalesManagementRepository repository)
        {
            this.clock = clock;
            this.httpClientJson = httpClientJson;
            this.repository = repository;
        }

        [CronJob]
        public string Get()
        {
            var date = this.clock.Now;
            var data = ExternalHttpSources.Post<Hashtable>(string.Format("/Courts.NET.WS/SalesManagement/SummaryTableData?todayDate={0}", date.ToSolrDate()), httpClientJson);
            var values = new List<Blue.Cosacs.SalesManagement.SummaryTable>();

            foreach (DictionaryEntry item in data)
            {
                values.AddRange(((Newtonsoft.Json.Linq.JArray)item.Value).ToObject<SalesSummary[]>()
                    .Select(p => new Blue.Cosacs.SalesManagement.SummaryTable
                    {
                        BranchId = p.BranchNo,
                        Amount = p.Amount,
                        Date = date,
                        Matrix = item.Key.ToString(),
                        SalesPersonId = p.SalesPerson
                    }));
            }

            repository.InsertSummaryTable(values);

            return DateTime.Now.ToString();
        }
    }
}