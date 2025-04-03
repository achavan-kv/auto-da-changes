using Blue.Admin;
using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class SanctionStage2Controller : ApiController
    {
        private readonly IEventStore audit;
        private readonly ISanctionStage2Repository repository;
        private readonly IClock clock;

        public SanctionStage2Controller(IEventStore audit, ISanctionStage2Repository repository, IClock clock)
        {
            this.audit = audit;
            this.clock = clock;
            this.repository = repository;
        }

        public HttpResponseMessage Get([FromUri]int id, bool? isApplicant2)
        {
            if (this.GetUser().HasPermission(PermissionsEnum.EditProposal) && this.GetUser().HasPermission(PermissionsEnum.ViewProposal))
            {
                if (!isApplicant2.Value)
                {
                    return Request.CreateResponse(repository.GetApplicant1Details(id, clock));
                }

                return Request.CreateResponse(repository.GetApplicant2Details(id));
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [Permission(PermissionsEnum.EditProposal)]
        public HttpResponseMessage Put(int id, Model.SanctionStage2.SanctionStage2 sanctionStage2)
        {
            if (!sanctionStage2.IsApplicant2)
            {
                repository.SaveSanctionStage2Applicant1(sanctionStage2.Id, sanctionStage2, sanctionStage2.IsApplicant2, this.GetUser().Id, clock.Now);
                audit.Log(sanctionStage2, EventType.SanctionStage2Applicant1Saved, EventCategory.Proposal);
            }
            else
            {
                repository.SaveSanctionStage2Applicant2(sanctionStage2.Id, sanctionStage2, sanctionStage2.IsApplicant2, this.GetUser().Id, clock.Now);
                audit.Log(sanctionStage2, EventType.SanctionStage2Applicant2Saved, EventCategory.Proposal);
            }
            return Request.CreateResponse();
        }
    }
}
