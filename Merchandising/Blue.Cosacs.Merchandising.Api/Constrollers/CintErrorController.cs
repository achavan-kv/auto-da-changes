using Blue.Cosacs.Merchandising;
using Blue.Cosacs.Merchandising.Models;
using Blue.Cosacs.Merchandising.Repositories;
using Blue.Glaucous.Client.Api;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Web.Areas.Merchandising.Controllers
{
    public class CintErrorController : ApiController
    {
        private readonly ICintErrorRepository repository;

        public CintErrorController(ICintErrorRepository repository)
        {
            this.repository = repository;
        }

        [Permission(MerchandisingPermissionEnum.CintErrorView)]
        public HttpResponseMessage Search(CintErrorQueryModel query)
        {
            if (query.Bulk)
            {
                return Request.CreateResponse(repository.SearchBulk(query));
            }
            else
            {
                return Request.CreateResponse(repository.Search(query));
            }
        }
    }
}