using Blue.Cosacs.Sales.Models;
using Blue.Cosacs.Sales.Repositories;
using Blue.Glaucous.Client.Api;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Sales.Api.Controllers
{
    public class AssociatedProductsController : ApiController
    {
        private readonly IProductRepository repository;
        private readonly ISalesLookupRepository salesLookup;

        public AssociatedProductsController(IProductRepository repo, ISalesLookupRepository salesLookup)
        {
            repository = repo;
            this.salesLookup = salesLookup;
        }

        public SolrResult<SalesItem> Get(int productId, short branch)
        {
            const int AssociatedProductsViewPermission = 2123;
            var user = this.GetUser();

            if (!user.HasPermission(AssociatedProductsViewPermission))
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "You are not allowed to perform the following action: Associated Products View (Merchandising)"
                };

                throw new HttpResponseException(msg);
            }

            var retLst = new SolrResult<SalesItem>();
            var url = string.Format("/Cosacs/Merchandising/AssociatedProducts/GetAssociatedProducts?productId={0}", productId);
            var userId = user.Id;
            var solrResponse = repository.GetSolrResults(url, userId);

            var locationId = salesLookup.GetBranches(this.GetUser().Id, branch).Id;
            var facia = salesLookup.GetBranchFascia(userId, locationId);

            if (solrResponse != null)
            {
                retLst = repository.ProjectData(solrResponse, locationId, userId, facia, branch);
            }

            return retLst;
        }
    }
}
