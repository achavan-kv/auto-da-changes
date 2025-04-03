using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Blue.Cosacs.Api.Customer.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.Customer.Api.Controllers
{
    [RoutePrefix("api/Reindex")]
    public class ReindexController : ApiController
    {
        private readonly Settings cosacsSettings;

        public ReindexController(Settings cosacsSettings)
        {
            this.cosacsSettings = cosacsSettings;
        }

        public HttpResponseMessage Get([FromUri]CustomerDetailRequest customerDetails)
        {
            var solrQuery = new Blue.Solr.Query();
            var solrResult = solrQuery.SelectJsonWithJsonQuery(
                            string.Empty,
                            ConvertToFilterString(customerDetails),
                            start: customerDetails.Start,
                            rows: customerDetails.Rows,
                            sort: "Priority ASC, LatestDate DESC",
                            showEmpty: false);


            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(solrResult, Encoding.UTF8, "application/json");
            return response;
        }

        private string ConvertToFilterString(CustomerDetailRequest input)
        {
            StringBuilder filterString = new StringBuilder("Type:Customer");
            if (!string.IsNullOrEmpty(input.CustomerId))
            {
                filterString.AppendFormat(" AND CustomerId:\"*{0}*\"", input.CustomerId);
            }
            if (!string.IsNullOrEmpty(input.Title))
            {
                filterString.AppendFormat(" AND Title:\"*{0}*\"", input.Title);
            }
            if (!string.IsNullOrEmpty(input.FirstName))
            {
                filterString.AppendFormat(" AND FirstName:\"*{0}*\"", input.FirstName);
            }
            if (!string.IsNullOrEmpty(input.LastName))
            {
                filterString.AppendFormat(" AND LastName:\"*{0}*\"", input.LastName);
            }
            if (!string.IsNullOrEmpty(input.HomePhoneNumber))
            {
                filterString.AppendFormat(" AND HomePhoneNumber:*{0}*", input.HomePhoneNumber);
            }
            if (!string.IsNullOrEmpty(input.MobileNumber))
            {
                filterString.AppendFormat(" AND MobileNumber:*{0}*", input.MobileNumber);
            }
            if (!string.IsNullOrEmpty(input.Email))
            {
                filterString.AppendFormat(" AND Email:*{0}*", input.Email);
            }
            if (!string.IsNullOrEmpty(input.Alias))
            {
                filterString.AppendFormat(" AND Alias:*{0}*", input.Alias);
            }
            if (!string.IsNullOrEmpty(input.AddressLine1))
            {
                filterString.AppendFormat(" AND AddressLine1:*{0}*", input.AddressLine1);
            }
            if (!string.IsNullOrEmpty(input.AddressLine2))
            {
                filterString.AppendFormat(" AND AddressLine2:*{0}*", input.AddressLine2);
            }
            if (!string.IsNullOrEmpty(input.TownOrCity))
            {
                filterString.AppendFormat(" AND TownOrCity:*{0}*", input.TownOrCity);
            }

            if (!string.IsNullOrEmpty(input.PostCode))
            {
                filterString.AppendFormat(" AND PostCode:*{0}*", input.PostCode);
            }

            if (input.DOB.HasValue)
            {
                filterString.AppendFormat(" AND DOB:[{0} TO {1}]", Convert.ToDateTime(input.DOB).ToSolrDate().Replace('Z', ' ').Trim(),
                    Convert.ToDateTime(input.DOB).Date.AddDays(1).AddTicks(-1).ToSolrDate().Replace('Z', ' ').Trim());
            }

            if (input.CustomerBranch.HasValue)
            {
                filterString.AppendFormat(" AND CustomerBranch:{0}", input.CustomerBranch);
            }

            if (input.AvailableSpend.HasValue)
            {
                filterString.AppendFormat(" AND AvailableSpend:{0}", input.AvailableSpend);
            }

            if (input.SalesPersonId.HasValue)
            {
                filterString.AppendFormat(" AND SalesPersonId:{0}", input.SalesPersonId);
            }

            if (input.DateLastBought.HasValue)
            {
                filterString.AppendFormat(" AND LastPurchaseDate:[{0} TO {1}]",
                    Convert.ToDateTime(input.DateLastBought).ToSolrDate().Replace('Z', ' ').Trim(),
                    Convert.ToDateTime(input.DateLastBought).Date.AddDays(1).AddTicks(-1).ToSolrDate().Replace('Z', ' ').Trim());
            }

            if (!string.IsNullOrEmpty(input.CustomerSource))
            {
                filterString.AppendFormat(" AND CustomerSource:*{0}*", input.CustomerSource);
            }

            return filterString.ToString();
        }

        [Route("SearchByCsr")]
        [HttpGet]
        public HttpResponseMessage SearchByCsr(int branch, string q = "", int start = 0, int rows = 25)
        {
            return this.Search(q, start, rows, this.GetUser().Id, null);
        }

        private HttpResponseMessage Search(string q = "", int start = 0, int rows = 25, int? userId = null, int? branch = null)
        {
            var facets = new List<string> { "HasPendingCalls", "ReceiveEmails", "DateLastBoughtRange", "IsActiveCashCustomer", "IsActiveCreditCustomer", "HasBirthday" };
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            var filter = new Solr.Request.Filter
            {
                Values = new System.Collections.ArrayList(),
                Negate = false
            };
            var solrQuery = new Blue.Solr.Query();
            var req = solrQuery.GetSearchRequest(q);

            var AvailableSpendRange = new Solr.Range
            {
                End = "9999999",
                Field = "AvailableSpend",
                Gap = cosacsSettings.CustomerSearchAvailableGroupSize.ToString(),
                Start = cosacsSettings.CustomerSearchAvailableStart.ToString()
            };

            if (branch.HasValue)
            {
                var BranchNoFacetKey = "CustomerBranchNo";

                facets.Add("SalesPerson");
                filter.Values.Add(branch);

                //Add default filter for branch number
                var hasBranchFilter = req.FacetFields != null && req.FacetFields
                    .Any(p => string.Compare(p.Key, BranchNoFacetKey, true) == 0);

                if (!hasBranchFilter)
                {
                    if (req == null)
                    {
                        req = new Solr.Request();
                    }

                    if (req.FacetFields == null)
                    {
                        req.FacetFields = new System.Collections.Generic.Dictionary<string, Solr.Request.Filter>();
                    }

                    req.FacetFields.Add(BranchNoFacetKey, filter);
                }
                else
                {
                    req.FacetFields[BranchNoFacetKey].Values.Add(filter);
                }
            }

            if (userId.HasValue)
            {
                facets.Add("CustomerBranchName");

                filter.Values.Add(userId);
                req.FacetFields.Add("SalesPersonId", filter);
            }

            q = req.ToJson();

            var solrResult = solrQuery
                .SelectJsonWithJsonQuery(
                    q,
                    "Type:Customer",
                    facetFields: facets.ToArray(),
                    showEmpty: false,
                    start: start,
                    rows: rows,
                    range: new Solr.Range[] { AvailableSpendRange }
                );

            response.Content = new StringContent(solrResult, Encoding.UTF8, "application/json");

            return response;
        }

        [Route("Search")]
        [HttpGet]
        public HttpResponseMessage Search(int branch, string q = "", int start = 0, int rows = 25)
        {
            return this.Search(q, start, rows, null, branch);
        }

        [Route("ForceIndex")]
        [HttpGet]
        [Permission(CustomerPermissionEnum.ReindexCustomer)]
        [LongRunningQueries]
        [CronJob]
        public string ForceIndex()
        {
            SolrIndex.CustomerDetails();

            return "ok " + DateTime.Now.ToString();
        }

        [HttpPost]
        [Permission(CustomerPermissionEnum.ReindexCustomer)]
        public string Post(List<string> customerIds)
        {
            Parallel.ForEach(customerIds, currentId =>
                {
                    SolrIndex.CustomerDetails(currentId);
                });

            return "ok " + DateTime.Now.ToString();
        }

        [Route("IndexSalesCustomer")]
        [HttpPost]
        [Permission(CustomerPermissionEnum.CreateOrders)]
        public HttpResponseMessage IndexSalesCustomer([FromUri] string firstName, [FromUri]  string lastName)
        {
            SolrIndex.IndexSalesCustomer(firstName, lastName);

            return Request.CreateResponse(HttpStatusCode.OK, "ok " + DateTime.Now);
        }
    }
}
