using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Repositories.Interfaces;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class ScoringBandMatrixController : ApiController
    {
        private IEventStore audit;
        private IScoringBandMatrixRepository repository;
        private IClock clock;

        public ScoringBandMatrixController(IEventStore audit, IScoringBandMatrixRepository repository, IClock clock)
        {
            this.audit = audit;
            this.repository = repository;
            this.clock = clock;
        }

        [Permission(PermissionsEnum.ScoringBandMatrix)]
        public HttpResponseMessage Get()
        {
            var scoringBandMatrixList = repository.Get();
            return Request.CreateResponse(scoringBandMatrixList);
        }

        [Permission(PermissionsEnum.ScoringBandMatrix)]
        public HttpResponseMessage Post(ScoringBandMatrix scoringBandMatrix)
        {
            scoringBandMatrix.CreatedBy = this.GetUser().Id;
            scoringBandMatrix.CreatedOn = clock.Now;
            repository.SaveScoringBandMatrix(scoringBandMatrix);
            audit.Log(scoringBandMatrix, EventType.AddScoringBandMatrix, EventCategory.ScoringBandMatrix);

            return Request.CreateResponse();
        }

        [Permission(PermissionsEnum.ScoringBandMatrix)]
        public HttpResponseMessage Delete(int id)
        {
            repository.Delete(id);
            return Request.CreateResponse();
        }
    }
}
