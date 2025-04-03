using Blue.Cosacs.Credit.Repositories;
using Blue.Glaucous.Client.Api;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class CustomerReindexController : ApiController
    {
        private const string Customer = "CustomerCredit";
        private readonly CustomerSolrIndex customerIndex;

        public CustomerReindexController(CustomerSolrIndex customerIndex)
        {
            this.customerIndex = customerIndex;
        }

        [Permission(PermissionsEnum.ReIndexCustomers)]
        [LongRunningQueries]
        [CronJob]
        public HttpResponseMessage Get()
        {
            try
            {
                new Blue.Solr.WebClient().DeleteByType(Customer);
            }
            catch (Exception)
            {
            }
            return Request.CreateResponse(new { count = customerIndex.Reindex() });
        }
    }
}
