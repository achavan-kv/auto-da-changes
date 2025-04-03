using Blue.Cosacs.NonStocks.Solr;
using Blue.Glaucous.Client.Api;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Blue.Cosacs.NonStocks.Api.Controllers
{
    [RoutePrefix("Api/Indexing")]
    public class IndexingController : ApiController
    {
        private SolrUtils solrUtils = null;

        public IndexingController(SolrUtils solrUtils)
        {
            this.solrUtils = solrUtils;
        }

        private string GetCurrentUser()
        {
            var test = this.GetUser();

            return test.Login;
        }

        [Route("Search")]
        [HttpGet]
        [Permission(PermissionsEnum.NonStocksView)]
        public HttpResponseMessage Search(int branch, string q = "", int start = 0, int rows = 25)
        {
            var solrResponse = this.Request.CreateResponse(HttpStatusCode.OK);

            var responseJson = new SolrUtils().GetNonStocks(branch, q, start, rows);
            solrResponse.Content = new StringContent(responseJson, Encoding.UTF8, "application/json");

            return solrResponse;
        }

        [Route("ForceIndex")]
        [HttpGet]
        [Permission(PermissionsEnum.NonStocksView)]
        public HttpResponseMessage ForceIndex()
        {
            //var te = GetCurrentUser();
            try
            {
                solrUtils.DeleteAllSolrNonStockDocuments();
                SolrIndex.Index();
                return Request.CreateResponse(new { Result = "Done" });
            }
            catch(Exception e)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
