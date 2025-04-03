using Blue.Cosacs.Sales.Models;
using Blue.Networking;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Sales.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IClock clock;
        private readonly IHttpClient httpClient;
        private readonly SalesLookupRepository salesLookupRepository;

        public ProductRepository(IClock clock, IHttpClient httpClient, SalesLookupRepository salesLookupRepo)
        {
            this.clock = clock;
            this.httpClient = httpClient;
            salesLookupRepository = salesLookupRepo;
        }

        public SolrResult<IndexedProducts> GetSolrResults(string url, int userId)
        {
            var jsonClient = new HttpClientJsonAuth(httpClient, clock, userId.ToString());

            var request = RequestJson<byte[]>.Create(url, System.Net.WebRequestMethods.Http.Get);
            var data = jsonClient.Do<byte[], SolrResult<IndexedProducts>>(request).Body;

            return data;
        }

        public SolrResult<SalesItem> ProjectData(SolrResult<IndexedProducts> solrResponse, int locationId, int userId, string fascia, short branch)
        {
            var projectedData = solrResponse.Response.Docs.Select(x => new SalesItem
            {
                ProductItemId = x.ProductId,
                ProductItemNo = x.Sku,
                PriceData = GetBranchSpecificPrice(x.PriceDataObject, locationId, fascia),
                PromoData = GetBranchSpecificPrice(x.PromoDataObject, locationId, fascia),
                Description = x.LongDescription,
                PosDescription = x.PosDescription,
                ProductType = x.ProductType,
                Type = "MerchandiseStockSummary",
                StockBranchProduct = branch,
                StoreType = fascia,
                HierarchyTags = JsonConvertHelper.DeserializeObjectOrDefault<List<HierarchyTags>>(x.HierarchyTags)
            }).ToList();

            var responseToSales = new SolrResult<SalesItem>
            {
                Facets = solrResponse.Facets,
                Response = new SolrResponse<SalesItem>
                {
                    Docs = projectedData,
                    NumFound = solrResponse.Response.NumFound,
                    Start = solrResponse.Response.Start
                },
                ResponseHeader = solrResponse.ResponseHeader
            };

            return responseToSales;
        }

        private PriceData GetBranchSpecificPrice(IEnumerable<PriceData> prices, int locationId, string branchFascia)
        {
            // Branch Specific
            var price = prices.FirstOrDefault(x => x.LocationId == locationId);

            if (price != null)
            {
                return price;
            }

            // Fascia Specific 
            price = prices.FirstOrDefault(x => (x.BranchNumber == null) &&
                    (x.Fascia == "Courts" && (branchFascia == "C" || x.Fascia == branchFascia)) ||
                    (x.Fascia != "Courts" && (branchFascia == "N" || x.Fascia == branchFascia))) ??   // General
                               prices.FirstOrDefault(x => x.BranchNumber == null && x.Fascia == null);

                return price;
        }
    }
}
