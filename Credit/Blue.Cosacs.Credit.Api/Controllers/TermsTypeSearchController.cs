using Blue.Glaucous.Client.Api;
using Blue.Solr;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class TermsTypeSearchController : ApiController
    {
        public TermsTypeSearchController()
        {
        }

        [Permission(PermissionsEnum.TermsTypeSearch)]
        public HttpResponseMessage Get(string q, int start = 0, int rows = 25)
        {
            var solrQuery = new Query();
            var solrResult = solrQuery.SelectJsonWithJsonQuery(q, "Type:TermsType", start: start, rows: rows, facetFields: new[] { "TermsType", "CustomerType", "CashLoanCustomerType", "Fascia", "IsActive", "IsDisabled" }, indent: false);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(solrResult, Encoding.UTF8, "application/json");

            return response;
        }
    }
}
