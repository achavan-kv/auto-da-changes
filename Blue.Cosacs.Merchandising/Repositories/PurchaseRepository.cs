namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Admin;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Infrastructure;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Solr;
    using Blue.Events;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Context = Blue.Cosacs.Merchandising.Context;
    using Settings = Blue.Cosacs.Merchandising.Settings;

    public interface IPurchaseRepository
    {
        PurchaseOrderViewModel Create(PurchaseOrderCreateModel model);

        PurchaseOrderViewModel Update(EditPurchaseOrderViewModel model);

        void ForceIndex(int[] orderIds);

        void Cancel(int id, int CancelreasonId);

        /// <summary>
        /// Code Added by Abhijeet for Purchase Order Approval 
        /// </summary>
        /// <param name="id"></param>

        void POFirstLevelApprove(int id);
        void POSecondLevelApprove(int id);
        void POBothLevelApprove(int id);
        void POFirstLevelReject(int id);
        void POSecondLevelReject(int id);
        void POBothLevelReject(int id);
        Dictionary<int, string> GetList();
        Boolean GetAshleyEnable();

        PurchaseOrderViewModel CancelProduct(int id, UserSession user, int purchaseOrderProductId);

        void RefreshStatus(int purchaseOrderId);

        PurchaseOrderViewModel Get(int id);

        PurchaseOrderPrintModel GetForPrint(int id);

        List<LabelModel> GetPrintLabels(List<PurchaseOrderProductLabelModel> purchaseOrder);

        PurchaseOrderViewModel GetForReceipt(int id);

        List<PurchaseOrderViewModel> GetNotReceived(int? vendorId = null);

        void UpdatePrint(int id, bool withCost);

        bool CorporatePoNumberIsUnique(string originSystem, string corporatePoNumber);

        void AutoExpire();

        List<PurchaseOrderLunrModel> GetNotReceived(int vendorId);
        List<ForceIndexAutoPurchaseOrder> GetForceIndexAutoPurchaseOrder();
        void UpdateForceIndexAutoPurchaseOrder(int[] ids);
        void AutoCreate();

    }

    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly IEventStore audit;
        private const string SolrType = "MerchandisePurchaseOrder";
        private readonly IProductRepository productRepository;
        private readonly ISupplierRepository supplierRepository;
        private readonly ILocationRepository locationRepository;
        private readonly IPurchaseOrderSolrIndexer purchaseOrderSolrIndexer;
        private readonly IStockSolrIndexer stockSolrIndexer;
        private readonly IUserRepository userRepository;

        public PurchaseRepository(
            IEventStore audit,
            IProductRepository productRepository,
            ISupplierRepository supplierRepository,
            ILocationRepository locationRepository,
            IPurchaseOrderSolrIndexer purchaseOrderSolrIndexer,
             IStockSolrIndexer stockSolrIndexer,
             IUserRepository userRepository)
        {
            this.audit = audit;
            this.productRepository = productRepository;
            this.supplierRepository = supplierRepository;
            this.locationRepository = locationRepository;
            this.purchaseOrderSolrIndexer = purchaseOrderSolrIndexer;
            this.stockSolrIndexer = stockSolrIndexer;
            this.userRepository = userRepository;
        }

        public List<PurchaseOrderLunrModel> GetNotReceived(int vendorId)
        {
            var filter = string.Format(
                  "Type:{0} AND "
                + "VendorId:{1} AND "
                + "!Status:Cancelled AND "
                + "!Status:Completed",
                SolrType,
                vendorId);

            return new Lunr().Search<PurchaseOrderLunrModel>(string.Empty, filter);
        }

        public PurchaseOrderViewModel Create(PurchaseOrderCreateModel model)
        {
            if (model.Products.Any(s => s.RequestedDeliveryDate < DateTime.Now.Date))
            {
                throw new Exception("Invalid requested date");
            }

            using (var scope = Context.Write())
            {
                // Add Condition for if Create Manual PO & Ashely Product order status is Unapproved
                    string FlgAshleyProdcut = string.Empty;                
                
                    var location = scope.Context.Location.Find(model.ReceivingLocationId);
                    var vendor = scope.Context.Supplier.Find(model.VendorId);

                    var purchaseOrder = Mapper.Map<PurchaseOrder>(model);

                if (purchaseOrder.Status == PurchaseOrderStatuses.Unapproved)
                {
                    foreach (var productModel in model.Products)
                    {
                        var ProductAttribute = scope.Context.ProductAttributes.ToList();

                        if (ProductAttribute.Any(p => p.ProductId == productModel.ProductId))
                        {
                            FlgAshleyProdcut = "Failed";
                            return new PurchaseOrderViewModel() { ResponseMsg = "DONE" };
                        }

                    }
                }
                
                    purchaseOrder.ReceivingLocation = location.Name;
                    purchaseOrder.Vendor = vendor.Name;
                    purchaseOrder.Currency = model.Currency ?? vendor.Currency;
                    purchaseOrder.Status = PurchaseOrderStatuses.New;
                    purchaseOrder.CreatedDate = DateTime.UtcNow;

                    var productIds = model.Products.Select(mp => mp.ProductId).Distinct().ToList();
                    var products = scope.Context.Product.Where(p => productIds.Contains(p.Id)).ToList();


                    var allCostPrices = scope.Context.CostPrice.Where(cp => productIds.Contains(cp.ProductId)).GroupBy(p => p.ProductId).ToList();
                    var costPrices = allCostPrices.Select(g => g.OrderByDescending(c => c.LastLandedCostUpdated).First()).ToList();

                    scope.Context.PurchaseOrder.Add(purchaseOrder);
                    scope.Context.SaveChanges();

                    audit.LogAsync(purchaseOrder, PurchaseEvents.Create, EventCategories.Merchandising);

                    var savedProducts = new List<PurchaseOrderProduct>();

                    foreach (var productModel in model.Products)
                    {
                        var product = products.Single(p => p.Id == productModel.ProductId);

                        var purchaseOrderProduct = Mapper.Map<PurchaseOrderProduct>(productModel);
                        purchaseOrderProduct.PurchaseOrderId = purchaseOrder.Id;
                        purchaseOrderProduct.Description = product.LongDescription;
                        purchaseOrderProduct.Sku = product.SKU;
                        purchaseOrderProduct.EstimatedDeliveryDate = purchaseOrderProduct.RequestedDeliveryDate; // initially these are the same and then can be updated

                        if (productModel.SupplierUnitCost <= 0)
                        {
                            var costPrice =
                                costPrices.Where(cp => cp.ProductId == productModel.ProductId)
                                    .OrderByDescending(p => p.LastLandedCostUpdated)
                                    .First();
                            purchaseOrderProduct.UnitCost = costPrice.SupplierCost;
                        }
                        else
                        {
                            purchaseOrderProduct.UnitCost = productModel.SupplierUnitCost;
                        }

                        scope.Context.PurchaseOrderProduct.Add(purchaseOrderProduct);
                        scope.Context.SaveChanges();

                        savedProducts.Add(purchaseOrderProduct);
                        audit.LogAsync(purchaseOrderProduct, PurchaseEvents.CreatePurchaseOrderProduct, EventCategories.Merchandising);
                        var stockLevel = scope.Context.ProductStockLevel.SingleOrDefault(s => s.ProductId == product.Id && s.LocationId == model.ReceivingLocationId);

                        if (stockLevel == null)
                        {
                            stockLevel = new ProductStockLevel
                            {
                                LocationId = model.ReceivingLocationId.Value,
                                ProductId = product.Id,
                                StockAvailable = 0,
                                StockOnHand = 0,
                                StockOnOrder = 0
                            };

                            scope.Context.ProductStockLevel.Add(stockLevel);
                        }

                        stockLevel.StockOnOrder += productModel.QuantityOrdered;
                        scope.Context.SaveChanges();
                    }

                    var mapped = Mapper.Map<PurchaseOrder, PurchaseOrderViewModel>(purchaseOrder);
                    mapped.Products = Mapper.Map<List<PurchaseOrderProduct>, List<PurchaseOrderProductViewModel>>(savedProducts);
                    scope.Complete();
                    return mapped;

            }
        }

        public PurchaseOrderViewModel Update(EditPurchaseOrderViewModel model)
        {
            PurchaseOrderViewModel porder;
            using (var scope = Context.Write())
            {
                var purchaseOrder = scope.Context.PurchaseOrder.Single(po => po.Id == model.Id);
                Mapper.Map(model, purchaseOrder);

                purchaseOrder.VendorId = model.VendorId;
                purchaseOrder.Vendor = scope.Context.Supplier.Where(s => s.Id == model.VendorId).FirstOrDefault().Name.ToString();

                scope.Context.SaveChanges();
                audit.LogAsync(purchaseOrder, PurchaseEvents.Edit, EventCategories.Merchandising);
                var products = scope.Context.PurchaseOrderProduct.Where(pop => pop.PurchaseOrderId == model.Id).ToList();

                foreach (var product in products)
                {
                    var localProduct = product;
                    var modelProduct = model.Products.Single(p => p.Id == localProduct.Id);
                    if (modelProduct.EstimatedDeliveryDate.HasValue && modelProduct.EstimatedDeliveryDate.Value.Date != product.EstimatedDeliveryDate.GetValueOrDefault(DateTime.MaxValue))
                    {
                        product.EstimatedDeliveryDate = modelProduct.EstimatedDeliveryDate;                        
                        audit.LogAsync(product, PurchaseEvents.EditPurchaseOrderProduct, EventCategories.Merchandising);
                    }
                    product.UnitCost = modelProduct.unitCost;
                   // product.PreLandedExtendedCost = modelProduct.lineCost;
                    scope.Context.SaveChanges();
                }

                RefreshStatus(purchaseOrder.Id);
                porder = Get(purchaseOrder.Id);
                scope.Complete();
            }
            return porder;
        }

        public void ForceIndex(int[] orderIds)
        {
            this.purchaseOrderSolrIndexer.Index(orderIds);
        }

        public void Cancel(int id, int cancelReasonId)
        {
            using (var scope = Context.Write())
            {
                var purchaseOrder = scope.Context.PurchaseOrder.Single(po => po.Id == id);
                var pops = scope.Context.PurchaseOrderProduct.Where(pop => pop.PurchaseOrderId == id).ToList();

                Cancel(purchaseOrder, pops, cancelReasonId);
                scope.Context.SaveChanges();
                purchaseOrderSolrIndexer.Index(new[] { id });
                scope.Complete();
            }
        }

        private void Cancel(PurchaseOrder purchaseOrder, List<PurchaseOrderProduct> pops, int cancelReasonId)
        {
            using (var scope = Context.Write())
            {
                var popIds = pops.Select(pop => pop.Id);
                var popStats = scope.Context.PurchaseOrderProductStatsView.Where(s => popIds.Contains(s.Id)).ToDictionary(d => d.Id);
                pops.ForEach(p =>
                {
                    p.QuantityCancelled = p.QuantityOrdered - (popStats.ContainsKey(p.Id) ? popStats[p.Id].QuantityReceived : 0);
                });

                purchaseOrder.Status = popStats.Sum(p => p.Value.QuantityReceived) > 0 ? PurchaseOrderStatuses.Completed : PurchaseOrderStatuses.Cancelled;

                if (cancelReasonId != 0)
                {
                    purchaseOrder.CancelReason = scope.Context.POCancelReasonList.Where(cr => cr.ID == cancelReasonId).FirstOrDefault().CancelReason.ToString();
                }
                scope.Context.SaveChanges();

                productRepository.CancelStock(new int[] { purchaseOrder.Id });

                // 6555907 - Populate the PO's for which StockOnOrder needs to update.           
                var dtPOId = new DataTable();
                dtPOId.Columns.Add("Id", typeof(int));
                dtPOId.Rows.Add(purchaseOrder.Id);
                scope.Context.ProductStockLevelsUpdateOnOrder(dtPOId);

                audit.LogAsync(new { action = "Cancel purchase order", purchaseOrder }, PurchaseEvents.EditPurchaseOrderProduct, EventCategories.Merchandising);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        #region AhsleyCR :-  
        public void POFirstLevelApprove(int id)
        {
            Settings s = new Settings();
            var ashEn = s.AshleyEnabled;
            var AshleyPOAuthorizationLevelFlg = s.AshleyPOAuthorizationLevelFlg;
            using (var scope = Context.Write())
            {

                var purchaseOrder = scope.Context.PurchaseOrder.Single(po => po.Id == id);

                //Check PO Atuthorization Level Setting If First Level PO Authorization True then direct PO Approved  otherwise Partially PO approved 
                if (AshleyPOAuthorizationLevelFlg == 1)
                {
                    purchaseOrder.Status = PurchaseOrderStatuses.Approved;
                }
                else
                {
                    purchaseOrder.Status = PurchaseOrderStatuses.PartiallyApproved;
                }

                scope.Context.SaveChanges();
                purchaseOrderSolrIndexer.Index(new[] { id });
                scope.Complete();
            }
        }

        public void POSecondLevelApprove(int id)
        {
            Settings s = new Settings();
            var ashEn = s.AshleyEnabled;
            using (var scope = Context.Write())
            {
                var purchaseOrder = scope.Context.PurchaseOrder.Single(po => po.Id == id);

                //var POSaleMap = scope.Context.PurchaseOrderSalesOrderMap.Single(p => p.purchaseorder == id);
                //if(POSaleMap.IsApprovedByFirstUp == true && POSaleMap.IsApprovedBySecondUp == true)
                //{
                purchaseOrder.Status = PurchaseOrderStatuses.Approved;
                //}
                scope.Context.SaveChanges();
                purchaseOrderSolrIndexer.Index(new[] { id });
                scope.Complete();
            }
        }

        public void POBothLevelApprove(int id)
        {
            Settings s = new Settings();
            var ashEn = s.AshleyEnabled;
            using (var scope = Context.Write())
            {
                var purchaseOrder = scope.Context.PurchaseOrder.Single(po => po.Id == id);

                //var POSaleMap = scope.Context.PurchaseOrderSalesOrderMap.Single(p => p.purchaseorder == id);
                //if(POSaleMap.IsApprovedByFirstUp == true && POSaleMap.IsApprovedBySecondUp == true)
                //{
                purchaseOrder.Status = PurchaseOrderStatuses.Approved;
                //}
                scope.Context.SaveChanges();
                purchaseOrderSolrIndexer.Index(new[] { id });
                scope.Complete();
            }
        }

        public void POFirstLevelReject(int id)
        {
            using (var scope = Context.Write())
            {
                var purchaseOrder = scope.Context.PurchaseOrder.Single(po => po.Id == id);

                /* var pops = scope.Context.PurchaseOrderProduct.Where(p => p.PurchaseOrderId == id).ToList();
                 foreach (var prodcuct in pops)
                 {
                     var stk = scope.Context.ProductStockLevel.Single(p => p.ProductId == prodcuct.ProductId && p.LocationId == purchaseOrder.ReceivingLocationId);
                     stk.StockOnOrder = stk.StockOnOrder - prodcuct.QuantityOrdered;
                 }*/
                purchaseOrder.Status = PurchaseOrderStatuses.Unapproved;

                scope.Context.SaveChanges();
                purchaseOrderSolrIndexer.Index(new[] { id });
                scope.Complete();
            }
        }

        public void POSecondLevelReject(int id)
        {
            using (var scope = Context.Write())
            {
                var purchaseOrder = scope.Context.PurchaseOrder.Single(po => po.Id == id);

                /*var pops = scope.Context.PurchaseOrderProduct.Where(p => p.PurchaseOrderId == id).ToList();
                foreach (var prodcuct in pops)
                {
                    var stk = scope.Context.ProductStockLevel.Single(p => p.ProductId == prodcuct.ProductId && p.LocationId == purchaseOrder.ReceivingLocationId);
                    stk.StockOnOrder = stk.StockOnOrder - prodcuct.QuantityOrdered;
                }*/

                purchaseOrder.Status = PurchaseOrderStatuses.Unapproved;
                scope.Context.SaveChanges();
                purchaseOrderSolrIndexer.Index(new[] { id });
                scope.Complete();
            }
        }

        public void POBothLevelReject(int id)
        {
            using (var scope = Context.Write())
            {
                var purchaseOrder = scope.Context.PurchaseOrder.Single(po => po.Id == id);

                /*var pops = scope.Context.PurchaseOrderProduct.Where(p => p.PurchaseOrderId == id).ToList();
                foreach (var prodcuct in pops)
                {
                    var stk = scope.Context.ProductStockLevel.Single(p => p.ProductId == prodcuct.ProductId && p.LocationId == purchaseOrder.ReceivingLocationId);
                    stk.StockOnOrder = stk.StockOnOrder - prodcuct.QuantityOrdered;
                }*/

                purchaseOrder.Status = PurchaseOrderStatuses.Unapproved;

                scope.Context.SaveChanges();
                purchaseOrderSolrIndexer.Index(new[] { id });
                scope.Complete();
            }
        }

        public Dictionary<int, string> GetList()
        {
            using (var scope = Context.Read())
            {
                var POCancelReasonList = scope.Context.POCancelReasonList.ToList();
                return POCancelReasonList.ToDictionary(l => l.ID, l => l.CancelReason);
            }
        }

        public Boolean GetAshleyEnable()
        {
            using (var scope = Context.Read())
            {
                Settings s = new Settings();
                var ashleyEnabled = s.AshleyEnabled;
                return ashleyEnabled;
            }
        }

        #endregion
        public PurchaseOrderViewModel CancelProduct(int id, UserSession user, int purchaseOrderProductId)
        {
            using (var scope = Context.Write())
            {
                var purchaseOrder = scope.Context.PurchaseOrder.Single(po => po.Id == id);
                var pops = scope.Context.PurchaseOrderProduct.Where(p => p.PurchaseOrderId == id);
                var popIds = pops.Select(p => p.Id);
                var popStats = scope.Context.PurchaseOrderProductStatsView.Where(s => popIds.Contains(s.Id)).ToList();

                // Update purchase order products
                var pop = pops.Single(p => p.Id == purchaseOrderProductId);
                var stat = popStats.SingleOrDefault(ps => ps.Id == pop.Id);
                var quantityToCancel = stat == null
                                            ? pop.QuantityOrdered
                                            : stat.QuantityPending ?? pop.QuantityOrdered;

                // Cancel Quantity
                pop.QuantityCancelled = quantityToCancel;

                // Adjust stock levels
                productRepository.ReceiveAndCancelStock(
                    new List<StockChange>
                    {
                        new StockChange
                        {
                           ProductId = pop.ProductId,
                           Location = purchaseOrder.ReceivingLocationId,
                           QuantityReceived = 0,
                           QuantityCancelled = quantityToCancel
                        }
                    });

                scope.Context.SaveChanges();
                stockSolrIndexer.Index(new List<int>() { pop.ProductId });

                audit.LogAsync(
                    new { action = "Cancel purchase order product", purchaseOrder, purchaseOrderProductId },
                    PurchaseEvents.EditPurchaseOrderProduct,
                    EventCategories.Merchandising);

                // 6555907 - Populate the PO's for which StockOnOrder needs to update.
                var dtPOIds = new DataTable();
                dtPOIds.Columns.Add("Id", typeof(int));
                dtPOIds.Rows.Add(purchaseOrder.Id);
                scope.Context.ProductStockLevelsUpdateOnOrder(dtPOIds);

                RefreshStatus(id);
                scope.Context.SaveChanges();
                scope.Complete();
            }
            return Get(id);
        }

        public void RefreshStatus(int purchaseOrderId)
        {
            using (var scope = Context.Write())
            {
                var purchaseOrder = scope.Context.PurchaseOrder.Find(purchaseOrderId);
                if (purchaseOrder.Status == PurchaseOrderStatuses.Completed || purchaseOrder.Status == PurchaseOrderStatuses.Cancelled || purchaseOrder.Status == PurchaseOrderStatuses.Unapproved)
                {
                    return;
                }

                var pops = scope.Context.PurchaseOrderProduct.Where(pop => pop.PurchaseOrderId == purchaseOrderId).ToList();
                var popIds = pops.Select(pop => pop.Id).ToList();
                var stats = scope.Context.PurchaseOrderProductStatsView.Where(p => popIds.Contains(p.Id)).ToList();
                var today = DateTime.Now.Date;
                var expired = pops.Where(p => p.PurchaseOrderId == purchaseOrderId).All(p => p.EstimatedDeliveryDate < today);

                var partiallyReceived = pops.Any(
                    pop =>
                    {
                        var stat = stats.SingleOrDefault(s => s.Id == pop.Id);
                        if (stat == null)
                        {
                            return false;
                        }

                        if (stat.QuantityReceived > 0)
                        {
                            return true;
                        }
                        return false;
                    });

                var complete = pops.All(
                    pop =>
                    {
                        var stat = stats.SingleOrDefault(s => s.Id == pop.Id);
                        if (stat == null)
                        {
                            return false;
                        }
                        if (stat.QuantityPending > 0)
                        {
                            return false;
                        }
                        return true;
                    });

                var cancelled = complete && !partiallyReceived;

                if (partiallyReceived)
                {
                    purchaseOrder.Status = PurchaseOrderStatuses.PartiallyReceived;
                }

                if (expired && purchaseOrder.Status != PurchaseOrderStatuses.Expired)
                {
                    purchaseOrder.Status = PurchaseOrderStatuses.Expired;
                    purchaseOrder.ExpiredDate = today;
                }

                if (complete)
                {
                    purchaseOrder.Status = PurchaseOrderStatuses.Completed;
                }

                if (cancelled)
                {
                    purchaseOrder.Status = PurchaseOrderStatuses.Cancelled;
                }

                if (!cancelled && !complete && !partiallyReceived && !expired)
                {
                    purchaseOrder.Status = PurchaseOrderStatuses.New;
                }

                scope.Context.SaveChanges();
                purchaseOrderSolrIndexer.Index(new[] { purchaseOrder.Id });
                scope.Complete();
            }
        }

        public PurchaseOrderViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var purchaseOrder = scope.Context.PurchaseOrder.Find(id);
                var purchaseOrderProducts = scope.Context.PurchaseOrderProduct.Where(pop => pop.PurchaseOrderId == id).ToList();
                var popIds = purchaseOrderProducts.Select(pop => pop.Id).ToList();
                var stats = scope.Context.PurchaseOrderProductStatsView.Where(p => popIds.Contains(p.Id)).ToList();


                var model = Mapper.Map<PurchaseOrder, PurchaseOrderViewModel>(purchaseOrder);
                model.Products = Mapper.Map<List<PurchaseOrderProduct>, List<PurchaseOrderProductViewModel>>(purchaseOrderProducts);
                
                /// Code Added by Abhijeet for Additional Cost Price 
                foreach(var p in model.Products)
                {
                    p.AdditionalCostPrice = scope.Context.AdditionalCostPrices.ToList().SingleOrDefault(q=> q.ProductId == p.ProductId); // GetAdditionalCostPrice(p.ProductId);
                }
                ///

                model.FormattedTotalCost = model.Products.Sum(p => p.UnitCost * p.QuantityOrdered).ToCurrency();

                model.Products.Each(pop =>
                {
                    var stat = stats.FirstOrDefault(p => p.Id == pop.Id);
                    pop.QuantityPending = stat != null ? stat.QuantityPending ?? (pop.QuantityOrdered - pop.QuantityCancelled) : (pop.QuantityOrdered - pop.QuantityCancelled);
                    pop.FormattedPendingCost = (pop.QuantityPending * pop.UnitCost).ToCurrency();
                    pop.TotalQuantityReceived = stat != null ? stat.QuantityReceived ?? 0 : 0;
                    pop.QuantityCancelled = stat != null ? stat.QuantityCancelled ?? pop.QuantityCancelled : pop.QuantityCancelled;
                });
                model.CreatedDate = DateTime.SpecifyKind(model.CreatedDate, DateTimeKind.Utc).ToLocalTime();
                return model;
            }
        }

        public PurchaseOrderPrintModel GetForPrint(int id)
        {
            var order = Get(id);

            var model = Mapper.Map<PurchaseOrderViewModel, PurchaseOrderPrintModel>(order);
            model.VendorDetails = supplierRepository.Get(model.VendorId);
            model.LocationDetails = locationRepository.Get(model.ReceivingLocationId);

            return model;
        }

        public List<LabelModel> GetPrintLabels(List<PurchaseOrderProductLabelModel> products)
        {
            using (var scope = Context.Read())
            {
                var ids = products.Select(p => p.Id).ToList();

                var prods = scope.Context.PrintLabelView.Where(p => ids.Contains(p.PurchaseOrderProductId)).ToList();

                return prods.Select(
                    p => new LabelModel()
                    {
                        CorporateUPC = p.CorporateUPC,
                        Brand = p.BrandName,
                        Description = p.LongDescription,
                        ModelNumber = p.VendorStyleLong,
                        Sku = p.SKU,
                        PurchaseOrderId = p.PurchaseOrderId,
                        VendorName = p.Vendor,
                        DateReceived = DateTime.Now,
                        TotalBoxes = p.BoxCount,
                        QuantityToPrint = products.Single(pr => pr.Id == p.PurchaseOrderProductId).PrintQuantity
                    }).ToList();
            }
        }

        public PurchaseOrderViewModel GetForReceipt(int id)
        {
            var model = Get(id);

            //Return only products where TotalQuantityReceived > 0
            model.Products = model.Products.FindAll(p => p.QuantityPending > 0);

            return model;
        }

        public List<PurchaseOrderViewModel> GetNotReceived(int? vendorId = null)
        {
            using (var scope = Context.Read())
            {
                var pos = scope.Context.PurchaseOrder.Where(p =>
                    p.Status != PurchaseOrderStatuses.Cancelled &&
                    p.Status != PurchaseOrderStatuses.Completed);

                if (vendorId.HasValue)
                {
                    pos = pos.Where(p => p.VendorId == vendorId);
                }

                return pos.ToList().Select(p => Get(p.Id)).ToList();
            }
        }

        public void UpdatePrint(int id, bool withCost)
        {
            using (var scope = Context.Write())
            {
                bool original = false;
                var order = scope.Context.PurchaseOrder.Single(p => p.Id == id);
                if (order.OriginalPrint == null)
                {
                    order.OriginalPrint = DateTime.UtcNow;
                    scope.Context.SaveChanges();
                    original = true;
                }
                audit.LogAsync(new { OriginalPrint = original, PurchaseOrder = this.Get(id) }, withCost ? PurchaseEvents.PrintPurchaseOrderWithCost : PurchaseEvents.PrintPurchaseOrderWithoutCost, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public bool CorporatePoNumberIsUnique(string originSystem, string corporatePoNumber)
        {
            using (var scope = Context.Read())
            {
                return !scope.Context.PurchaseOrder.Any(po => po.OriginSystem == originSystem && po.CorporatePoNumber == corporatePoNumber);
            }
        }

        public void AutoExpire()
        {
            using (var scope = Context.Write())
            {
                var now = DateTime.UtcNow.Date;
                var expiredIds = scope.Context.AutoExpirePurchaseOrders(new AutoExpirePurchaseOrdersInput { ExpireDate = now, DaysTillCancel = new Settings().DaysUntilAutoCancelPurchaseOrder }).Result;
                var canceledIds = scope.Context.AutoCancelPurchaseOrders(new AutoCancelPurchaseOrdersInput { CancelDate = now, DaysTillCancel = new Settings().DaysUntilAutoCancelPurchaseOrder }).Result;

                productRepository.CancelStock(canceledIds.Select(c => c.Id));
                var pos = expiredIds.Select(e => e.Id).Concat(canceledIds.Select(c => c.Id)).Distinct().ToArray();

                var productIds = GetPurchaseOrderProducts(pos.ToList());
                this.stockSolrIndexer.Index(productIds);

                // 6555907 - Populate the PO's for which StockOnOrder needs to update.
                var dtPOIds = new DataTable();
                dtPOIds.Columns.Add("Id", typeof(int));
                pos.Each(r => dtPOIds.Rows.Add(r));

                scope.Context.ProductStockLevelsUpdateOnOrder(dtPOIds);

                this.purchaseOrderSolrIndexer.Index(pos);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public List<int> GetPurchaseOrderProducts(List<int> purchaseOrderIds)
        {
            using (var scope = Context.Read())
            {
                var productIds =
                    scope.Context.PurchaseOrderProduct
                        .Where(p => purchaseOrderIds.Contains(p.PurchaseOrderId))
                        .Select(p => p.ProductId)
                        .Distinct()
                        .ToList();

                return productIds;
            }
        }



        List<ForceIndexAutoPurchaseOrder> IPurchaseRepository.GetForceIndexAutoPurchaseOrder()
        {
            using (var scope = Context.Read())
            {
                var purchaseOrder = scope.Context.ForceIndexAutoPurchaseOrder
                    .Where(p => p.IsReIndexed == false)
                    .ToList();
                return purchaseOrder;
            }
        }
        void IPurchaseRepository.UpdateForceIndexAutoPurchaseOrder(int[] ids)
        {
            using (var scope = Context.Write())
            {
                foreach (var id in ids)
                {
                    var poIndex = scope.Context.ForceIndexAutoPurchaseOrder.Single(p => p.PurchaseId == id);
                    poIndex.IsReIndexed = true;
                }
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }
        /// <summary>
        /// Author : Rahul Dubey
        /// Date   : 18/02/2019
        /// CR     : #Ashley
        /// Details: Auto Create PO JOB
        /// </summary>
        /// <returns></returns>
        public void AutoCreate()
        {
            using (var scope = Context.Read())
            {
                scope.Context.Database.ExecuteSqlCommand("CreateAutoPO");
            }
        }
    }
}
