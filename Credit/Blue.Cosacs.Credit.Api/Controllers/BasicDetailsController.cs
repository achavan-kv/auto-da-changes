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
    public class BasicDetailsController : ApiController
    {
        private readonly IClock clock;
        private readonly IEventStore audit;
        private readonly IProposalRepository repository;

        public BasicDetailsController(IClock clock, IEventStore audit, IProposalRepository repository)
        {
            this.clock = clock;
            this.audit = audit;
            this.repository = repository;
        }

        public HttpResponseMessage Get(int id)
        {
            if (this.GetUser().HasPermission(PermissionsEnum.EditProposal) && this.GetUser().HasPermission(PermissionsEnum.ViewProposal))
            {
                return Request.CreateResponse(new { proposal = repository.GetBasicDetailsApplicant1(id) });
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [Permission(PermissionsEnum.EditProposal)]
        public HttpResponseMessage Post(Base.Model.ProposalApplicant1 proposal)
        {
            proposal.Applicant1.Branch = this.GetUser().Branch;
            proposal.Applicant1.CreatedBy = this.GetUser().Id;
            proposal.Applicant1.CreatedOn = clock.Now;
            proposal.Applicant1.Source = Constants.ProposalSource.Credit;

            proposal.UpdatedBy = this.GetUser().Id;
            proposal.UpdatedOn = clock.Now;

            var id = repository.SaveNewProposal(proposal);
            audit.Log(proposal, EventType.BasicDetailsCreated, EventCategory.Proposal);

            return Request.CreateResponse(new { Message = new { Id = id } });
        }

        [Permission(PermissionsEnum.EditProposal)]
        public HttpResponseMessage Put(int id, Base.Model.ProposalApplicant1 proposal)
        {
            proposal.Applicant1.UpdatedBy = this.GetUser().Id;
            proposal.Applicant1.UpdatedOn = clock.Now;

            repository.SaveProposal(id, proposal);
            audit.Log(proposal, EventType.BasicDetailsSaved, EventCategory.Proposal);
            return Request.CreateResponse();
        }
    }
}
