using Blue.Cosacs.Merchandising;
using Blue.Cosacs.Merchandising.Repositories;
using Blue.Cosacs.Merchandising.Solr;
using Blue.Cosacs.Web.Common;
using Blue.Cosacs.Web.Helpers;
using Blue.Glaucous.Client.Mvc;
using Microsoft.Practices.ObjectBuilder2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blue.Cosacs.Web.Areas.Merchandising.Controllers
{
    /// <summary>
    /// Author : Rahul Dubey
    /// Date   : 15/02/2019
    /// CR     : #Ashley
    /// Details: Controller for Product Stock Range.
    /// </summary>
    public class ProductRangeController : Controller
    {

        private readonly IProductRangeRepository productRangeRepository;
        private readonly IStockSolrIndexer stockSolrIndexer;
        private readonly Settings settings;

        public ProductRangeController(IProductRangeRepository productRangeRepository, Settings settings, IStockSolrIndexer stockSolrIndexer)
        {
            this.productRangeRepository = productRangeRepository;
            this.settings = settings;
            this.stockSolrIndexer = stockSolrIndexer;
        }



        //[Permission(MerchandisingPermissionEnum.Test)]
        public JsonResult Create(ProductStockRange model)
        {
            return Save(model);
        }

        //[Permission(MerchandisingPermissionEnum.Test)]
        public JsonResult Update(ProductStockRange model)
        {
            return Save(model);
        }

        private Common.JSendResult Save(ProductStockRange model)
        {
            if (productRangeRepository.GetAll().Any(t => t.ProductId == model.ProductId))
            {
                //Update
                var result = productRangeRepository.Update(model);
                if (result==null)
                {
                    return new JSendResult(JSendStatus.BadRequest, message: "Mismach in Ashley product data, please save Ashley data again!!!", data: new { supplier = model });
                }
                if (model.ProductId.HasValue)
                {
                    //this.stockSolrIndexer.Index(new int[] { model.ProductID.Value });
                }
                return new JSendResult(JSendStatus.Success, result);
            }

            if (ModelState.IsValid)
            {
                var result = productRangeRepository.Save(model);
                if (result == null)
                {
                    return new JSendResult(JSendStatus.BadRequest, message: "Mismach in Ashley product data, please save Ashley data First then Stock Range!!!", data: new { supplier = model });
                }
                if (model.ProductId.HasValue)
                {
                    //this.stockSolrIndexer.Index(new int[] { model.ProductID.Value });
                }
                return new JSendResult(JSendStatus.Success, result);
            }
            return new JSendResult(JSendStatus.BadRequest, message: ModelState.GetErrors().JoinStrings(","), data: new { supplier = model });
        }
    }
}