using Blue.Cosacs.Credit.Model;
using Blue.Cosacs.Credit.Repositories;
using Blue.Glaucous.Client.Api;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class ProposalsSearchController : ApiController
    {
        private IProposalRepository repository;

        public ProposalsSearchController(IProposalRepository repository)
        {
            this.repository = repository;
        }

        [Permission(PermissionsEnum.ViewProposal)]
        public HttpResponseMessage Post(ProposalSearchParams searchParams)
        {
            return Request.CreateResponse(new { results = repository.Search(searchParams) });
        }
    }
}
