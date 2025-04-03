using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers 
{
    public class ScoringController : ApiController
    {
        // Temp implementation
        public HttpResponseMessage Get(int id)
        {
            using (var scope = Context.Read())
            {
                var prop = scope.Context.Proposal.Find(id);
                return Request.CreateResponse(new { hasApplicant2 = prop.ApplicationType != "Sole" });
            }
        }

        public HttpResponseMessage Put(int id)
        {
            using (var scope = Context.Write())
            {
                var prop = scope.Context.Proposal.Find(id);
                prop.ApplicationStage = prop.ApplicationStage | (int)ProposalStagesEnum.Scored;
                scope.Context.SaveChanges();
                scope.Complete();
                return Request.CreateResponse();
            }
        }
    }
}
