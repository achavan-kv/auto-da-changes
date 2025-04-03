using Blue.Cosacs.Credit.Repositories.Reindex;
using Blue.Glaucous.Client.Api;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class TermsTypeReindexController : ApiController
    {
        private const string TermsType = "TermsType";
        private readonly ITermsTypeSolrIndex termstypeIndex;

        public TermsTypeReindexController(ITermsTypeSolrIndex termstypeIndex)
        {
            this.termstypeIndex = termstypeIndex;
        }

        [Permission(PermissionsEnum.ReIndexTermsType)]
        [LongRunningQueries]
        [CronJob]
        public HttpResponseMessage Get()
        {
            try
            {
                new Blue.Solr.WebClient().DeleteByType(TermsType);
            }
            catch (Exception)
            {
            }
            return Request.CreateResponse(new { count = termstypeIndex.Reindex() });
        }
    }
}
