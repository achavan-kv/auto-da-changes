using Blue.Cosacs.Credit.Repositories;
using Blue.Glaucous.Client.Api;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class ScoreCardController : ApiController
    {
        private readonly IScoreCardConfigurationRepository scoreCardConfiguration;
        private readonly IClock clock;

        public ScoreCardController(IScoreCardConfigurationRepository scoreCardConfiguration, IClock clock)
        {
            this.scoreCardConfiguration = scoreCardConfiguration;
            this.clock = clock;
        }

        [Permission(PermissionsEnum.ScoreConfig)]
        public HttpResponseMessage Post(Model.ScoreCard card)
        {
            scoreCardConfiguration.Save(card, this.GetUser().Id, clock);
            return Request.CreateResponse();
        }

        [Permission(PermissionsEnum.ScoreConfig)]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(scoreCardConfiguration.GetRules());
        }
    }
}
