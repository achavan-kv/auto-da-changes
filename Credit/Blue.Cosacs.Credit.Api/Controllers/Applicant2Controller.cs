using Blue.Admin;
using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Base = Blue.Cosacs.Credit;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class Applicant2Controller : ApiController
    {
        private IClock clock;
        private IEventStore audit;
        private IProposalRepository repository;

        public Applicant2Controller(IClock clock, IEventStore audit, IProposalRepository repository)
        {
            this.clock = clock;
            this.audit = audit;
            this.repository = repository;
        }

        public HttpResponseMessage Get(int id)
        {
            if (this.GetUser().HasPermission(PermissionsEnum.EditProposal) && this.GetUser().HasPermission(PermissionsEnum.ViewProposal))
            {
                return Request.CreateResponse(repository.GetBasicDetailsApplicant2(id));
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [Permission(PermissionsEnum.EditProposal)]
        public HttpResponseMessage Post(Base.Model.ProposalApplicant2 prop)
        {
            repository.SaveNewApplicant2(prop, this.GetUser().Id, clock.Now);
            audit.Log(prop, EventType.BasicDetailsApplicant2Saved, EventCategory.Proposal);
            return Request.CreateResponse();
        }

        [Permission(PermissionsEnum.EditProposal)]
        public HttpResponseMessage Put(int id, Base.Model.ProposalApplicant2 prop)
        {
            repository.SaveApplicant2(id, prop, this.GetUser().Id, clock.Now);
            audit.Log(prop, EventType.BasicDetailsApplicant2Saved, EventCategory.Proposal);
            return Request.CreateResponse();
        }
    }
}
