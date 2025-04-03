namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Admin;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Publishers;
    using Blue.Cosacs.Messages.Merchandising.GoodsReceiptEmail;
    using Blue.Events;
    using Blue.Hub.Client;
    using MoreLinq;
    using Solr;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using Context = Blue.Cosacs.Merchandising.Context;
    using Settings = Blue.Cosacs.Merchandising.Settings;
    public interface IGoodsReceiptRepository
    {
        GoodsReceiptViewModel Get(int id);

        GoodsReceiptViewModel Create(GoodsReceiptCreateModel model, UserSession user, Func<int, string> reviewUrl, Func<int, string, string> productUrl);

        void Approve(int id, int userId, string comments);

        void ConfirmCosts(int id, int userId);

        void UpdateComments(int id, string comments);

        GoodsReceiptCostsModel GetCosts(int id);

        GoodsReceiptPrintModel Print(int id, bool withCost);

        List<GoodsReceiptPrintModel> BulkPrint(List<int> ids, bool withCost);

        void RecreateDataForResume();
    }

    public class GoodsReceiptRepository : IGoodsReceiptRepository
    {
        private readonly IEventStore audit;
        private readonly IProductRepository productRepository;
        private readonly IPurchaseRepository purchaseRepository;
        private readonly IGoodsReceiptPublisher goodsReceiptPublisher;
        private readonly IStockSolrIndexer stockSolrIndexer;

        private readonly IPublisher publisher;

        private readonly Settings settings;

        public GoodsReceiptRepository(IEventStore audit,
                                      IProductRepository productRepository,
                                      IPurchaseRepository purchaseRepository,
                                      IGoodsReceiptPublisher goodsReceiptPublisher,
                                      IPublisher publisher,
                                      IStockSolrIndexer stockSolrIndexer,
                                      Settings settings)
        {
            this.audit = audit;
            this.productRepository = productRepository;
            this.purchaseRepository = purchaseRepository;
            this.goodsReceiptPublisher = goodsReceiptPublisher;
            this.publisher = publisher;
            this.settings = settings;
            this.stockSolrIndexer = stockSolrIndexer;
        }

        public GoodsReceiptViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var goodsReceipt = scope.Context.GoodsReceipt.Single(x => x.Id == id);
                var location = scope.Context.Location.Single(x => x.Id == goodsReceipt.LocationId);
                var goodsReceiptProducts = scope.Context.GoodsReceiptProductView.Where(x => x.GoodsReceiptId == id).ToList();

                return MapGoodsReceiptViewModel(goodsReceipt, goodsReceiptProducts, location);
            }
        }

        private bool InvalidRecievedQuantity(GoodsReceiptCreateModel model)
        {
            using (var scope = Context.Read())
            {
                var poids = model.PurchaseOrders.Select(s => s.Id);
                var onOrder = scope.Context.ProductsOnOrder.Where(p => poids.Contains(p.PurchaseOrderId))
                              .ToDictionary(d => new { d.ProductId, d.PurchaseOrderId });

                return (from po in model.PurchaseOrders
                        from p in po.Products
                        where onOrder[new { ProductId = p.ProductId, PurchaseOrderId = po.Id }].OnOrder < p.QuantityReceived
                        select po.Id).Any();
            }
        }

        public GoodsReceiptViewModel Create(GoodsReceiptCreateModel model, UserSession user, Func<int, string> reviewUrl, Func<int, string, string> productUrl)
        {
            using (var scope = Context.Write())
            {
                if (InvalidRecievedQuantity(model))
                {
                    throw new Exception("Received quantity is greater than stock on order.");
                }

                var goodsReceipt = Mapper.Map<GoodsReceipt>(model);

                // Save the goods receipt
                goodsReceipt.Location = scope.Context.Location.Find(model.LocationId).Name;
                goodsReceipt.CreatedDate = DateTime.UtcNow;
                goodsReceipt.CreatedById = user.Id;
                goodsReceipt.CreatedBy = user.FullName;
                goodsReceipt.ReceivedBy = scope.Context.UserPermissionsView.First(u => u.UserId == model.ReceivedById).FullName;
                scope.Context.GoodsReceipt.Add(goodsReceipt);
                scope.Context.SaveChanges();

                // Save the goods receipt products
                var goodsReceiptProducts = model.PurchaseOrders
                    .SelectMany(po => po.Products)
                    .Select(p =>
                    {
                        var goodsReceiptProduct = Mapper.Map<GoodsReceiptProduct>(p);
                        goodsReceiptProduct.GoodsReceiptId = goodsReceipt.Id;
                        goodsReceiptProduct.PurchaseOrderProductId = p.Id;
                        return goodsReceiptProduct;
                    })
                    .ToList();

                goodsReceiptProducts.ForEach(g =>
                {
                    scope.Context.Entry(g).State = EntityState.Added;
                });
                scope.Context.SaveChanges();                

                // Updated purchase order statuses
                foreach (var purchaseOrder in model.PurchaseOrders)
                {
                    purchaseRepository.RefreshStatus(purchaseOrder.Id);
                }

                var outstandingGoodsReceiptCosts = OutstandingGoodsReceiptCosts(goodsReceipt);
                var costsOk = outstandingGoodsReceiptCosts == 0;

                var supplier = GetSupplier(model);


                var popIds = goodsReceiptProducts.Select(g => g.PurchaseOrderProductId).ToList();
                var productIds = scope.Context.PurchaseOrderProduct.Where(pop => popIds.Contains(pop.Id)).Select(pop => new { id = pop.ProductId, sku = pop.Sku }).ToList();

                // Update stock and average weighted costs if costing ok
                if (supplier.Type.ToLower() == "local" || !settings.VerifyCosts || costsOk)
                {
                    goodsReceipt.CostConfirmed = DateTime.UtcNow;
                    UpdateProductStockAndCosts(goodsReceipt.Id, goodsReceipt.LocationId, goodsReceiptProducts);
                }
                else if (settings.VerifyCosts)
                {
                    var productUrls = productIds.Select(p => productUrl(p.id, p.sku)).ToArray();
                    this.SendReviewEmail(reviewUrl, productUrls, goodsReceipt, outstandingGoodsReceiptCosts);
                }

                // UnAge
                productRepository.UnAge(productIds.Select(s => s.id));

                var viewModel = Get(goodsReceipt.Id);
                viewModel.EnableBackOrderCancel = settings.BackOrders;
                viewModel.VendorType = supplier.Type;
                stockSolrIndexer.Index(productIds.Select(s => s.id));
                audit.LogAsync(viewModel, GoodsReceivedEvents.Create, EventCategories.Merchandising);
                scope.Complete();
                return viewModel;
            }
        }

        private Supplier GetSupplier(GoodsReceiptCreateModel model)
        {
            using (var scope = Context.Read())
            {
                var firstPoModel = model.PurchaseOrders.First();
                var firstPo = scope.Context.PurchaseOrder.Single(po => po.Id == firstPoModel.Id);
                var supplier = scope.Context.Supplier.Single(s => s.Id == firstPo.VendorId);
                return supplier;
            }
        }

        private void UpdateProductStockAndCosts(int id, int locationId, List<GoodsReceiptProduct> goodsReceiptProducts)
        {
            using (var scope = Context.Write())
            {
                var popIds = goodsReceiptProducts.Select(s => s.PurchaseOrderProductId).ToList();

                var costs = (from c in scope.Context.CurrentCostPriceView
                             join pop in scope.Context.PurchaseOrderProduct on c.ProductId equals pop.ProductId
                             where popIds.Contains(pop.Id)
                             select new { landedCost = c.LastLandedCost, popId = pop.Id, poId = pop.PurchaseOrderId }).ToDictionary(d => d.popId);

                goodsReceiptProducts.ForEach(g =>
                {
                    g.LastLandedCost = costs[g.PurchaseOrderProductId].landedCost;
                });

                scope.Context.SaveChanges();
                productRepository.ReceiveAndCancelStock(goodsReceiptProducts, false);
                
                // 6555907 - Populate the PO's for which StockOnOrder needs to update.
                var poIds = (from pop in scope.Context.PurchaseOrderProduct
                             where popIds.Contains(pop.Id)
                             select pop.PurchaseOrderId).Distinct().ToList();
                var dtpurchaseOrderIds = new DataTable();
                dtpurchaseOrderIds.Columns.Add("Id", typeof(int));
                poIds.Each(r => dtpurchaseOrderIds.Rows.Add(r));
                scope.Context.ProductStockLevelsUpdateOnOrder(dtpurchaseOrderIds);

                var viewModel = Get(id);
                goodsReceiptPublisher.PublishCreated(viewModel);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void SendReviewEmail(Func<int, string> reviewUrl, string[] productUrls, GoodsReceipt goodsReceipt, int itemsToReviewCount)
        {
            this.publisher.Publish<Context, GoodsReceiptReview>(
                "Merchandising.GoodsReceipt",
                new GoodsReceiptReview()
                {
                    AbsoluteReviewUrl = reviewUrl(goodsReceipt.Id),
                    FinanceEmailAddress = settings.FinanceEmailAddress,
                    GoodsReceiptId = goodsReceipt.Id,
                    NumberOfItemsToReview = itemsToReviewCount,
                    ProductURLs = productUrls
                });
        }

        public void Approve(int id, int userId, string comments)
        {
            using (var scope = Context.Write())
            {
                var goodsReceipt = scope.Context.GoodsReceipt.Single(x => x.Id == id);
                var user = scope.Context.UserPermissionsView.First(u => u.UserId == userId).FullName;

                if (goodsReceipt.DateApproved.HasValue)
                {
                    throw new ArgumentException(string.Format("The goods receipt with id {0} has already been approved", id));
                }
                goodsReceipt.ApprovedBy = user;
                goodsReceipt.ApprovedById = userId;
                goodsReceipt.DateApproved = DateTime.UtcNow;
                goodsReceipt.Comments = comments;

                // 6555907 - Populate the PO's for which StockOnOrder needs to update.
                var poIds = (from grnpIds in scope.Context.GoodsReceiptProduct
                             join popsIds in scope.Context.PurchaseOrderProduct
                             on grnpIds.PurchaseOrderProductId equals popsIds.Id
                             where grnpIds.GoodsReceiptId == goodsReceipt.Id
                             select popsIds.PurchaseOrderId).Distinct().ToList();
                var dtpurchaseOrderIds = new DataTable();
                dtpurchaseOrderIds.Columns.Add("Id", typeof(int));
                poIds.Each(r => dtpurchaseOrderIds.Rows.Add(r));
                scope.Context.ProductStockLevelsUpdateOnOrder(dtpurchaseOrderIds);

                scope.Context.SaveChanges();                
                audit.LogAsync(new { id, userId, comments }, GoodsReceivedEvents.Approve, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public void ConfirmCosts(int id, int userId)
        {
            using (var scope = Context.Write())
            {
                // Update this receipt' product cost and stock levels 
                var goodsReceipt = scope.Context.GoodsReceipt.Single(x => x.Id == id);
                var goodsReceiptProducts = scope.Context.GoodsReceiptProduct.Where(grp => grp.GoodsReceiptId == id).ToList();
                UpdateProductStockAndCosts(goodsReceipt.Id, goodsReceipt.LocationId, goodsReceiptProducts);

                // Save the confirmation details
                var user = scope.Context.UserPermissionsView.First(u => u.UserId == userId).FullName;

                if (goodsReceipt.CostConfirmed.HasValue)
                {
                    throw new ArgumentException(string.Format("Costs for the goods receipt with id {0} have already been confirmed", id));
                }
                goodsReceipt.CostConfirmedBy = user;
                goodsReceipt.CostConfirmedById = userId;
                goodsReceipt.CostConfirmed = DateTime.UtcNow;
                scope.Context.SaveChanges();

                audit.LogAsync(new { id, userId }, GoodsReceivedEvents.Confirm, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public void UpdateComments(int id, string comments)
        {
            using (var scope = Context.Write())
            {
                var goodsReceipt = scope.Context.GoodsReceipt.Single(x => x.Id == id);

                if (goodsReceipt.DateApproved.HasValue)
                {
                    throw new ArgumentException(string.Format("The goods receipt with id {0} has already been approved and cannot be updated", id));
                }

                goodsReceipt.Comments = comments;

                scope.Context.SaveChanges();

                audit.LogAsync(goodsReceipt, GoodsReceivedEvents.Update, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public GoodsReceiptCostsModel GetCosts(int id)
        {
            using (var scope = Context.Read())
            {
                var goodsReceipt = scope.Context.GoodsReceipt.Single(x => x.Id == id);
                var model = Mapper.Map<GoodsReceiptCostsModel>(goodsReceipt);
                var prods = scope.Context.GoodsReceiptProductView.Where(x => x.GoodsReceiptId == id && x.QuantityReceived > 0).ToList();

                var productIds = prods.Select(p => p.ProductId);
                var costs = scope.Context.CurrentCostPriceView.Where(c => productIds.Contains(c.ProductId)).GroupBy(p => p.ProductId).ToList();
                var productCosts = costs.Select(g => g.OrderByDescending(c => c.LastLandedCostUpdated).First()).Select(c => new { c.ProductId, c.LastLandedCost, c.AverageWeightedCost, c.LastLandedCostUpdated }).ToList();
                var prices = scope.Context.CurrentRetailPriceView.Where(c => productIds.Contains(c.ProductId)).ToList();
                var groups = prods.GroupBy(p => p.PurchaseOrderId);

                groups.ForEach(g =>
                {
                    var poModel = new GoodsReceiptCostsPurchaseOrderModel() { Id = g.Key };
                    var purchaseOrder = scope.Context.PurchaseOrder.FirstOrDefault(p => p.Id == poModel.Id);
                    poModel.Vendor = purchaseOrder.Vendor;
                    poModel.VendorId = purchaseOrder.VendorId;
                    prods.Where(p => p.PurchaseOrderId == g.Key).ForEach(gp =>
                    {
                        var thisCost = productCosts.First(c => c.ProductId == gp.ProductId);
                        var thisPrice = prices.Where(c => c.ProductId == gp.ProductId && (c.LocationId == goodsReceipt.LocationId || !c.LocationId.HasValue)).OrderBy(
                            c =>
                            {
                                if (string.IsNullOrWhiteSpace(c.Fascia) && !c.LocationId.HasValue)
                                {
                                    return 3;
                                }
                                if (!string.IsNullOrWhiteSpace(c.Fascia) && !c.LocationId.HasValue)
                                {
                                    return 2;
                                }
                                if (string.IsNullOrWhiteSpace(c.Fascia) && c.LocationId.HasValue)
                                {
                                    return 1;
                                }
                                return int.MaxValue;
                            }).FirstOrDefault();
                        var thisCashPrice = thisPrice == null ? 0M : thisPrice.CashPrice ?? 0M;

                        poModel.Products.Add(
                            new GoodsReceiptCostsProductModel()
                            {
                                Id = gp.Id,
                                ProductId = gp.ProductId,
                                ProductCode = gp.ProductCode,
                                Comments = gp.Comments,
                                Description = gp.Description,
                                GoodsReceiptDate = goodsReceipt.DateReceived,
                                GoodsReceiptId = goodsReceipt.Id,
                                LandedCostLastUpdated = thisCost.LastLandedCostUpdated,
                                UnitLandedCost = thisCost.LastLandedCost,
                                TotalLandedCost = thisCost.LastLandedCost * gp.QuantityReceived,
                                QuantityReceived = gp.QuantityReceived,
                                LastReceivedDate = scope.Context.GoodsReceiptProductView
                                        .Where(p => p.ProductId == gp.ProductId && p.GoodsReceiptId != gp.GoodsReceiptId && p.QuantityReceived > 0)
                                        .OrderByDescending(p => p.DateReceived)
                                        .Select(p => p.DateReceived)
                                        .FirstOrDefault(),
                                LastCostsConfirmedDate = scope.Context.GoodsReceiptProductView
                                        .Where(p => p.ProductId == gp.ProductId && p.GoodsReceiptId != gp.GoodsReceiptId && p.QuantityReceived > 0)
                                        .OrderByDescending(p => p.CostConfirmed)
                                        .Select(p => p.CostConfirmed)
                                        .FirstOrDefault(),
                                Margin = thisCashPrice <= 0 || (thisCost.AverageWeightedCost) == 0 ? 0 : (thisCashPrice - thisCost.AverageWeightedCost) / thisCashPrice
                            });
                    });
                    model.PurchaseOrders.Add(poModel);
                });

                return model;
            }
        }

        private int OutstandingGoodsReceiptCosts(GoodsReceipt goodsReceipt)
        {
            using (var scope = Context.Read())
            {
                var currentGoodsReceiptProducts = scope.Context.GoodsReceiptProductView
                    .Where(x => x.GoodsReceiptId == goodsReceipt.Id
                             && x.QuantityReceived > 0)
                    .Select(c => c.ProductId);

                var relatedGoodsReceiptProducts = scope.Context.GoodsReceiptProductView
                    .Where(x => x.GoodsReceiptId != goodsReceipt.Id
                             && x.QuantityReceived > 0
                             && currentGoodsReceiptProducts.Contains(x.ProductId));

                var lastVerifiedGoodsReceipts = relatedGoodsReceiptProducts
                    .Where(p => p.CostConfirmed != null)
                    .GroupBy(p => p.ProductId)
                    .ToDictionary(p => p.Key, g => g.OrderByDescending(i => i.CostConfirmed).First());

                var productsToCheck = lastVerifiedGoodsReceipts.Values;
                var productIds = productsToCheck.Select(p => p.ProductId).ToList();

                var costs = scope.Context.CostPrice
                    .Where(c => productIds.Contains(c.ProductId))
                    .GroupBy(c => c.ProductId)
                    .ToDictionary(c => c.Key, g => g.OrderByDescending(i => i.LastLandedCostUpdated).First());

                return productsToCheck.Count(p => costs[p.ProductId].LastLandedCostUpdated < lastVerifiedGoodsReceipts[p.ProductId].CostConfirmed);
            }
        }

        public GoodsReceiptPrintModel Print(int id, bool withCost)
        {
            using (var scope = Context.Write())
            {
                var goodsReceiptPrint = scope.Context.GoodsReceipt.Single(x => x.Id == id);
                var model = Mapper.Map<GoodsReceiptPrintModel>(goodsReceiptPrint);
                model.Products = scope.Context.GoodsReceiptProductView.Where(x => x.GoodsReceiptId == id && x.QuantityReceived > 0).ToList();
                bool original = false;

                if (goodsReceiptPrint.OriginalPrint == null)
                {
                    original = true;
                    goodsReceiptPrint.OriginalPrint = DateTime.UtcNow;
                    scope.Context.SaveChanges();
                }

                audit.LogAsync(new { OriginalPrint = original, GoodsReceipt = this.Get(id) }, withCost ? GoodsReceivedEvents.PrintGoodsReceiptWithCost : GoodsReceivedEvents.PrintGoodsReceiptPrintWithoutCost, EventCategories.Merchandising);
                scope.Complete();
                return model;
            }
        }

        public List<GoodsReceiptPrintModel> BulkPrint(List<int> ids, bool withCost)
        {
            var receipts = new List<GoodsReceiptPrintModel>();
            var audits = new List<Tuple<object, string>>();
            using (var scope = Context.Write())
            {
                var goodsReceiptPrints = scope.Context.GoodsReceipt.Where(x => ids.Contains(x.Id));
                var products = scope.Context.GoodsReceiptProductView.Where(x => ids.Contains(x.GoodsReceiptId) && x.QuantityReceived > 0).ToList();

                foreach (var goodsReceiptPrint in goodsReceiptPrints)
                {
                    var model = Mapper.Map<GoodsReceiptPrintModel>(goodsReceiptPrint);
                    receipts.Add(model);
                    model.Products = products.Where(x => x.GoodsReceiptId == goodsReceiptPrint.Id).ToList();
                    bool original = false;

                    if (goodsReceiptPrint.OriginalPrint == null)
                    {
                        original = true;
                        goodsReceiptPrint.OriginalPrint = DateTime.UtcNow;
                    }

                    audits.Add(new Tuple<object, string>(
                        new { OriginalPrint = original, GoodsReceipt = this.Get(goodsReceiptPrint.Id) },
                        withCost ? GoodsReceivedEvents.PrintGoodsReceiptWithCost : GoodsReceivedEvents.PrintGoodsReceiptPrintWithoutCost));
                }

                scope.Context.SaveChanges();

                audits.ForEach(a =>
                {
                    audit.LogAsync(a.Item1, a.Item2, EventCategories.Merchandising);
                });
                scope.Complete();
                return receipts;
            }
        }

        private GoodsReceiptViewModel MapGoodsReceiptViewModel(GoodsReceipt goodsReceipt, IEnumerable<GoodsReceiptProductView> goodsReceiptProducts, Location location)
        {
            var viewModel = Mapper.Map<GoodsReceiptViewModel>(goodsReceipt);
            viewModel.SalesLocationId = location.SalesId;
            var purchaseOrders = goodsReceiptProducts.DistinctBy(p => p.PurchaseOrderId).ToDictionary(g => g.PurchaseOrderId, Mapper.Map<GoodsReceiptPurchaseOrderViewModel>);
            var purchaseOrderProducts = goodsReceiptProducts.GroupBy(g => g.PurchaseOrderId, Mapper.Map<GoodsReceiptProductViewModel>).ToList();
            var productIds = new List<int>();
            foreach (var x in purchaseOrders)
            {
                x.Value.Products = purchaseOrderProducts.Where(p => p.Key == x.Key).SelectMany(p => p).ToList();
                productIds.AddRange(x.Value.Products.Select(p => p.ProductId));
            }

            viewModel.VendorType = goodsReceiptProducts.First().VendorType;
            viewModel.PurchaseOrders = purchaseOrders.Select(p => p.Value).ToList();

            return viewModel;
        }

        public void RecreateDataForResume()
        {
            using (var scope = Context.Write())
            {
                scope.Context.RecreateDataForGoodsReceiptResume();
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }
    }
}