using Blue.Cosacs.Credit.Repositories;
using Blue.Glaucous.Client.Api;
using Blue.Solr;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class TermsTypeSimulatorController : ApiController
    {
        private ITermsTypeRepository repository;

        public TermsTypeSimulatorController(ITermsTypeRepository repository)
        {
            this.repository = repository;
        }

        [Permission(PermissionsEnum.TermsTypeSimulator)]
        public HttpResponseMessage Post(Model.TermsTypeSearch search)
        {
            return Request.CreateResponse(repository.Search(search));
        }
    }
}
