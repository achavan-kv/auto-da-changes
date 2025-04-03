using Blue.Cosacs.Credit.Api.Models;
using Blue.Glaucous.Client.Api;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class CustomersController : ApiController
    {
        [Permission(PermissionsEnum.ViewProposal)]
        public HttpResponseMessage Get([FromUri] CustomerSolrSearch search)
        {
            var solrQuery = new Blue.Solr.Query();
            var solrResult = solrQuery.SelectJsonWithJsonQuery(
                            string.Empty,
                            BuildSolrMessage(search),
                            start: 0,
                            rows: 7,
                            sort: "LatestDate DESC",
                            showEmpty: false);

            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(solrResult, Encoding.UTF8, "application/json");
            return response;
        }

        private string BuildSolrMessage(CustomerSolrSearch search)
        {
            StringBuilder filterString = new StringBuilder("Type:CustomerCredit");
            if (!string.IsNullOrEmpty(search.FirstName))
            {
                filterString.AppendFormat(" AND FirstName:\"{0}*\"", search.FirstName);
            }
            if (!string.IsNullOrEmpty(search.LastName))
            {
                filterString.AppendFormat(" AND LastName:\"*{0}*\"", search.LastName);
            }
            if (!string.IsNullOrEmpty(search.HomePhone))
            {
                filterString.AppendFormat(" AND HomePhoneNumber:*{0}*", search.HomePhone);
            }
            if (!string.IsNullOrEmpty(search.MobilePhone))
            {
                filterString.AppendFormat(" AND MobileNumber:*{0}*", search.MobilePhone);
            }
            if (!string.IsNullOrEmpty(search.Email))
            {
                filterString.AppendFormat(" AND Email:*{0}*", search.Email);
            }
            return filterString.ToString();
        }
    }
}