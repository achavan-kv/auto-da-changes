using Blue.Cosacs.Credit.Repositories;
using Blue.Glaucous.Client.Api;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class ScoreCardConfigurationController : ApiController
    {
        private readonly IScoreCardConfigurationRepository scoreCardConfiguration;

        public ScoreCardConfigurationController(IScoreCardConfigurationRepository scoreCardConfiguration)
        {
            this.scoreCardConfiguration = scoreCardConfiguration;
        }

        [Permission(PermissionsEnum.ScoreConfig)]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(scoreCardConfiguration.GetSoreCardConfig());
        }
    }
}
