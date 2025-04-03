using Blue.Cosacs.Sales.Api.Extensions;
using Blue.Cosacs.Sales.Models;
using Blue.Cosacs.Sales.Repositories;
using Blue.Glaucous.Client.Api;
using Blue.Networking;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

//using Microsoft.AspNet.Identity.Owin;

namespace Blue.Cosacs.Sales.Api.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private IHttpClientJson httpClientJson;
        private readonly IProductRepository repository;
        private readonly ISalesLookupRepository salesLookup;

        public ProductsController(IHttpClientJson httpClientJson, IProductRepository repo, ISalesLookupRepository salesLookup)
        {
            this.httpClientJson = httpClientJson;
            this.salesLookup = salesLookup;
            repository = repo;
        }

        [Route("Search")]
        [HttpGet]
        public SolrResult<SalesItem> Search(string q, int rows = 25, bool isStrict = true, string upc = "")
        {
            if (!q.EndsWith("*") && !isStrict)
            {
                q = q + "*";
            }

            var branch = this.GetUser().Branch;
            var locationId = salesLookup.GetBranches(this.GetUser().Id, branch).Id; //this.GetUser().LocationId;
            var ret = GetProducts(locationId, q, 0, rows, upc);

            return ret;
        }

        private string GetProductsJson(int locationId, string query, int start = 0, int rows = 25)
        {
            if (query == null)
            {
                query = string.Empty;
            }

            var solrQuery = new Blue.Solr.Query();
            var request = solrQuery.GetSearchRequest(query);
            var userBranch = new ArrayList { locationId };
            if (request.FacetFields == null)
            {
                request.FacetFields = new Dictionary<string, Blue.Solr.Request.Filter>();
            }
            // TODO: Implement
            // branch.Add(this.GetUser().Branch);
            request.FacetFields["StockBranchProduct"] = new Solr.Request.Filter { Values = userBranch };
            var sw = new StringWriter();

            new Newtonsoft.Json.JsonSerializer { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore }.Serialize(
                sw, request);

            var solrResult = solrQuery.SelectJsonWithJsonQuery(
                sw.ToString(),
                "Type:Product",
                facetFields: new string[] { "Level_1", "Level_2", "Level_3" },
                sort: "ProductItemId asc",
                start: start,
                rows: rows,
                showEmpty: false);
            var noneStock = JsonConvert.DeserializeObject<Solr.Result>(solrResult);
            return solrResult;
        }

        [HttpGet]
        public HttpResponseMessage GetAssociatedProducts(int branch, string itemNo)
        {
            var uriString = string.Format("/Courts.NET.WS/Sales/GetAssociatedProducts?branch={0}&itemNo={1}", branch.ToString(CultureInfo.InvariantCulture), itemNo.ToString(CultureInfo.InvariantCulture));

            //this list contains associated items' itemNo... this list of itemNos will be used to fetch the corresponsing complete detail regarding htat itemNO
            var associatedItems = httpClientJson.Do<byte[], List<string>>(RequestJson<byte[]>.Create(uriString, WebRequestMethods.Http.Get)).Body;

            if (associatedItems.Count > 0)
            {
                associatedItems = associatedItems
                   .Take(associatedItems.Count - 1)
                   .Select(p => string.Format("ProductItemNo:{0} OR ", p))
                   .Union(associatedItems.Skip(associatedItems.Count - 1)
                        .Select(p => string.Format("ProductItemNo:{0}", p))
                        .ToList())
                   .ToList();

                return this.GetJsonResponseMessage(GetItemDetails(branch, string.Join(" ", associatedItems.ToArray())));
            }
            else
            {
                return this.GetJsonResponseMessage(string.Empty);
            }
        }

        //Used to fetch the complete item details for associated items
        private string GetItemDetails(int branch, string itemNo)
        {
            var solrQuery = new Blue.Solr.Query();

            var solrResult = solrQuery.SelectJsonWithJsonQuery(
              string.Empty,
              string.Format("Type:Product AND StockBranchProduct:{0} AND ({1})", branch, itemNo),
              sort: "ProductItemNo asc",
              start: 0,
              rows: 50,
              showEmpty: false);

            return solrResult;
        }

        [HttpGet]
        public CustomSolrResult TempGetData(string q = "", int rows = 25)
        {
            q = q.Replace("merchandisingLevel_", "MerchandisingLevel_")
                    .Replace("division", "Division")
                    .Replace("department", "Department")
                    .Replace("class", "Class");

            return MergedProductsWithQuey(q, rows);
        }

        private CustomSolrResult MergedProductsWithQuey(string q = "", int rows = 25)
        {
            var solrQuery = new Blue.Solr.Query();
            var request = solrQuery.GetSearchRequest(q);
            var isEmptySearch = string.IsNullOrEmpty(request.Search) && request.FacetFields.Count == 0;

            if (isEmptySearch)
            {
                // Don't return results only facets
                rows = 0;
            }

            var merData = new Solr.Query().SelectJsonWithJsonQuery(
                q,
                "Type:NonStock OR Type:MerchandiseStockSummary",
                facetFields: new string[] { "MerchandisingLevel_1", "MerchandisingLevel_2", "MerchandisingLevel_3" },
                start: 0,
                rows: rows,
                showEmpty: false);

            var merResult = JsonConvert.DeserializeObject<CustomSolrResult>(merData);

            return merResult;
        }

        public string Get(string query, int start = 0, int rows = 25)
        {
            var branch = this.GetUser().Branch;

            return "";
        }

        public SolrResult<SalesItem> Get(int branch, string q = "", int start = 0, int rows = 25)
        {
            var locationId = salesLookup.GetBranches(this.GetUser().Id, (short)branch).Id;

            var ret = GetProducts(locationId, q, start, rows);
            return ret;
        }

        private SolrResult<SalesItem> GetProducts(int locationId, string q, int start = 0, int rows = 25, string upc = "")
        {
            const int ViewStockPermission = 2112;
            var user = this.GetUser();

            if (!user.HasPermission(ViewStockPermission))
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "You are not allowed to perform the following action: View Stock (Merchandising)"
                };

                throw new HttpResponseException(msg);
            }

            q = q.Replace("merchandisingLevel_", "MerchandisingLevel_")
                .Replace("division", "Division")
                .Replace("department", "Department")
                .Replace("class", "Class");

            var query = new StringBuilder();

            query.Append("!ProductStatus:Deleted AND !ProductStatus:\"Non Active\"");// "ProductType:RegularStock AND !ProductStatus:Deleted AND !ProductStatus:\"Non Active\"";

            if (!string.IsNullOrEmpty(upc))
            {
                query.AppendFormat(" AND (CorporateUPC=\"{0}\" OR VendorUPC=\"{0}\")", upc);
            }
            var url = string.Format("/Cosacs/Merchandising/Products/GetProducts?q={0}&start={1}&rows={2}&query={3}", q, start, rows, query.ToString());
            var userId = user.Id;

            var solrResponse = repository.GetSolrResults(url, userId);
            var branch = this.GetUser().Branch;
            locationId = salesLookup.GetBranches(this.GetUser().Id, branch).Id; 
            var facia = salesLookup.GetBranchFascia(userId, locationId);
            // this.GetUser().LocationId is wrong right now that's why we used this
            

            return repository.ProjectData(solrResponse, locationId, userId, facia, this.GetUser().Branch);
        }

    }
}