using Blue.Cosacs.Merchandising;
using Blue.Cosacs.Merchandising.Repositories;
using Blue.Cosacs.Merchandising.Solr;
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
    /// Details: Controller for Multiple Cost Price.
    /// </summary>
    public class AdditionalCostPriceController : Controller
    {
        private readonly IAdditionalCostRepository costRepository;
        private readonly IStockSolrIndexer stockSolrIndexer;
        public AdditionalCostPriceController(IAdditionalCostRepository costRepository, IStockSolrIndexer stockSolrIndexer)
        {
            this.costRepository = costRepository;
            this.stockSolrIndexer = stockSolrIndexer;
        }

        public JsonResult Create(List<AdditionalCostPrice> model)
        {
            return Save(model);
        }

        private JSendResult Save(List<AdditionalCostPrice> model)
        {
            if (ModelState.IsValid)
            {
                if (costRepository.Delete(model.FirstOrDefault().ProductId))
                {
                    var result = costRepository.Save(model, User.Identity.Name);
                    return new JSendResult(JSendStatus.Success, result);
                }
                
            }
            return new JSendResult(JSendStatus.BadRequest, message: ModelState.GetErrors().JoinStrings(","), data: new { supplier = model });
        }

        public ActionResult GetByVendorDetail(string VendorId)
        {
            return new JSendResult(JSendStatus.Success, costRepository.GetByVendorDetail(VendorId));

        }
    }
}