namespace Blue.Cosacs.Web.Areas.Merchandising.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web.Mvc;
    using Blue.Cosacs.Merchandising;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Infrastructure;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Cosacs.Merchandising.Solr;
    using Blue.Cosacs.Messages.Merchandising.PurchaseOrder;
    using Blue.Cosacs.Web.Common;
    using Blue.Solr;
    using Blue.Hub.Client;
    using Blue.Glaucous.Client.Mvc;
    using Blue.Cosacs.Web.Helpers;

        public class PurchaseController : Controller
    {
        private readonly IPurchaseRepository purchaseRepository;
        private readonly ILog log;
        private readonly IPurchaseOrderSolrIndexer purchaseOrderSolrIndexer;

        private readonly Settings settings;

        private readonly IPublisher publisher;
        private readonly ISupplierRepository supplierRepository;

        public PurchaseController(IPurchaseRepository purchaseRepository, ISupplierRepository supplierRepository, IPublisher publisher, ILog log, IPurchaseOrderSolrIndexer purchaseOrderSolrIndexer, Settings settings)
        {
            this.supplierRepository = supplierRepository;
            this.purchaseRepository = purchaseRepository;
            this.publisher = publisher;
            this.log = log;
            this.purchaseOrderSolrIndexer = purchaseOrderSolrIndexer;
            this.settings = settings;
            this.supplierRepository = supplierRepository;
        }

        [Permission(MerchandisingPermissionEnum.PurchaseOrderEdit)]
        public ActionResult New()
        {
            return View("Detail", new PurchaseOrderCreateModel());
        }

        [Permission(MerchandisingPermissionEnum.PurchaseOrderView)]
        public ActionResult Detail(int id)
        {
            return View("Detail", purchaseRepository.Get(id));
        }

        [Permission(MerchandisingPermissionEnum.PurchaseOrderView)]
        public ActionResult Print(int id)
        {
            var model = purchaseRepository.GetForPrint(id);
            purchaseRepository.UpdatePrint(id, false);
            return View(model);
        }

        [Permission(MerchandisingPermissionEnum.PurchaseOrderView)]
        public ActionResult PrintWithCost(int id)
        {
            var model = purchaseRepository.GetForPrint(id);
            purchaseRepository.UpdatePrint(id, true);
            return View(model);
        }

        [Permission(MerchandisingPermissionEnum.PurchaseOrderView)]
        public ActionResult PrintAlternate(int id)
        {
            var model = purchaseRepository.GetForPrint(id);
            return View(model);
        }

        [Permission(MerchandisingPermissionEnum.PurchaseOrderView)]
        public JSendResult Get(int id)
        {
            return new JSendResult(JSendStatus.Success, purchaseRepository.Get(id));
        }

        [Permission(MerchandisingPermissionEnum.GoodsReceiptEdit)]
        public JSendResult GetForReceipt(int id)
        {
            return new JSendResult(JSendStatus.Success, purchaseRepository.GetForReceipt(id));
        }
       
        [Permission(MerchandisingPermissionEnum.PurchaseOrderEdit)]
        public JSendResult Create(PurchaseOrderCreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = this.GetUser();
                    model.CreatedById = user.Id;
                    model.CreatedBy = user.FullName;
                    var purchaseOrder = purchaseRepository.Create(model);

                    // Code Added by Abhijeet for Ashley CR 
                    // If Create PO any one in Ashley Product Provide Message 
                    if (purchaseOrder.ResponseMsg == "DONE")
                    {
                        return new JSendResult(JSendStatus.Error, message: "Select Ashely Products in Current List");
                    }
                    // End Code 

                    try
                    {
                        ForceIndex(new[] { purchaseOrder.Id });
                    }
                    catch (Exception e)
                    {
                        log.Exception(new IndexingException(EventCategories.Merchandising, e));
                    }
                    return new JSendResult(JSendStatus.Success, purchaseOrder);                 
                }
                catch (Exception e)
                {
                    log.Exception(new IndexingException(EventCategories.Merchandising, e));
                    return new JSendResult(JSendStatus.Error, message: "There was a problem saving the purchase order.");
                }
            }

            return new JSendResult(JSendStatus.BadRequest, ModelState.GetErrors());
        }
        
        [Permission(MerchandisingPermissionEnum.PurchaseOrderView)]
        public ActionResult Email(int id)
        {
            var purchaseOrder = purchaseRepository.Get(id);
            // Load vendor and try to send email
            var vendor = supplierRepository.Get(purchaseOrder.VendorId);
            if (vendor != null)
            {
                if (vendor.OrderEmail != null)
                {
                    publisher.Publish<Context, VendorPurchaseOrder>(
                        "Merchandising.VendorMail",
                        new VendorPurchaseOrder()
                            {
                                VendorId = vendor.Id,
                                PurchaseOrderId = purchaseOrder.Id,
                                VendorEmail = vendor.OrderEmail,
                                VendorName = vendor.Name
                            });
                           
                    return new JSendResult(JSendStatus.Success, purchaseOrder);
                }
                return new JSendResult(JSendStatus.BadRequest, message: "No Vendor Email Found");
            }
            return new JSendResult(JSendStatus.BadRequest, ModelState.GetErrors());
        }

        [Permission(MerchandisingPermissionEnum.PurchaseOrderEdit)]
        public JSendResult Cancel(int id,int cancelReasonId)
        {
            purchaseRepository.Cancel(id, cancelReasonId);

            try
            {
                ForceIndex(new[] { id });
            }
            catch (Exception e)
            {
                log.Exception(new IndexingException(EventCategories.Merchandising, e));
            }

            return new JSendResult(JSendStatus.Success);
        }
        [Permission(MerchandisingPermissionEnum.AshleyPOFirstApproval)]
        public JSendResult POFirstLevelApprove(int id)
        {
            purchaseRepository.POFirstLevelApprove(id);

            try
            {
                ForceIndex(new[] { id });
            }
            catch (Exception e)
            {
                log.Exception(new IndexingException(EventCategories.Merchandising, e));
            }

            return new JSendResult(JSendStatus.Success);
        }

        [Permission(MerchandisingPermissionEnum.AshleyPOSecondApproval)]
        public JSendResult POSecondLevelApprove(int id)
        {
            purchaseRepository.POSecondLevelApprove(id);

            try
            {
                ForceIndex(new[] { id });
            }
            catch (Exception e)
            {
                log.Exception(new IndexingException(EventCategories.Merchandising, e));
            }

            return new JSendResult(JSendStatus.Success);
        }
        [Permission(MerchandisingPermissionEnum.AshleyPOSecondApproval)]
        public JSendResult POBothLevelApprove(int id)
        {
            purchaseRepository.POBothLevelApprove(id);

            try
            {
                ForceIndex(new[] { id });
            }
            catch (Exception e)
            {
                log.Exception(new IndexingException(EventCategories.Merchandising, e));
            }

            return new JSendResult(JSendStatus.Success);
        }

        [Permission(MerchandisingPermissionEnum.AshleyPOFirstApproval)]
        public JSendResult POFirstLevelReject(int id)
        {
            purchaseRepository.POFirstLevelReject(id);
            
            try
            {
                ForceIndex(new[] { id });
            }
            catch (Exception e)
            {
                log.Exception(new IndexingException(EventCategories.Merchandising, e));
            }

            return new JSendResult(JSendStatus.Success);
        }
        [Permission(MerchandisingPermissionEnum.AshleyPOSecondApproval)]
        public JSendResult POSecondLevelReject(int id)
        {
            purchaseRepository.POSecondLevelReject(id);

            try
            {
                ForceIndex(new[] { id });
            }
            catch (Exception e)
            {
                log.Exception(new IndexingException(EventCategories.Merchandising, e));
            }

            return new JSendResult(JSendStatus.Success);
        }

        [Permission(MerchandisingPermissionEnum.AshleyPOSecondApproval)]
        public JSendResult POBothLevelReject(int id)
        {
            purchaseRepository.POBothLevelReject(id);

            try
            {
                ForceIndex(new[] { id });
            }
            catch (Exception e)
            {
                log.Exception(new IndexingException(EventCategories.Merchandising, e));
            }

            return new JSendResult(JSendStatus.Success);
        }

        public ActionResult GetList()
        {
            return new JSendResult(JSendStatus.Success, purchaseRepository.GetList());

        }
        public ActionResult GetAshleyEnabled()
        {
            return new JSendResult(JSendStatus.Success, purchaseRepository.GetAshleyEnable());

        }

        [Permission(MerchandisingPermissionEnum.PurchaseOrderEdit)]
        public JSendResult CancelProduct(int id, int purchaseOrderProductId)
        {
            var user = HttpContext.GetUser();
            return new JSendResult(JSendStatus.Success, purchaseRepository.CancelProduct(id, user, purchaseOrderProductId));
        }

        [Permission(MerchandisingPermissionEnum.PurchaseOrderEdit)]
        public JSendResult Update(EditPurchaseOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                purchaseRepository.Update(model);
                try
                {
                    ForceIndex(new[] { model.Id });
                }
                catch (Exception e)
                {
                    log.Exception(new IndexingException(EventCategories.Merchandising, e));
                }
                return new JSendResult(JSendStatus.Success, purchaseRepository.Get(model.Id));
            }
            return new JSendResult(JSendStatus.BadRequest, ModelState.GetErrors());
        }

        [HttpGet]
        [CronJob]
        [Permission(MerchandisingPermissionEnum.RunScheduledJobs)]
        [LongRunningQueries]
        public HttpStatusCodeResult AutoExpire()
        {
            purchaseRepository.AutoExpire();
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }
        #region Ashley CR
        /// <summary>
        /// Author : Rahul Dubey
        /// Date   : 18/02/2019
        /// CR     : #Ashley
        /// Details: Auto Create PO JOB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CronJob]
        [Permission(MerchandisingPermissionEnum.RunScheduledJobs)]
        [LongRunningQueries]
        public HttpStatusCodeResult AutoCreate()
        {
            purchaseRepository.AutoCreate();
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }
        #endregion

        [HttpGet]
        [Permission(MerchandisingPermissionEnum.PurchaseOrderView)]
        public ActionResult Index(string q = "")
        {
            /// <summary>
            /// Author : Rahul Dubey
            /// Date   : 15/02/2019
            /// CR     : #Ashley
            /// Details: Changes made to implement Logic to Force Re-Index Auto PO.
            /// </summary>
            var POsRequiredIndexing = purchaseRepository.GetForceIndexAutoPurchaseOrder();
            if (POsRequiredIndexing != null)
            {
                List<int> ids = new List<int>();
                foreach(var Po in POsRequiredIndexing)
                {
                    ids.Add(Po.PurchaseId);
                }
                this.purchaseOrderSolrIndexer.Index(ids.ToArray());
                purchaseRepository.UpdateForceIndexAutoPurchaseOrder(ids.ToArray());
            }
            return View(model: SearchSolr(q));
        }

        [LongRunningQueries]
        [Permission(Cosacs.Warehouse.Common.WarehousePermissionEnum.Reindex)]
        public void ForceIndex(int[] ids = null)
        {
            this.purchaseOrderSolrIndexer.Index(ids);
        }

        [Permission(MerchandisingPermissionEnum.PurchaseOrderView)]
        public JSendResult NotReceived(int vendorId)
        {
            return new JSendResult(JSendStatus.Success, purchaseRepository.GetNotReceived(vendorId));
        }

        [HttpGet]
        [Permission(MerchandisingPermissionEnum.PurchaseOrderView)]
        public void SearchInstant(string q, int start = 0)
        {
            var result = SearchSolr(q, start);
            Response.Write(result);
        }

        [Permission(MerchandisingPermissionEnum.PurchaseOrderView)]
        public JSendResult Labels(List<PurchaseOrderProductLabelModel> products)
        {
            var model = purchaseRepository.GetPrintLabels(products);
            var labels = RenderRazorViewToString("StockLabelPrint", model);

            return new JSendResult(JSendStatus.Success, new { data = labels, printer = settings.ZebraPrinterName });
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                var result = sw.GetStringBuilder().ToString();
                return result;
            }
        }

        private string SearchSolr(string q, int start = 0, int rows = 25, string type = "MerchandisePurchaseOrder")
        {
            var result = new Query().SelectJsonWithJsonQuery(
                q,
                "Type:" + type,
                facetFields: new[] { "Vendor", "ReceivingLocation", "Status", "OriginSystem" },
                showEmpty: false,
                // the order that the fields appear on the search page are determined by the order of this array
                start: start,
                rows: rows);

            return result;
        }
    }
}
