namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Blue.Cosacs.Event;
    using Blue.Cosacs.Merchandising.Calculations;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Infrastructure;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Solr;
    using Blue.Events;
    using EntityFramework.BulkInsert.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core;
    using System.Linq;
    using System.Text.RegularExpressions;
    using AssociatedProduct = Blue.Cosacs.Merchandising.AssociatedProduct;
    using Context = Blue.Cosacs.Merchandising.Context;
    using System.Data;
    using System.Data.SqlClient;

    public interface IProductRepository
    {
        IList<Product> GetByType(string productType);

        IDictionary<int, List<string>> GetStoreTypes(List<int> ids);

        Product Get(int id);

        List<Product> GetBasic(List<int> queryProductIds);

        ProductDescriptor GetDescriptor(int id);

        List<ProductMessageView> GetProductMessages(List<int> ids);

        Product GetBySku(string sku);

        IEnumerable<Product> GetBySku(IEnumerable<string> sku);

        List<Product> SearchBySku(IEnumerable<string> skus, IEnumerable<string> tags = null);

        RepossessionDetailsModel GetRepossessionDetails(int? id);

        List<Models.AssociatedProduct> GetAssociatedProducts(string hierarchy = null);
        List<Models.AssociatedProduct> GetAssociatedProductsForExport();

        bool ProductAssociationExists(string sku, string serializedHierarchy);

        int GetStatusUpdate(int id);

        List<ProductStockLevel> StockLevels(List<int> validProductIds, int locationId);

        Models.AssociatedProduct AssociateProduct(string sku, string serializedHierarchy);

        void CancelStock(IEnumerable<int> purchaseOrdersIds, bool isDirect = false);

        void ReceiveAndCancelStock(IEnumerable<GoodsReceiptProduct> goodsReceiptProducts, bool isDirect = false);

        void ReceiveAndCancelStock(IEnumerable<StockChange> stockCancellations, bool isDirect = false);

        void ReturnStock(
            int id,
            int goodsReceiptReceivingLocationId,
            int quantityReceived,
            int quantityReturned,
            decimal landedCost);

        void ReturnStock(List<VendorReturnProductView> vendorReturnProducts);

        void ReturnStock(List<VendorReturnDirectProductView> vendorReturnProducts);

        void AdjustStock(List<StockAdjustmentModel> adjustments);

        string GetProductType(int id);

        ProductRepository.ProductLocation LocateResource(string sku);

        decimal? GetFirstYearWarranty(int productId);

        string GetProductHierarchyJson(int id);

        bool SkuExists(string sku);

        Product Save(Product model, int userId);

        Product Save(Merchandising.Product model, int userId);

        Dictionary<string, int> Save(IEnumerable<Models.Product> prod);

        void SaveHierarchySetting(int id, string level, string tag);
        string IsValidSkuNonStock(int? id, string sku);

        bool IsValidSku(int? id, string sku);

        void SaveStoreTypes(int id, List<string> storeTypes);

        void DeleteAssociation(int id);

        void SaveProductTags(int id, List<string> tags);

        void AddSupplier(int id, int supplierId);

        void RemoveSupplier(int id, int supplierId);

        List<string> CreateRepossessedProducts(int id, int userId);

        int GetAvailability(int productId, int sendingLocationId);

        List<StockLunrModel> GetStockProductsWithMatchingStoreType(string q, int locationId, int storeTypeLocationId);

        List<StockLunrModel> GetStockProducts(string q, int locationId);

        List<BasicProductDetails> GetAllStockProducts(string q);

        void AdjustStock(List<StockMovementModel> stockMovements);

        List<Models.ProductSKU> Get();

        void UnAge(IEnumerable<int> productIds);
        string GetAshleyEnable();
    }

    public class ProductRepository : IProductRepository
    {
        private const string SolrType = "MerchandiseStock";
        private const string ProductsSolrType = "MerchandiseStockSummary";

        public class ProductLocation
        {
            public int Id { get; set; }
            public string ProductType { get; set; }
            public string PreviousProductType { get; set; }
        }

        private readonly IEventStore audit;
        private readonly ILog logger;
        private readonly IRepossessedConditionsRepository conditionsRepository;
        private readonly IStockSolrIndexer stockSolrIndexer;
        private readonly ILocationRepository locationRepository;
        private readonly IPromotionRepository promotionRepository;
        private readonly IProductStatusProgresser productStatusProgresser;
        private readonly Merchandising.Settings merchandisingSettings;
        private readonly IBrandRepository brandRepository;
        private readonly ISupplierRepository supplierRepository;

        public ProductRepository(
                                 ILog logger,
                                 IEventStore audit,
                                 IRepossessedConditionsRepository conditionsRepository,
                                 IProductStatusProgresser productStatusProgresser,
                                 IStockSolrIndexer stockSolrIndexer,
                                 ILocationRepository locationRepository,
                                 IPromotionRepository promotionRepository,
                                 Merchandising.Settings merchandisingSettings,
                                 IBrandRepository brandRepository,
                                 ISupplierRepository supplierRepository)
        {
            this.logger = logger;
            this.audit = audit;
            this.conditionsRepository = conditionsRepository;
            this.productStatusProgresser = productStatusProgresser;
            this.stockSolrIndexer = stockSolrIndexer;
            this.locationRepository = locationRepository;
            this.promotionRepository = promotionRepository;
            this.merchandisingSettings = merchandisingSettings;
            this.brandRepository = brandRepository;
            this.supplierRepository = supplierRepository;
        }

        public List<StockLunrModel> GetStockProducts(string q, int locationId)
        {
            var filter = string.Format(
                  "Type:{0} AND "
                + "LocationId:{1} AND "
                + "!ProductType:Combo AND "
                + "!ProductType:Set AND "
                + "!ProductType:ProductWithoutStock AND "
                + "!ProductStatus:\"Non Active\"",
                SolrType,
                locationId);

            return new Lunr().Search<StockLunrModel>(q, filter);
        }

        public List<BasicProductDetails> GetAllStockProducts(string q)
        {
            var filter = string.Format(
                                       "Type:{0} AND "
                                     + "!ProductType:Combo AND "
                                     + "!ProductType:Set AND "
                                     + "!ProductType:ProductWithoutStock AND "
                                     + "!ProductStatus:\"Non Active\"",
                                     ProductsSolrType);

            return new Lunr().Search<BasicProductDetails>(q, filter);
        }

        public List<StockLunrModel> GetStockProductsWithMatchingStoreType(string q, int locationId, int storeTypeLocationId)
        {
            var results = this.GetStockProducts(q, locationId);

            var location = locationRepository.Get(storeTypeLocationId);
            var products = GetBasic(results.Select(r => r.ProductId).ToList());

            return results.Where(
                r =>
                {
                    var product = products.Single(prod => prod.Id == r.ProductId);

                    if (product.StoreTypes == null)
                    {
                        return true;
                    }
                    return !product.StoreTypes.Any() || product.StoreTypes.Contains(location.StoreType);
                }).ToList();
        }

        public int GetAvailability(int productId, int sendingLocationId)
        {
            using (var scope = Context.Read())
            {
                var stock = scope.Context.ProductStockLevel.SingleOrDefault(
                    s => s.LocationId == sendingLocationId && s.ProductId == productId);
                return stock == null ? 0 : stock.StockAvailable;
            }
        }

        public IList<Product> GetByType(string productType)
        {
            using (var scope = Context.Read())
            {
                var products = scope.Context.Product.Where(p => p.ProductType == productType);
                var result = Mapper.Map<IList<Product>>(products);

                foreach (var prod in result)
                {
                    prod.Hierarchy =
                        scope.Context.ProductHierarchyView.Where(h => h.ProductId == prod.Id)
                            .ToDictionary(h => h.Level, h => h.Tag);
                }
                return result;
            }
        }

        public IDictionary<int, List<string>> GetStoreTypes(List<int> ids)
        {
            using (var scope = Context.Read())
            {
                var products = scope.Context.Product.Where(p => ids.Contains(p.Id));

                return products.ToDictionary(p => p.Id, p => JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(p.StoreTypes));
            }
        }

        public Product Get(int id)
        {
            using (var scope = Context.Read())
            {
                var product = scope.Context.Product.Single(p => p.Id == id);
                var model = Mapper.Map<Product>(product);

                model.Hierarchy = scope.Context.ProductHierarchyView.Where(h => h.ProductId == product.Id).ToDictionary(h => h.Level.ToLower(), h => h.Tag);
                model.Suppliers = scope.Context.ProductSupplierView.Where(p => p.ProductId == product.Id).ToList().Select(p => new KeyValuePair<int, string>(p.SupplierId, p.SupplierName)).ToList();
                model.TaxRates = Mapper.Map<IEnumerable<TaxRateModel>>(scope.Context.TaxRate.Where(t => t.ProductId == product.Id));
                return model;
            }
        }

        public List<Product> GetBasic(List<int> productIds)
        {
            using (var scope = Context.Read())
            {
                var products = scope.Context.Product.Where(p => productIds.Contains(p.Id));

                var model = Mapper.Map<List<Product>>(products);
                return model;
            }
        }

        public ProductDescriptor GetDescriptor(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Product
                    .Where(p => p.Id == id)
                    .Project()
                    .To<ProductDescriptor>()
                    .Single();
            }
        }

        public List<ProductMessageView> GetProductMessages(List<int> ProductIds)
        {
            using (var scope = Context.Read())
            {
                //CR : closing Quarterly Stock counts with variance = 0
                var dtProductIds = new DataTable();
                dtProductIds.Columns.Add("ProductIds", typeof(int));
                ProductIds.Distinct().Each(r => dtProductIds.Rows.Add(r));
                return scope.Context.GetProductMessagesByProductId(dtProductIds);
            }
        }

        public Product GetBySku(string sku)
        {
            using (var scope = Context.Read())
            {
                var product = scope.Context.Product.FirstOrDefault(p => p.SKU == sku);
                if (product == null)
                {
                    return null;
                }
                var model = Mapper.Map<Product>(product);
                model.Hierarchy = scope.Context.ProductHierarchyView
                    .Where(h => h.ProductId == product.Id)
                    .ToDictionary(h => h.Level.ToLower(), h => h.Tag);

                return model;
            }
        }

        public IEnumerable<Product> GetBySku(IEnumerable<string> sku)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Product.Where(p => sku.Contains(p.SKU)).ToList().Select(s => Mapper.Map<Product>(s)).ToList();
            }
        }

        public List<Product> SearchBySku(IEnumerable<string> skus, IEnumerable<string> tags = null)
        {
            using (var scope = Context.Read())
            {
                var products = scope.Context.Product.Where(p => skus.Contains(p.SKU));
                if (tags != null)
                {
                    products = products.Where(p => tags.Contains(p.Tags));
                }
                return Mapper.Map<List<Product>>(products.ToList());
            }
        }

        public RepossessionDetailsModel GetRepossessionDetails(int? id)
        {
            using (var scope = Context.Read())
            {
                var repossession = scope.Context.RepossessedProduct.SingleOrDefault(p => p.Id == id);
                if (repossession == null || id == null)
                {
                    return new RepossessionDetailsModel();
                }

                var condition = conditionsRepository.Get(repossession.RepossessedConditionId);
                //var originalProduct = Get(id.Value);
                var model = new RepossessionDetailsModel
                {
                    Condition = condition.Name
                };
                return model;
            }
        }

        public List<Models.AssociatedProduct> GetAssociatedProducts(string hierarchy = null)
        {
            using (var scope = Context.Read())
            {
                var associatedProducts =
                    scope.Context.AssociatedProduct.Where(a => string.IsNullOrEmpty(hierarchy) ||
                        a.AssociatedHierarchy == hierarchy).ToList();
                var apids = associatedProducts.Select(a => a.ProductId).ToList();
                var adbProducts = scope.Context.Product.Where(p => apids.Any(ap => ap == p.Id)).ToList();

                return (from assocProd in associatedProducts
                        let prod = adbProducts.FirstOrDefault(p => assocProd.ProductId == p.Id)
                        where prod != null
                        select
                            new Models.AssociatedProduct
                            {
                                Id = assocProd.Id,
                                LongDescription = prod.LongDescription,
                                PosDescription = prod.POSDescription,
                                SKU = prod.SKU,
                                StatusValid = prod.Status != (int)ProductStatuses.NonActive && prod.Status != (int)ProductStatuses.Deleted && prod.Status != (int)ProductStatuses.Discontinued,
                                Hierarchy =
                                        JsonConvertHelper
                                        .DeserializeObjectOrDefault<Dictionary<string, string>>(
                                            assocProd.AssociatedHierarchy)
                            }).ToList();
            }
        }

        public List<Models.AssociatedProduct> GetAssociatedProductsForExport()
        {
            var associatedProductsList = new List<AssociatedProduct>();

            using (var scope = Context.Read())
            {
                var associatedProducts =
                    scope.Context.AssociatedProduct.ToList();
                var apids = associatedProducts.Select(a => a.ProductId).ToList();
                var adbProducts = scope.Context.Product.Where(p => apids.Any(ap => ap == p.Id)).ToList();

                var associatedProd1 = (
                    from assocProd in associatedProducts
                    let prod = adbProducts.FirstOrDefault(p => assocProd.ProductId == p.Id)
                    where prod != null
                    select
                        new Models.AssociatedProduct
                        {
                            Id = assocProd.Id,
                            LongDescription = prod.LongDescription,
                            PosDescription = prod.POSDescription,
                            SKU = prod.SKU,
                            StatusValid = prod.Status != (int)ProductStatuses.NonActive && prod.Status != (int)ProductStatuses.Deleted && prod.Status != (int)ProductStatuses.Discontinued,
                            HierarchyClass =
                                    JsonConvertHelper
                                    .DeserializeObjectOrDefault<Models.AssociatedProductHierarchy>(
                                        assocProd.AssociatedHierarchy)
                        }).ToList();

                var result = (from ap in associatedProd1
                              from apv in scope.Context.AssociatedProductsHierarchyView
                              where ((ap.HierarchyClass.Class == apv.Class && ap.HierarchyClass.Class != string.Empty)
                                      || (ap.HierarchyClass.Department == apv.Department && ap.HierarchyClass.Department != string.Empty && ap.HierarchyClass.Class == string.Empty)
                                      || (ap.HierarchyClass.Division == apv.Division && ap.HierarchyClass.Division != string.Empty
                                      && ap.HierarchyClass.Department == string.Empty && ap.HierarchyClass.Class == string.Empty)
                                      || (ap.HierarchyClass.Class == string.Empty && ap.HierarchyClass.Department == string.Empty && ap.HierarchyClass.Division == string.Empty))
                              select
                                 new Models.AssociatedProduct
                                 {
                                     Id = ap.Id,
                                     LongDescription = ap.LongDescription,
                                     PosDescription = ap.PosDescription,
                                     SKU = ap.SKU,
                                     StatusValid = ap.StatusValid,
                                     HierarchyClass = new AssociatedProductHierarchy
                                     {
                                         Class = apv.Class
                                     }
                                 }
                                 ).GroupBy(t => new
                                 {
                                     t.HierarchyClass.Class,
                                     t.SKU
                                 }).Select(o => o.FirstOrDefault()).ToList();
                return result;
            }
        }

        public bool ProductAssociationExists(string sku, string serializedHierarchy)
        {
            using (var scope = Context.Read())
            {
                var product = scope.Context.Product.Single(p => p.SKU == sku);

                return scope.Context.AssociatedProduct.Any(ap => ap.ProductId == product.Id && ap.AssociatedHierarchy == serializedHierarchy);
            }
        }

        public int GetStatusUpdate(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Product.Where(p => p.Id == id).Select(p => p.Status).Single();
            }
        }

        public List<ProductStockLevel> StockLevels(List<int> validProductIds, int locationId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ProductStockLevel.Where(l => validProductIds.Contains(l.ProductId) && l.LocationId == locationId).ToList();
            }
        }

        public string GetProductHierarchyJson(int id)
        {
            using (var scope = Context.Read())
            {
                var result = scope.Context.ProductHierarchyConcatView.SingleOrDefault(p => p.ProductId == id);

                return result != null ? result.Hierarchy : string.Empty;
            }
        }
        public Models.AssociatedProduct AssociateProduct(string sku, string serializedHierarchy)
        {
            using (var scope = Context.Write())
            {
                var product = scope.Context.Product.FirstOrDefault(p => p.SKU == sku);
                if (product != null)
                {
                    var association =
                        scope.Context.AssociatedProduct.FirstOrDefault(
                            p => p.AssociatedHierarchy == serializedHierarchy && p.ProductId == product.Id);

                    if (association == null)
                    {
                        association = new AssociatedProduct()
                        {
                            AssociatedHierarchy = serializedHierarchy,
                            ProductId = product.Id
                        };
                        scope.Context.AssociatedProduct.Add(association);
                    }

                    scope.Context.SaveChanges();
                    scope.Complete();

                    this.audit.LogAsync(product, ProductEvents.EditProductAssociation, EventCategories.Merchandising);

                    return new Models.AssociatedProduct
                    {
                        Id = association.Id,
                        LongDescription = product.LongDescription,
                        PosDescription = product.POSDescription,
                        StatusValid = product.Status != (int)ProductStatuses.NonActive && product.Status != (int)ProductStatuses.Deleted && product.Status != (int)ProductStatuses.Discontinued,
                        SKU = product.SKU,
                        HierarchyClass =
                                       JsonConvertHelper
                                       .DeserializeObjectOrDefault<Models.AssociatedProductHierarchy>(
                                           serializedHierarchy)
                    };
                }
                else
                {
                    logger.Exception(string.Format("Associate Product - product not found - '{0}'", sku));
                }
                return null;
            }
        }

        private int GetCancelled(PurchaseOrderProductStatsView pop, int orderedQty)
        {
            if (pop == null)
            {
                return orderedQty;
            }
            return pop.QuantityPending.HasValue && pop.QuantityPending > 0 ? pop.QuantityPending.Value : orderedQty;
        }

        public void CancelStock(IEnumerable<int> purchaseOrdersIds, bool isDirect = false) // All stock for po
        {
            using (var scope = Context.Write())
            {
                var pos = scope.Context.StockChangeView.Where(s => purchaseOrdersIds.Contains(s.PurchaseOrderId))
                           .Select(s => new StockChange
                           {
                               ProductId = s.ProductId,
                               Location = s.LocationId,
                               QuantityReceived = 0,   // Should have already adjusted when if gr'd.
                               QuantityCancelled = s.QuantityCancelled.Value // Always not null. Mapping bug in template.
                           }).ToList();
                ReceiveAndCancelStock(pos, isDirect);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void ReceiveAndCancelStock(IEnumerable<GoodsReceiptProduct> goodsReceiptProducts, bool isDirect = false)
        {
            using (var scope = Context.Write())
            {
                var grpIds = goodsReceiptProducts.Select(g => g.GoodsReceiptId).ToList();

                var pos = scope.Context.StockChangeView.Where(s => s.GoodsRecieptId != null && grpIds.Contains(s.GoodsRecieptId.Value))
                           .Select(s => new StockChange
                           {
                               ProductId = s.ProductId,
                               Location = s.LocationId,
                               QuantityReceived = s.QuantityReceived,
                               QuantityCancelled = s.QuantityCancelled.Value
                           }).ToList();
                ReceiveAndCancelStock(pos);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void ReceiveAndCancelStock(IEnumerable<StockChange> stockChanges, bool isDirect = false)
        {
            using (var scope = Context.Write())
            {
                // Preload costprice.
                var ids = stockChanges.Select(s => s.ProductId).ToList();
                var costPrice = (from p in scope.Context.CostPrice
                                 where ids.Contains(p.ProductId)
                                 group p by p.ProductId into g
                                 select new
                                 {
                                     Id = g.Key,
                                     values = g.OrderByDescending(c => c.AverageWeightedCostUpdated)
                                          .ThenByDescending(c => c.LastLandedCostUpdated).FirstOrDefault()
                                 })
                                .ToDictionary(d => d.Id, d => d.values);

                // Load matching stockloctions.
                var productStockLevel = scope.Context.ProductStockLevel.Where(l => ids.Contains(l.ProductId)).ToList();
                var productStockGroup = productStockLevel.GroupBy(l => new Tuple<int, int>(l.ProductId, l.LocationId)).ToDictionary(d => d.Key, d => d.Sum(x => x.StockOnHand));
                // Generate cost price.
                var onHand = productStockLevel.GroupBy(p => p.ProductId)
                                                .ToDictionary(d => d.Key, d => d.Sum(p => p.StockOnHand));

                var costPrices = stockChanges.Where(p => p.QuantityReceived + p.QuantityCancelled > 0 && p.QuantityReceived > 0 && costPrice.ContainsKey(p.ProductId))
                    .Select(p =>
                      new CostPrice
                      {
                          ProductId = p.ProductId,
                          AverageWeightedCost = AWC.CalculateAWC(
                                                                 costPrice[p.ProductId].AverageWeightedCost,
                                                                 costPrice[p.ProductId].LastLandedCost,
                                                                 onHand[p.ProductId],
                                                                 p.QuantityReceived),
                          LastLandedCost = costPrice[p.ProductId].LastLandedCost,
                          LastLandedCostUpdated = costPrice[p.ProductId].LastLandedCostUpdated,
                          SupplierCost = costPrice[p.ProductId].SupplierCost,
                          SupplierCurrency = costPrice[p.ProductId].SupplierCurrency,
                          AverageWeightedCostUpdated = DateTime.UtcNow
                      });
                scope.Context.CostPrice.AddRange(costPrices);

                // Update stock quantities at locations.
                var posGroups = stockChanges.GroupBy(g => new Tuple<int, int>(g.ProductId, g.Location))
                                .Select(s =>
                                    new
                                    {
                                        s.Key,
                                        QuantityReceived = s.Sum(q => q.QuantityReceived),
                                        QuantityDiff = s.Sum(q => q.QuantityReceived + q.QuantityCancelled),
                                    }).ToDictionary(d => d.Key);

                productStockLevel.ForEach(p =>
                {
                    var key = new Tuple<int, int>(p.ProductId, p.LocationId);
                    if (posGroups.ContainsKey(key))
                    {
                        p.StockOnHand += posGroups[key].QuantityReceived;
                        p.StockAvailable += posGroups[key].QuantityReceived;
                    }
                });

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void ReturnStock(
            int id,
            int goodsReceiptReceivingLocationId,
            int quantityReceived,
            int quantityReturned,
            decimal landedCost)
        {
            if (quantityReceived > 0 && quantityReturned > 0)
            {
                using (var scope = Context.Write())
                {
                    quantityReturned = 0 - quantityReturned;
                    var productStockLevels = scope.Context.ProductStockLevel.Where(s => s.ProductId == id).ToList();

                    var actualReceivingStockLevel = GetProductStockLevel(productStockLevels, id, goodsReceiptReceivingLocationId);
                    var costPrice = scope.Context.CurrentCostPriceView.Where(cp => cp.ProductId == id).OrderByDescending(c => c.AverageWeightedCostUpdated).First();

                    var newOnHand = productStockLevels.Sum(l => l.StockOnHand) + quantityReturned;
                    CostPrice cost;

                    if (newOnHand > 0)
                    {
                        var newAWC = AWC.CalculateAWC(costPrice.AverageWeightedCost, landedCost, productStockLevels.Sum(l => l.StockOnHand), quantityReturned);
                        cost = new CostPrice { ProductId = id, AverageWeightedCost = newAWC, LastLandedCost = costPrice.LastLandedCost, SupplierCost = costPrice.SupplierCost, SupplierCurrency = costPrice.SupplierCurrency, LastLandedCostUpdated = costPrice.LastLandedCostUpdated, AverageWeightedCostUpdated = DateTime.UtcNow };
                        scope.Context.CostPrice.Add(cost);
                        scope.Context.SaveChanges();
                    }
                    actualReceivingStockLevel.StockOnHand += quantityReturned;
                    actualReceivingStockLevel.StockAvailable += quantityReturned;

                    scope.Context.SaveChanges();
                    this.stockSolrIndexer.Index(new[] { id });
                    scope.Complete();
                }
            }
        }

        public void ReturnStock(List<VendorReturnProductView> vendorReturnProducts)
        {
            using (var scope = Context.Read())
            {
                foreach (var returnProduct in vendorReturnProducts)
                {
                    ReturnStock(
                        returnProduct.ProductId,
                        returnProduct.GoodsReceiptLocationId,
                        returnProduct.QuantityReceived,
                        returnProduct.QuantityReturned,
                        returnProduct.LastLandedCost);
                }
            }
        }

        public void ReturnStock(List<VendorReturnDirectProductView> vendorReturnProducts)
        {
            foreach (var returnProduct in vendorReturnProducts)
            {
                ReturnStock(
                    returnProduct.ProductId,
                    returnProduct.GoodsReceiptLocationId,
                    returnProduct.QuantityReceived,
                    returnProduct.QuantityReturned,
                    returnProduct.LastLandedCost);
            }
        }

        public void AdjustStock(List<StockAdjustmentModel> adjustments)
        {
            adjustments = adjustments.Where(a => a.Quantity != 0).ToList();
            var productIds = adjustments.Select(a => a.ProductId).ToList();

            using (var scope = Context.Write())
            {
                var stockLevels = scope.Context.ProductStockLevel
                    .Where(l => productIds.Contains(l.ProductId))
                    .ToList();

                adjustments.ForEach(a =>
                {
                    var productLevel = GetProductStockLevel(stockLevels, a.ProductId, a.LocationId);
                    productLevel.StockOnHand += a.Quantity;
                    productLevel.StockAvailable += a.Quantity;
                });

                scope.Context.SaveChanges();

                stockSolrIndexer.Index(productIds);

                scope.Complete();
            }
        }

        public void AdjustStock(List<StockMovementModel> movements)
        {
            var adjustments = movements.Select(x => new StockAdjustmentModel
            {
                ProductId = x.ProductId,
                LocationId = x.SendingLocationId,
                Quantity = x.Quantity * -1,
            }).ToList();

            adjustments.AddRange(movements.Select(x => new StockAdjustmentModel
            {
                ProductId = x.ProductId,
                LocationId = x.ReceivingLocationId,
                Quantity = x.Quantity,
            }));

            AdjustStock(adjustments);
        }

        private ProductStockLevel GetProductStockLevel(List<ProductStockLevel> levels, int productId, int locationId)
        {
            var level = levels.SingleOrDefault(s => s.LocationId == locationId && s.ProductId == productId);
            if (level != null)
            {
                return level;
            }
            using (var scope = Context.Write())
            {
                level = new ProductStockLevel
                {
                    ProductId = productId,
                    LocationId = locationId,
                };
                scope.Context.ProductStockLevel.Add(level);
                levels.Add(level);
                scope.Context.SaveChanges();
                scope.Complete();
            }

            return level;
        }

        public string GetProductType(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Product.Where(p => p.Id == id).Select(p => p.ProductType).Single();
            }
        }

        public ProductLocation LocateResource(string sku)
        {
            using (var scope = Context.Read())
            {
                return scope.Context
                    .Product
                    .Where(p => p.SKU == sku)
                    .Select(p => new ProductLocation { Id = p.Id, ProductType = p.ProductType })
                    .FirstOrDefault();
            }
        }

        public decimal? GetFirstYearWarranty(int productId)
        {
            using (var scope = Context.Read())
            {
                var fyw =
                    scope.Context.ProductHierarchyView.Where(v => v.ProductId == productId && v.FirstYearWarrantyProvision != null)
                        .OrderByDescending(v => v.LevelId)
                        .FirstOrDefault();

                return fyw != null ? fyw.FirstYearWarrantyProvision : null;
            }
        }

        public bool SkuExists(string sku)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Product.Any(p => p.SKU == sku);
            }
        }

        public Product Save(Product model, int userId)
        {
            const string NonAlphaNum = @"[^A-Za-z0-9_]+";
            model.Attributes.ForEach(a => a.Name = Regex.Replace(a.Name, NonAlphaNum, string.Empty));
            model.Features.ForEach(a => a.Name = Regex.Replace(a.Name, NonAlphaNum, string.Empty));

            var product = Mapper.Map<Merchandising.Product>(model);

            var validMagento = new List<string> { MagentoExportTypes.All, MagentoExportTypes.NotAvailable, MagentoExportTypes.Warehouse };

            if (validMagento.Contains(model.MagentoExportType))
            {
                if ((model.MagentoExportType == MagentoExportTypes.All || model.MagentoExportType == MagentoExportTypes.Warehouse) && !product.OnlineDateAdded.HasValue)
                {
                    product.OnlineDateAdded = DateTime.UtcNow;
                }
            }
            else
            {
                product.MagentoExportType = MagentoExportTypes.NotAvailable;
                product.OnlineDateAdded = null;
            }

            return this.Save(product, userId);
        }

        public Product Save(Merchandising.Product model, int userId)
        {
            Models.Product prod;

            using (var scope = Context.Write())
            {
                var product = scope.Context.Product.Find(model.Id);
                string eventType;

                if (product == null)
                {
                    product = Mapper.Map<Merchandising.Product>(model);
                    product.LastStatusChangeDate = DateTime.UtcNow;
                    product.CreatedDate = model.CreatedDate == new DateTime() ? DateTime.UtcNow : model.CreatedDate;
                    product.CreatedById = userId;
                    product.StoreTypes = null;
                    product.Tags = null;

                    scope.Context.Product.Add(product);
                    eventType = ProductEvents.CreateProduct;
                }
                else
                {
                    model.CreatedDate = product.CreatedDate;

                    if (product.OnlineDateAdded.HasValue && !model.OnlineDateAdded.HasValue)
                    {
                        model.OnlineDateAdded = null;
                    }

                    if (product.OnlineDateAdded.HasValue && model.OnlineDateAdded.HasValue)
                    {
                        // is already set so dont reset the date
                        model.OnlineDateAdded = product.OnlineDateAdded;
                    }

                    var lastStatusChange = product.LastStatusChangeDate;

                    if (model.Status != product.Status)
                    {
                        lastStatusChange = DateTime.UtcNow;
                    }

                    // cant change sku on update
                    model.SKU = product.SKU;
                    Mapper.Map(model, product);
                    product.LastStatusChangeDate = lastStatusChange;
                    eventType = ProductEvents.EditProduct;


                }

                product.LastUpdatedDate = DateTime.UtcNow;

                productStatusProgresser.AutoProgress(product);
                scope.Context.SaveChanges();

                this.audit.LogAsync(
                    new
                    {
                        product.Id,
                        product.SKU,
                        product.LongDescription,

                        product.Tags,
                        product.StoreTypes,
                        product.POSDescription,
                        product.Attributes,
                        product.CreatedDate,
                        product.LastUpdatedDate,
                        Status = ((ProductStatuses)product.Status).ToString(),
                        PreviousProductType= product.PreviousProductType,
                        ProductType = product.ProductType,
                        product.PriceTicket,
                        product.SKUStatus,
                        product.CorporateUPC,
                        product.VendorUPC,
                        product.VendorStyleLong,
                        product.CountryOfOrigin,
                        product.VendorWarranty,
                        product.ReplacingTo,
                        product.Features,
                        BrandName = product.BrandId.HasValue ? brandRepository.Get(product.BrandId.Value).BrandName : string.Empty,
                        PrimaryVendorName = product.PrimaryVendorId.HasValue ? supplierRepository.Get(product.PrimaryVendorId.Value).Name : string.Empty,
                        product.LastStatusChangeDate,
                        product.OnlineDateAdded
                    },
                    eventType,
                    EventCategories.Merchandising);

                prod = Mapper.Map<Product>(Get(product.Id));
                scope.Context.CreateNewProductStockLevels();
                scope.Context.CreateHistoryForGRNandPO(product);
                scope.Complete();
            }
            return prod;
        }

        public Dictionary<string, int> Save(IEnumerable<Models.Product> prod)
        {
            var map = new Dictionary<string, int>();
            using (var scope = Context.Write())
            {
                scope.Context.BulkInsert(prod.Select(s => new ProductStaging(s)));
                scope.Context.SaveChanges();

                scope.Context.BulkProductSave();
                map = scope.Context.Product.Select(p => new { p.SKU, p.Id }).ToDictionary(d => d.SKU, d => d.Id);
                scope.Context.SaveChanges();
                scope.Complete();
            }

            return map;
        }

        public void SaveHierarchySetting(int id, string level, string tag)
        {
            using (var scope = Context.Write())
            {
                var dblevel = scope.Context.HierarchyLevel.Single(t => t.Name == level);
                var dbtag = scope.Context.HierarchyTag.SingleOrDefault(t => t.Name == tag && t.LevelId == dblevel.Id);
                var hierarchyLevelSetting = scope.Context.ProductHierarchy.SingleOrDefault(h => h.ProductId == id && h.HierarchyLevelId == dblevel.Id);

                if (dbtag == null && hierarchyLevelSetting != null)
                {
                    scope.Context.ProductHierarchy.Remove(hierarchyLevelSetting);
                }
                else if (hierarchyLevelSetting == null)
                {
                    hierarchyLevelSetting = new ProductHierarchy { ProductId = id, HierarchyLevelId = dblevel.Id, HierarchyTagId = dbtag.Id };
                    scope.Context.ProductHierarchy.Add(hierarchyLevelSetting);
                }
                else
                {
                    hierarchyLevelSetting.HierarchyTagId = dbtag.Id;
                }

                var product = scope.Context.Product.FirstOrDefault(p => p.Id == id);
                scope.Context.SaveChanges();

                productStatusProgresser.AutoProgress(product);

                var sku = product.SKU;

                scope.Context.SaveChanges();

                this.audit.LogAsync(new { SKU = sku, Level = level, Tag = tag }, ProductEvents.EditProductHierarchy, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public bool IsValidSku(int? id, string sku)
        {
            using (var scope = Context.Read())
            {
                return !scope.Context.Product.Any(p => p.SKU == sku && (!id.HasValue || p.Id != id));
            }
        }

        public void SaveStoreTypes(int id, List<string> storeTypes)
        {
            using (var scope = Context.Write())
            {
                var product = scope.Context.Product.Find(id);

                if (product == null)
                {
                    throw new ObjectNotFoundException();
                }

                product.StoreTypes = JsonConvertHelper.Serialize(storeTypes);

                scope.Context.SaveChanges();

                this.audit.LogAsync(new { product.SKU, product.StoreTypes }, ProductEvents.EditProductStoreTypes, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public void DeleteAssociation(int id)
        {
            using (var scope = Context.Write())
            {
                var associatedProduct = scope.Context.AssociatedProduct.Find(id);

                if (associatedProduct == null)
                {
                    throw new ObjectNotFoundException();
                }

                scope.Context.AssociatedProduct.Remove(associatedProduct);

                scope.Context.SaveChanges();

                this.audit.LogAsync(associatedProduct, ProductEvents.DeleteProductAssociation, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public void SaveProductTags(int id, List<string> tags)
        {
            using (var scope = Context.Write())
            {
                var product = scope.Context.Product.Find(id);

                if (product == null)
                {
                    throw new ObjectNotFoundException();
                }

                product.Tags = JsonConvertHelper.Serialize(tags);

                scope.Context.SaveChanges();

                this.audit.LogAsync(new { product.SKU, product.Tags }, ProductEvents.EditProductTags, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public void AddSupplier(int id, int supplierId)
        {
            using (var scope = Context.Write())
            {
                var product = scope.Context.Product.Single(p => p.Id == id);
                var prodSupplier = scope.Context.ProductSupplier.FirstOrDefault(p => p.ProductId == id && p.SupplierId == supplierId);
                if (prodSupplier == null)
                {
                    scope.Context.ProductSupplier.Add(
                        new ProductSupplier()
                        {
                            ProductId = id,
                            SupplierId = supplierId
                        });

                    scope.Context.SaveChanges();

                    productStatusProgresser.AutoProgress(product);
                    scope.Context.SaveChanges();
                }

                var supplier = scope.Context.Supplier.Single(s => s.Id == supplierId);
                this.audit.LogAsync(new { ProductSKU = product.SKU, VendorName = supplier.Name, Action = "Add" }, ProductEvents.EditProductVendor, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public void RemoveSupplier(int id, int supplierId)
        {
            using (var scope = Context.Write())
            {
                var prodSupplier = scope.Context.ProductSupplier.FirstOrDefault(p => p.ProductId == id && p.SupplierId == supplierId);
                if (prodSupplier != null)
                {
                    scope.Context.ProductSupplier.Remove(prodSupplier);
                    scope.Context.SaveChanges();
                }

                var supplier = scope.Context.Supplier.Single(s => s.Id == supplierId);
                var product = scope.Context.Product.Single(p => p.Id == id);

                this.audit.LogAsync(new { ProductSKU = product.SKU, VendorName = supplier.Name, Action = "Remove" }, ProductEvents.EditProductVendor, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public List<string> CreateRepossessedProducts(int id, int userId)
        {
            using (var scope = Context.Write())
            {
                var product = scope.Context.Product.Single(p => p.Id == id);
                if (product.ProductType != ProductTypes.RegularStock)
                {
                    throw new InvalidOperationException();
                }
                var tax = scope.Context.TaxRate.Where(t => t.ProductId == id).ToList();
                var existingConditions = scope.Context.RepossessedProduct.Where(p => p.OriginalProductId == id).Select(p => p.RepossessedConditionId).ToList();
                var conditions = conditionsRepository.Get();
                var conditionsToCreate = conditions.Where(c => !existingConditions.Contains(c.Id)).ToList();
                var newProducts = new List<Merchandising.Product>();

                foreach (var condition in conditionsToCreate)
                {
                    var newProduct = new Merchandising.Product();
                    Mapper.Map(product, newProduct);
                    newProduct.Id = 0;
                    newProduct.SKU += condition.SKUSuffix;
                    newProduct.ProductType = ProductTypes.RepossessedStock;
                    newProduct.PriceTicket = false;
                    newProduct.Status = (int)ProductStatuses.NonActive;
                    newProduct.OnlineDateAdded = null;
                    newProduct.MagentoExportType = "Not Available Online";
                    newProducts.Add(newProduct);

                    // copy product
                    var savedProduct = Save(newProduct, userId);
                    newProduct.Id = savedProduct.Id.Value;

                    var repossessedProduct = new RepossessedProduct
                    {
                        Id = newProduct.Id,
                        OriginalProductId = product.Id,
                        RepossessedConditionId = condition.Id,
                    };

                    // create repossessed product
                    scope.Context.RepossessedProduct.Add(repossessedProduct);

                    // copy tax
                    var clonedTax = tax.Select(t => new TaxRate
                    {
                        ProductId = newProduct.Id,
                        Name = t.Name,
                        Rate = t.Rate,
                        EffectiveDate = t.EffectiveDate
                    });
                    scope.Context.TaxRate.AddRange(clonedTax);

                    // create cost
                    var cost = new CostPrice
                    {
                        ProductId = newProduct.Id,
                        AverageWeightedCost = 0.0M,
                        LastLandedCost = 0.0M,
                        SupplierCost = 0.0M,
                        SupplierCurrency = merchandisingSettings.LocalCurrency,
                        LastLandedCostUpdated = DateTime.UtcNow,
                        AverageWeightedCostUpdated = DateTime.UtcNow
                    };

                    scope.Context.CostPrice.Add(cost);
                }
                scope.Context.SaveChanges();
                this.audit.LogAsync(new { Sku = product.SKU, Action = "Create Repossessed Products" }, ProductEvents.CreateProduct, EventCategories.Merchandising);
                this.stockSolrIndexer.Index(newProducts.Select(p => p.Id).ToArray());
                scope.Complete();
                return newProducts.Select(p => p.SKU).ToList();
            }
        }

        public List<Models.ProductSKU> Get()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Product.Select(p => new Models.ProductSKU()
                {
                    SkuDescription = p.LongDescription,
                    SkuId = p.SKU
                }).ToList();
            }
        }

        public void UnAge(IEnumerable<int> productIds)
        {
            using (var scope = Context.Write())
            {
                var newstock = scope.Context.Product.Where(p => productIds.Contains(p.Id) && p.Status == (int)ProductStatuses.Aged).ToList();
                newstock.ForEach(n =>
                {
                    n.Status = (int)ProductStatuses.ActiveCurrent;
                });
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }


        //Setting for Enable/Disable Ashley CR related Features
        public string GetAshleyEnable()
        {
            using (var scope = Context.Read())
            {
                Settings s = new Settings();
                bool ashleyEnabled = s.AshleyEnabled;
                return "{\"status\":\"success\",\"data\": \"" + ashleyEnabled + "\"}";
            }
        }



        #region to Restrict Code for to save duplicate and nonstock SKU in product Table by ST
        public string IsValidSkuNonStock(int? id, string sku)
        {
            string res = string.Empty;
            SqlParameter[] parameters = new SqlParameter[2];

            using (var scope = Context.Write())
            {
                var paramId = new SqlParameter("@ProductId", SqlDbType.Int);
                paramId.Value = id == null ? 0 : id;

                var paramSKU = new SqlParameter("@SKU", SqlDbType.VarChar, 18);
                paramSKU.Value = sku;
                res = scope.Context.Database.SqlQuery<string>("EXEC [Merchandising].[ValidateProductSKU] @ProductId, @SKU", paramId, paramSKU).Single();
            }

            return res;
        }
        #endregion



    }
}