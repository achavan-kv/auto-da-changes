using Blue.Admin;
using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Base = Blue.Cosacs.Credit.Model.SanctionStage1;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class SanctionStage1Controller : ApiController
    {
        private readonly IClock clock;
        private readonly IEventStore audit;
        private readonly ISanctionStage1Repository repository;

        public SanctionStage1Controller(IClock clock, IEventStore audit, ISanctionStage1Repository repository)
        {
            this.clock = clock;
            this.audit = audit;
            this.repository = repository;
        }

        [Permission(PermissionsEnum.EditProposal)]
        public HttpResponseMessage Put(int id, Base.SanctionStage1 proposal)
        {
            if (proposal.IsApplicant2)
            {
                repository.SaveSanctionStage1Applicant2(id, proposal, this.GetUser().Id, clock.Now);
                audit.Log(proposal, EventType.SanctionStage1Applicant2Saved, EventCategory.Proposal);
                return Request.CreateResponse();
            }

            repository.SaveSanctionStage1Applicant1(id, proposal, this.GetUser().Id, clock.Now);
            audit.Log(proposal, EventType.SanctionStage1Applicant1Saved, EventCategory.Proposal);
            return Request.CreateResponse();
        }

        public HttpResponseMessage Get([FromUri]int id, bool? isApplicant2)
        {
            if (this.GetUser().HasPermission(PermissionsEnum.EditProposal) && this.GetUser().HasPermission(PermissionsEnum.ViewProposal))
            {
                if (isApplicant2.Value)
                {
                    return Request.CreateResponse(repository.GetApplicant2Details(id));
                }
                return Request.CreateResponse(repository.GetApplicant1Details(id));
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}
