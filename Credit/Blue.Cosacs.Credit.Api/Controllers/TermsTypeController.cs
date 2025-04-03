using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class TermsTypeController : ApiController
    {
        private IEventStore audit;
        private ITermsTypeRepository repository;
        private IClock clock;

        public TermsTypeController(IEventStore audit, ITermsTypeRepository repository, IClock clock)
        {
            this.audit = audit;
            this.repository = repository;
            this.clock = clock;
        }

        [Permission(PermissionsEnum.AddTermsTypes)]
        public HttpResponseMessage Get(int id)
        {
            var termsType = repository.Get(id);
            return Request.CreateResponse(termsType);
        }

        [Permission(PermissionsEnum.AddTermsTypes)]
        public HttpResponseMessage Post(Model.TermsTypeDetails termsType)
        {
            termsType.CreatedBy = this.GetUser().Id;
            termsType.CreatedOn = clock.Now;

            repository.Save(termsType, this.GetUser().Id, clock.Now);
            audit.Log(termsType, EventType.AddTermsType, EventCategory.TermsType);

            return Request.CreateResponse();
        }
    }
}
