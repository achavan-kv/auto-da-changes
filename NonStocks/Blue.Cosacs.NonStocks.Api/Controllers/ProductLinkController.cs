using Blue.Cosacs.NonStocks.Models;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.NonStocks.Api.Controllers
{
    [RoutePrefix("Api/ProductLink")]
    public class ProductLinkController : ApiController
    {
        private IProductLinkRepository productLinkRepository = null;
        private readonly IEventStore audit;

        public ProductLinkController(IProductLinkRepository productLinkRepository, IEventStore audit)
        {
            this.productLinkRepository = productLinkRepository;
            this.audit = audit;
        }

        [Route("LoadAllLinks")]
        [HttpGet]
        [Permission(PermissionsEnum.NonStocksProductLinksView)]
        public HttpResponseMessage LoadAllLinks([FromUri]NonStockLinkSearch search)
        {
            //search = WebApiBindingFailureTemporaryFix(search); // TODO: fixed
            try
            {
                if (search.PageSize <= 0)
                {
                    search.PageSize = 5;
                }

                if (search.PageIndex <= 0)
                {
                    search.PageIndex = 1;
                }

                var links = new List<Models.Link>();
                if (search.HasFilter())
                {
                    var filteredIds =
                        productLinkRepository.GetIdsUsingNameDateFilters(
                        search.Name, search.DateFrom, search.DateTo);

                    links = productLinkRepository.LoadLinks(filteredIds, search);
                }
                else
                {
                    links = productLinkRepository.LoadAllLinks(search);
                }

                var recordCount = productLinkRepository.GetLinkCount(search);
                var pageCount = Math.Ceiling(((decimal)recordCount) / ((decimal)search.PageSize));

                //audit.Log(@event: links,
                //    category: AuditCategories.NonStocksProductLinks, type: AuditEventTypes.SearchProductLinks);

                return Request.CreateResponse(new
                {
                    Result = "Done",
                    RecordCount = recordCount,
                    PageCount = pageCount,
                    PageSize = search.PageSize,
                    PageIndex = search.PageIndex,
                    Links = links
                });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, e);
            }
        }

        //private NonStockLinkSearch WebApiBindingFailureTemporaryFix(NonStockLinkSearch search)
        //{
        //    var tmpDate = new DateTime();
        //    var tmpInt = -1;

        //    if (search == null) // if binding failed, apply fix !!!
        //    {
        //        search = new NonStockLinkSearch();

        //        NameValueCollection nameValueCollection =
        //            HttpUtility.ParseQueryString(Request.RequestUri.Query);

        //        search.Name = nameValueCollection["Name"];
        //        if (DateTime.TryParse(nameValueCollection["DateFrom"], out tmpDate))
        //        {
        //            search.DateFrom = tmpDate;
        //        }
        //        if (DateTime.TryParse(nameValueCollection["DateTo"], out tmpDate))
        //        {
        //            search.DateTo = tmpDate;
        //        }
        //        if (int.TryParse(nameValueCollection["PageSize"], out tmpInt))
        //        {
        //            search.PageSize = tmpInt;
        //        }
        //        if (int.TryParse(nameValueCollection["PageIndex"], out tmpInt))
        //        {
        //            search.PageIndex = tmpInt;
        //        }
        //    }
        //    return search;
        //}

        [Permission(PermissionsEnum.NonStocksProductLinksView)]
        public HttpResponseMessage Get(int id)
        {
            var tmpLink = productLinkRepository.LoadLink(id);

            //audit.Log(@event: tmpLink,
            //    category: AuditCategories.NonStocksProductLinks, type: AuditEventTypes.ViewProductLinkById);

            return Request.CreateResponse(new { Result = "Done", @Link = tmpLink });
        }

        [Permission(PermissionsEnum.NonStocksProductLinksEdit)]
        public HttpResponseMessage Post(Models.Link link)
        {
            var savedLink = productLinkRepository.SaveLink(link);

            audit.Log(@event: savedLink,
                category: AuditCategories.NonStocksProductLinks, type: AuditEventTypes.CreateProductLink);

            return Request.CreateResponse(new { Result = "Done", SavedLink = savedLink });
        }

        [Permission(PermissionsEnum.NonStocksProductLinksEdit)]
        public HttpResponseMessage Delete(int id)
        {
            var linkToDelete = productLinkRepository.GetLink(id);
            var result = productLinkRepository.DeleteLink(id);

            audit.Log(@event: linkToDelete,
                category: AuditCategories.NonStocksProductLinks, type: AuditEventTypes.DeleteProductLink);

            if (result)
            {
                return Request.CreateResponse(new { Result = "Done", Id = id });
            }
            else
            {
                return Request.CreateResponse(new { Result = "Fail" });
            }
        }
    }
}
