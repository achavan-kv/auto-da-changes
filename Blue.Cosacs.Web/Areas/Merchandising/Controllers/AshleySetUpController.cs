using Blue.Cosacs.Merchandising;
using Blue.Cosacs.Merchandising.Repositories;
using Blue.Cosacs.Web.Common;
using Blue.Cosacs.Web.Helpers;
using Microsoft.Practices.ObjectBuilder2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blue.Cosacs.Web.Areas.Merchandising.Controllers
{
    /// <summary>
    /// Author : Rahul Dubey
    /// Date   : 15/02/2019
    /// CR     : #Ashley
    /// Details: Controller for Ashley Product Attribute
    /// </summary>
    public class AshleySetupController : Controller
    {
        private readonly IAshleySetUpRepository ashleySetUpRepository;
        private readonly IProductRepository productRepository;
        public AshleySetupController(IAshleySetUpRepository ashleySetUpRepository, IProductRepository productRepository)
        {
            this.ashleySetUpRepository = ashleySetUpRepository;
            this.productRepository = productRepository;
        }
        public JsonResult Create(ProductAttribute model)
        {
            return Save(model);
        }
        
        public JsonResult Update(ProductAttribute model)
        {
            return Save(model);
        }
        private JSendResult Save(ProductAttribute model)
        {
            if (ashleySetUpRepository.GetAll().Any(t => t.ProductId == model.ProductId))
            {
                //Update
                var result = ashleySetUpRepository.Update(model);
                if (result == null)
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
                var result = ashleySetUpRepository.Save(model);
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

        public string GetAshleyEnable()
        {
            return productRepository.GetAshleyEnable();
        }
    }
}