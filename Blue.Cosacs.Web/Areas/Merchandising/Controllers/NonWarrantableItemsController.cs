using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using Blue.Cosacs.Merchandising;
using Blue.Cosacs.Merchandising.Repositories;
using Blue.Cosacs.Web.Common;
using Blue.Events;
using Blue.Glaucous.Client.Mvc;
using Blue.Data;
using Blue.Cosacs.Merchandising.Models;
using Blue.Cosacs.Merchandising.Infrastructure;

namespace Blue.Cosacs.Web.Areas.Merchandising.Controllers
{
    public class NonWarrantableItemsController : Controller
    {
        private readonly INonWarrantableItemsRepository nonWarrantableRepository;
        private readonly IClock clock;
        private readonly IEventStore eventStore;

        public NonWarrantableItemsController(IClock clock, IEventStore eventStore, INonWarrantableItemsRepository nonWarrantableRepository)
        {
            this.clock = clock;
            this.eventStore = eventStore;
            this.nonWarrantableRepository = nonWarrantableRepository;
        }

        [Permission(MerchandisingPermissionEnum.NonWarrantableItms)]
        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.HttpPut]
        [Permission(MerchandisingPermissionEnum.NonWarrantableItms)]
        public JSendResult Update(int productId, BasicProductDetails product)
        {
            //CR - Product warranty association need to populate based on warrantable status of product.
            bool isValidProduct = product.Warrantable.Value.Equals(false) ? nonWarrantableRepository.ValidateWarrantableProduct(product.Sku) 
                : true;
            if (isValidProduct)
            {
                nonWarrantableRepository.Update(productId, product.Warrantable.Value);
                return new JSendResult(JSendStatus.Success);
            }
            else { return new JSendResult(JSendStatus.BadRequest,null,"The product have warranty linked. Please delink it before making the product non warrantable."); }
           
        }

        [HttpGet]
        [Permission(MerchandisingPermissionEnum.ViewStock)]
        public JSendResult SelectSearch(string q)
        {
            //CR - Product warranty association need to populate based on warrantable status of product.
            const string SolrType = "MerchandiseStockSummary";

            var filter =
                string.Format(
                    "Type:{0} AND " +
                    "ProductType:RegularStock AND (" +
                    "ProductStatus:\"Active New\" || " +
                    "ProductStatus:\"Active Current\" || " +
                    "ProductStatus:\"Non Active\")",
                    SolrType);

            return new JSendResult(JSendStatus.Success, new Lunr().Search<StockLunrModel>(q, filter));
        }

        [HttpGet]
        [Permission(MerchandisingPermissionEnum.NonWarrantableItms)]
        public ActionResult Search([System.Web.Http.FromUri]int pageSize, [System.Web.Http.FromUri]int pageIndex)
        {
            var filter = new PagedSearch
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = nonWarrantableRepository.Get(filter);
            return new JSendResult(JSendStatus.Success, data);
        }
    }
}