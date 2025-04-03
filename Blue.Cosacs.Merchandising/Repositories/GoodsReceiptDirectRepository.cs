namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Admin;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Publishers;
    using Blue.Events;
    using Solr;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context = Blue.Cosacs.Merchandising.Context;
    public interface IGoodsReceiptDirectRepository
    {
        GoodsReceiptDirectViewModel Get(int id);

        GoodsReceiptDirectViewModel Create(GoodsReceiptDirectCreateModel model, UserSession user);

        void Update(int id, List<StringKeyValue> referenceNumbers, string comments);

        void Approve(int id, int userId, string comments);

        List<GoodsReceiptDirectPrintModel> BulkPrint(List<int> ids, bool includeCosts);

        GoodsReceiptDirectPrintModel Print(int id, bool withCost);
    }

    public class GoodsReceiptDirectRepository : IGoodsReceiptDirectRepository
    {
        private readonly IEventStore audit;

        private readonly IProductRepository productRepository;
        private readonly IGoodsReceiptDirectPublisher publisher;
        private readonly ICostRepository costRepository;
        private readonly IStockSolrIndexer stockSolrIndexer;

        public GoodsReceiptDirectRepository(
            IEventStore audit,
            IProductRepository productRepository,
            IGoodsReceiptDirectPublisher publisher,
            ICostRepository costRepository,
            IStockSolrIndexer stockSolrIndexer)
        {
            this.audit = audit;
            this.productRepository = productRepository;
            this.publisher = publisher;
            this.costRepository = costRepository;
            this.stockSolrIndexer = stockSolrIndexer;
        }

        public GoodsReceiptDirectViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var goodsReceipt = scope.Context.GoodsReceiptDirect.Single(x => x.Id == id);
                var goodsReceiptProducts = scope.Context.GoodsReceiptDirectProduct.Where(x => x.GoodsReceiptDirectId == id).ToList();
                var location = scope.Context.Location.Single(x => x.Id == goodsReceipt.LocationId);

                return MapGoodsReceiptViewModel(goodsReceipt, goodsReceiptProducts, location);
            }
        }

        public GoodsReceiptDirectViewModel Create(GoodsReceiptDirectCreateModel model, UserSession user)
        {
            using (var scope = Context.Write())
            {
                var goodsReceiptDirect = Mapper.Map<GoodsReceiptDirect>(model);
                goodsReceiptDirect.CreatedDate = DateTime.UtcNow;
                goodsReceiptDirect.CreatedById = user.Id;
                goodsReceiptDirect.CreatedBy = user.FullName;
                goodsReceiptDirect.Location = scope.Context.Location.Find(model.LocationId).Name;
                goodsReceiptDirect.ReceivedBy = scope.Context.UserPermissionsView.First(u => u.UserId == model.ReceivedById).FullName;

                var supplier = scope.Context.Supplier.First(s => s.Id == model.VendorId);
                goodsReceiptDirect.Vendor = supplier.Name;
                goodsReceiptDirect.Currency = supplier.Currency;
                scope.Context.GoodsReceiptDirect.Add(goodsReceiptDirect);
                scope.Context.SaveChanges();

                // retrieve all product info we will need for populating GoodsReceiptProducts
                var productIds = model.Products.Select(p => p.ProductId).Distinct();
                var products = scope.Context.Product.Where(p => productIds.Contains(p.Id)).Select(p => new { p.Id, p.SKU, p.LongDescription }).ToList();
                var landedCosts = costRepository.GetCurrentByProducts(productIds.ToList());

                var goodsReceiptProducts = model.Products
                    .Select(p =>
                    {
                        var product = products.First(prd => prd.Id == p.ProductId);
                        var landedCost = landedCosts.First(c => c.ProductId == p.ProductId);
                        var goodsReceiptProduct = Mapper.Map<GoodsReceiptDirectProduct>(p);
                        goodsReceiptProduct.GoodsReceiptDirectId = goodsReceiptDirect.Id;
                        goodsReceiptProduct.Sku = product.SKU;
                        goodsReceiptProduct.Description = product.LongDescription;
                        goodsReceiptProduct.UnitLandedCost = landedCost.LastLandedCost;
                        return goodsReceiptProduct;
                    })
                    .ToList();

                scope.Context.GoodsReceiptDirectProduct.AddRange(goodsReceiptProducts);
                scope.Context.SaveChanges();

                productRepository.ReceiveAndCancelStock(
                    goodsReceiptProducts.Select(g =>
                    new StockChange
                    {
                        ProductId = g.ProductId,
                        Location = goodsReceiptDirect.LocationId,
                        QuantityReceived = g.QuantityReceived,
                        QuantityCancelled = 0
                    }));

                productRepository.UnAge(productIds);

                var viewModel = Get(goodsReceiptDirect.Id);
                viewModel.VendorType = supplier.Type;
                publisher.PublishCreated(viewModel);
                stockSolrIndexer.Index(productIds);
                scope.Context.ProductStockLevelsUpdateOnOrder();
                audit.LogAsync(viewModel, GoodsReceivedEvents.CreateDirect, EventCategories.Merchandising);
                scope.Context.SaveChanges();
                scope.Complete();
                return viewModel;
            }
        }

        public void Update(int id, List<StringKeyValue> referenceNumbers, string comments)
        {
            using (var scope = Context.Write())
            {
                var goodsReceipt = scope.Context.GoodsReceiptDirect.Single(x => x.Id == id);

                if (goodsReceipt.DateApproved.HasValue)
                {
                    throw new ArgumentException(string.Format("The direct receipt with id {0} has already been approved and cannot be updated", id));
                }

                goodsReceipt.Comments = comments;
                goodsReceipt.ReferenceNumbers = JsonConvertHelper.SerializeObject(referenceNumbers);
                scope.Context.SaveChanges();
                audit.LogAsync(goodsReceipt, GoodsReceivedEvents.Update, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public void Approve(int id, int userId, string comments)
        {
            using (var scope = Context.Write())
            {
                var goodsReceipt = scope.Context.GoodsReceiptDirect.Single(x => x.Id == id);
                var user = scope.Context.UserPermissionsView.First(u => u.UserId == userId).FullName;

                if (goodsReceipt.DateApproved.HasValue)
                {
                    throw new ArgumentException(string.Format("The goods receipt with id {0} has already been approved", id));
                }
                goodsReceipt.ApprovedBy = user;
                goodsReceipt.ApprovedById = userId;
                goodsReceipt.DateApproved = DateTime.UtcNow;
                goodsReceipt.Comments = comments;

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public List<GoodsReceiptDirectPrintModel> BulkPrint(List<int> ids, bool includeCosts)
        {
            var receipts = new List<GoodsReceiptDirectPrintModel>();
            var auditData = new List<Tuple<object, string>>();
            using (var scope = Context.Write())
            {
                var goodsReceiptPrints = scope.Context.GoodsReceiptDirect.Where(x => ids.Contains(x.Id));
                var products = scope.Context.GoodsReceiptDirectProduct.Where(x => ids.Contains(x.GoodsReceiptDirectId)).ToList();

                foreach (var goodsReceiptPrint in goodsReceiptPrints)
                {
                    var model = Mapper.Map<GoodsReceiptDirectPrintModel>(goodsReceiptPrint);
                    receipts.Add(model);
                    model.Products = Mapper.Map<List<GoodsReceiptDirectProductPrintModel>>(products.Where(x => x.GoodsReceiptDirectId == goodsReceiptPrint.Id)).ToList();
                    bool original = false;

                    if (goodsReceiptPrint.OriginalPrint == null)
                    {
                        original = true;
                        goodsReceiptPrint.OriginalPrint = DateTime.UtcNow;
                    }

                    auditData.Add(
                        new Tuple<object, string>(
                            new { GoodsReceiptId = goodsReceiptPrint.Id, OriginalPrint = original, DirectReceipt = Get(goodsReceiptPrint.Id) },
                            includeCosts ? DirectReceiptEvents.PrintDirectReceiptWithCost : DirectReceiptEvents.PrintDirectReceiptPrintWithoutCost));
                }
                scope.Context.SaveChanges();

                auditData.ForEach(a =>
                {
                    audit.LogAsync(a.Item1, a.Item2, EventCategories.Merchandising);
                });
                scope.Complete();
                return receipts;
            }
        }

        public GoodsReceiptDirectPrintModel Print(int id, bool withCost)
        {
            using (var scope = Context.Write())
            {
                var goodsReceipt = scope.Context.GoodsReceiptDirect.Single(x => x.Id == id);
                var model = Mapper.Map<GoodsReceiptDirectPrintModel>(goodsReceipt);
                model.Products = Mapper.Map<List<GoodsReceiptDirectProductPrintModel>>(scope.Context.GoodsReceiptDirectProduct
                    .Where(x => x.GoodsReceiptDirectId == id)
                    .ToList());
                model.Products.ForEach(p => p.Currency = model.Currency);

                var original = false;
                if (goodsReceipt.OriginalPrint == null)
                {
                    goodsReceipt.OriginalPrint = DateTime.UtcNow;
                    scope.Context.SaveChanges();
                    original = true;
                }

                audit.LogAsync(
                    new { GoodsReceiptId = id, OriginalPrint = original, DirectReceipt = Get(id) },
                    withCost ? DirectReceiptEvents.PrintDirectReceiptWithCost : DirectReceiptEvents.PrintDirectReceiptPrintWithoutCost,
                    EventCategories.Merchandising);
                scope.Complete();
                return model;
            }
        }

        private GoodsReceiptDirectViewModel MapGoodsReceiptViewModel(GoodsReceiptDirect goodsReceipt, List<GoodsReceiptDirectProduct> goodsReceiptProducts, Location location)
        {
            var viewModel = Mapper.Map<GoodsReceiptDirectViewModel>(goodsReceipt);
            viewModel.SalesLocationId = location.SalesId;
            viewModel.Products = goodsReceiptProducts;
            viewModel.ReferenceNumbers =
                JsonConvertHelper.DeserializeObjectOrDefault<List<StringKeyValue>>(goodsReceipt.ReferenceNumbers);
            return viewModel;
        }
    }
}