namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Infrastructure;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public interface IStockCountRepository
    {
        StockCountViewModel Get(int id);

        List<SimpleStockCountViewModel> Get();

        StockCountPrintModel PrintStockCount(int id);

        StockCountPrintModel PrintVariance(int id);

        StockCountProductView GetStockCountProduct(int stockCountId, string sku);

        void Create(StockCountCreateModel model, int userId);

        void Update(List<StockCountProductUpdateModel> model, int userId);

        void Update(List<SimpleStockCountProductViewModel> model, int userId);

        StockCountViewModel Cancel(int id, int userId);

        StockCountViewModel Close(int id, int userId, out string message, bool isForcefullyClose);

        StockCountStartViewModel GetStockCountStart(int stockCountId);

        void CreateStockProducts(int stockCountId, int userId);

        StockCountView GetSchedule(int locationId, DateTime countDate);

        PagedSearchResult<StockCountPreviewView> Preview(StockCountCreateModel model, int pageSize, int pageIndex);

        StockCountSearchModel Search(StockCountSearchQueryModel search, int pageSize, int pageIndex);

        void AutoClose(int cronUserId);

        PagedSearchResult<StockCountProductViewModel> ProductSearch(int id, int? productId, int pageSize, int pageIndex);

        PagedSearchResult<StockCountPreviousItemViewModel> GetPrevious(int productId, int pageSize, int pageIndex);

        List<StockCountProductExportModel> Export(int id);

        bool IsPerpetualStockCount(int stockCountId);
    }

    public class StockCountRepository : IStockCountRepository
    {
        private readonly IEventStore audit;

        private readonly IProductStockRepository productStockRepository;

        private readonly Settings settings;

        private readonly ICostRepository costRepository;

        private readonly IProductRepository productRepository;

        private readonly IStockAdjustmentRepository stockAdjustmentRepository;

        public StockCountRepository(IEventStore audit, IProductStockRepository productStockRepository, Settings settings, ICostRepository costRepository, IProductRepository productRepository, IStockAdjustmentRepository stockAdjustmentRepository)
        {
            this.audit = audit;
            this.productStockRepository = productStockRepository;
            this.settings = settings;
            this.costRepository = costRepository;
            this.productRepository = productRepository;
            this.stockAdjustmentRepository = stockAdjustmentRepository;
        }

        public StockCountViewModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var model = Mapper.Map<StockCountViewModel>(scope.Context.StockCountView.Single(c => c.Id == id));
                var hierarchy = scope.Context.StockCountHierarchyView.Where(s => s.StockCountId == id);
                model.Hierarchy = hierarchy.OrderBy(h => h.HierarchyLevelId)
                    .ToDictionary(h => h.HierarchyLevelId, h => h.Level.ToUpper() + ": " + (string.IsNullOrEmpty(h.Tag) ? "Any" : h.Tag));
                return model;
            }
        }

        public List<SimpleStockCountViewModel> Get()
        {
            using (var scope = Context.Read())
            {
                var now = DateTime.Now.Date;
                var counts = Mapper.Map<List<SimpleStockCountViewModel>>(
                    scope.Context.StockCountView
                    .Where(c => c.ClosedById == null &&
                        c.CancelledById == null &&
                        c.CountDate <= now &&
                        c.StartedById > 0)
                        .ToList());
                var countIds = counts.Select(s => s.Id);
                var prods = Mapper.Map<List<SimpleStockCountProductViewModel>>(scope.Context.StockCountProductView.Where(p => countIds.Any(c => c == p.StockCountId)).ToList());

                counts.ForEach(
                    count =>
                    {
                        count.Products = prods.Where(p => p.StockCountId == count.Id).ToList();
                        count.Products.ForEach(p => p.Count = 0);
                    });

                return counts;
            }
        }

        public StockCountPrintModel PrintStockCount(int id)
        {
            using (var scope = Context.Read())
            {
                var model = Mapper.Map<StockCountPrintModel>(scope.Context.StockCountView.Single(c => c.Id == id));
                var allProducts = scope.Context.StockCountProductView.Where(p => p.StockCountId == id);
                var tags = allProducts.Select(p => p.Hierarchy).Distinct().ToList();
                var hierarchys = tags.Select(t => new StockCountHierarchyPrintModel
                {
                    Hierarchy = t,
                    Products = Mapper.Map<List<StockCountProductViewModel>>(allProducts.Where(p => p.Hierarchy == t).ToList())
                }).ToList();

                model.Hierarchys = hierarchys;
                return model;
            }
        }

        public StockCountPrintModel PrintVariance(int id)
        {
            using (var scope = Context.Read())
            {
                var stockCount = scope.Context.StockCountView.Single(c => c.Id == id);
                var model = Mapper.Map<StockCountPrintModel>(stockCount);
                var allProducts = scope.Context.StockCountProductView.Where(p => p.StockCountId == id && p.Variance != 0);
                var tags = allProducts.Select(p => p.Hierarchy).Distinct().ToList();
                var hierarchys = new List<StockCountHierarchyPrintModel>();

                foreach (string t in tags)
                {
                    hierarchys.Add(new StockCountHierarchyPrintModel
                    {
                        Hierarchy = t,
                        Products = Mapper.Map<List<StockCountProductViewModel>>(allProducts.Where(p => p.Hierarchy == t).ToList())
                    });
                }

                model.Hierarchys = hierarchys;

                // get prices for current otherwise end date
                var date = stockCount.ClosedDate ?? DateTime.UtcNow;
                var costs = costRepository.GetCurrentByProducts(allProducts.Select(p => p.ProductId).ToList(), date);

                foreach (var hierarchy in model.Hierarchys)
                {
                    foreach (var product in hierarchy.Products)
                    {
                        var cost = costs.First(c => c.ProductId == product.ProductId);
                        product.Cost = cost.AverageWeightedCost;
                        product.Currency = cost.SupplierCurrency;
                    }
                }

                return model;
            }
        }

        public StockCountProductView GetStockCountProduct(int stockCountId, string sku)
        {
            using (var scope = Context.Read())
            {
                var model = scope.Context.StockCountProductView.First(p => p.StockCountId == stockCountId && p.Sku == sku);
                return model;
            }
        }

        public void Create(StockCountCreateModel model, int userId)
        {
            using (var scope = Context.Read())
            {
                if (model.Fascia == null)
                {
                    CreateStockCount(model, userId);
                    return;
                }

                var locations = scope.Context.Location.Where(l => l.Fascia == model.Fascia).ToList();

                foreach (Location l in locations)
                {
                    CreateStockCount(
                        new StockCountCreateModel
                        {
                            LocationId = l.Id,
                            Type = model.Type,
                            CountDate = model.CountDate.HasValue ? model.CountDate.Value.ToUniversalTime() : (DateTime?)null,
                            Hierarchy = model.Hierarchy
                        },
                        userId);
                }
            }
        }

        private void CreateStockCount(StockCountCreateModel model, int userId)
        {
            using (var scope = Context.Write())
            {
                var stockCount = Mapper.Map<StockCount>(model);
                stockCount.CreatedDate = DateTime.UtcNow;
                stockCount.CreatedById = userId;
                scope.Context.StockCount.Add(stockCount);
                scope.Context.SaveChanges();

                if (model.Type == "Perpetual")
                {
                    scope.Context.HierarchyLevel.ToList().ForEach(h =>
                    {
                        int? tagId = null;
                        var modelHierarchy = model.Hierarchy.Where(m => m.Key == h.Id).ToList();
                        if (modelHierarchy.Any())
                        {
                            tagId = int.Parse(modelHierarchy.First().Value);
                        }

                        scope.Context.StockCountHierarchy.Add(
                            new StockCountHierarchy
                            {
                                StockCountId = stockCount.Id,
                                HierarchyLevelId = h.Id,
                                HierarchyTagId = tagId
                            });
                    });
                }

                scope.Context.SaveChanges();
                audit.LogAsync(stockCount, StockCountEvents.Create, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public void Update(List<StockCountProductUpdateModel> model, int userId)
        {
            using (var scope = Context.Write())
            {
                if (!model.Any())
                {
                    return;
                }

                var scpIds = model.Select(m => m.Id).ToList();

                var stockCountProducts = scope.Context.StockCountProduct
                    .Where(p => scpIds.Contains(p.Id))
                    .ToList();

                stockCountProducts.Each(p =>
                {
                    Mapper.Map(model.Single(m => m.Id == p.Id), p);
                    p.VerifiedById = userId;
                });

                scope.Context.SaveChanges();

                // Renaming "count" to "stock count" for audit
                audit.LogAsync(
                    model.Select(p => new { p.Id, StockCount = p.Count, p.SystemAdjustment, p.Comments }),
                    StockCountEvents.VerifyProduct,
                    EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public void Update(List<SimpleStockCountProductViewModel> model, int userId)
        {
            using (var scope = Context.Write())
            {
                if (!model.Any())
                {
                    return;
                }

                var now = DateTime.Now.Date;
                var validStockCountIds =
                    scope.Context.StockCount.Where(
                        c => c.ClosedById == null && c.CancelledById == null && c.CountDate <= now && c.StartedById > 0)
                        .Select(a => a.Id);

                if (validStockCountIds.Any())
                {
                    var validModelIds =
                        model.Where(m => validStockCountIds.Contains(m.StockCountId))
                            .Select(m => m.StockCountId + "," + m.Id)
                            .ToList();

                    if (validModelIds.Any())
                    {
                        var prods =
                            scope.Context.StockCountProduct.Where(
                                p => validModelIds.Contains(p.StockCountId + "," + p.Id)).ToList();

                        prods.Each(
                            p =>
                            {
                                // Add the new count on to whatever we've already counted (via other handhelds, web etc)
                                var thisProdCount =
                                    model.Single(m => m.Id == p.Id && m.StockCountId == p.StockCountId);
                                thisProdCount.Count += p.Count ?? 0;
                                Mapper.Map(thisProdCount, p);
                                p.VerifiedById = userId;
                            });

                        scope.Context.SaveChanges();
                        audit.LogAsync(model, StockCountEvents.VerifyProductHandheld, EventCategories.Merchandising);
                    }
                }
                scope.Complete();
            }
        }

        public StockCountViewModel Cancel(int id, int userId)
        {
            StockCountViewModel sc;
            using (var scope = Context.Write())
            {
                var stockAdjustment = scope.Context.StockCount.Single(a => a.Id == id);
                if (stockAdjustment.ClosedDate != null)
                {
                    throw new InvalidOperationException("This stock count has already been closed");
                }
                if (stockAdjustment.CancelledDate != null)
                {
                    throw new InvalidOperationException("This stock count has already been cancelled");
                }
                stockAdjustment.CancelledDate = DateTime.UtcNow;
                stockAdjustment.CancelledById = userId;
                scope.Context.SaveChanges();

                audit.LogAsync(stockAdjustment, StockCountEvents.Cancel, EventCategories.Merchandising);
                sc = Get(id);
                scope.Complete();
                return sc;
            }
        }

        public StockCountViewModel Close(int id, int userId, out string message, bool isForcefullyClose)
        {
            StockCountViewModel sc;
            message = string.Empty;
            using (var scope = Context.Write())
            {
                var stockCount = scope.Context.StockCount.Single(a => a.Id == id);
                if (stockCount.ClosedDate != null)
                {
                    throw new InvalidOperationException("This stock count has already been closed");
                }
                if (stockCount.CancelledDate != null)
                {
                    throw new InvalidOperationException("This stock count has already been cancelled");
                }
                stockCount.ClosedDate = DateTime.UtcNow;
                stockCount.ClosedById = userId;
                audit.LogAsync(stockCount, StockCountEvents.Close, EventCategories.Merchandising);
                scope.Context.SaveChanges();

                if (stockCount.Type == "Quarterly")
                {
                    var adjustmentReason = scope.Context.StockAdjustmentSecondaryReason.First(r => r.DefaultForCountAdjustment);

                    var adjustment = new StockAdjustmentCreateModel
                    {
                        LocationId = stockCount.LocationId,
                        SecondaryReasonId = adjustmentReason.Id,
                        PrimaryReasonId = adjustmentReason.PrimaryReasonId,
                        Comments = string.Format("Automatic adjustment for stock count sc#{0}", stockCount.Id)
                    };

                    // CR : closing Quarterly Stock counts with variance = 0
                    var productCounts = scope.Context.StockCountProduct.Where(p => p.StockCountId == stockCount.Id);
                    List<StockCountProduct> adjustments = new List<StockCountProduct>();
                    adjustments = productCounts.ToList()
                                    .Where(p => p.Count.GetValueOrDefault(0) + p.SystemAdjustment.GetValueOrDefault(0) - p.StartStockOnHand.GetValueOrDefault(0) != 0).ToList();
                    bool isVarianceExist = adjustments.Any(); 

                    if (!isForcefullyClose)
                    {
                        if (!isVarianceExist)
                        {
                            message = "There are no variances for this count. Are you sure you want to close?";
                            return new StockCountViewModel();
                        }
                    }

                    if (isVarianceExist)
                    {
                        var productAdjustments =
                            adjustments.Select(
                                productCount =>
                                new StockAdjustmentProductCreateModel()
                                {
                                    Comments = productCount.Comments,
                                    ProductId = productCount.ProductId,
                                    Quantity = productCount.Count.GetValueOrDefault(0) + productCount.SystemAdjustment.GetValueOrDefault(0) - productCount.StartStockOnHand.GetValueOrDefault(0),
                                }).ToList();

                        adjustment.Products = productAdjustments;

                        var createdAdjustment = stockAdjustmentRepository.Create(adjustment, userId);
                        stockAdjustmentRepository.Approve(createdAdjustment.Id, createdAdjustment.Comments, userId);
                        stockCount.StockAdjustmentId = createdAdjustment.Id;
                    }
                }

                scope.Context.SaveChanges();
                sc = Get(id);
                scope.Complete();
                return sc;
            }
        }

        public StockCountStartViewModel GetStockCountStart(int stockCountId)
        {
            using (var scope = Context.Read())
            {
                var stockCount = scope.Context.StockCount.Single(s => s.Id == stockCountId);
                var hierarchy = scope.Context.StockCountHierarchyView.Where(s => s.StockCountId == stockCountId);
                var location = scope.Context.Location.Single(l => l.Id == stockCount.LocationId);
                if (CheckStockCount(stockCount))
                {
                    return new StockCountStartViewModel()
                    {
                        StockCountId = stockCountId,
                        CountDate = stockCount.CountDate,
                        Questions =
                                       settings.StockCountQuestions.ToDictionary(
                                           s => s,
                                           s => false),
                        Hierarchy = hierarchy.OrderBy(h => h.HierarchyLevelId).ToDictionary(h => h.HierarchyLevelId, h => h.Level.ToUpper() + ": " + (string.IsNullOrEmpty(h.Tag) ? "Any" : h.Tag)),
                        Type = stockCount.Type,
                        Location = location.Name,
                        LocationId = location.Id
                    };
                }
                throw new ArgumentException("Invalid stock count");
            }
        }

        private bool CheckStockCount(StockCount stockCount)
        {
            using (var scope = Context.Read())
            {
                // Check this stock count hasn't already been created
                if (scope.Context.StockCountProduct.Any(s => s.StockCountId == stockCount.Id) ||
                    (stockCount.StartedById.HasValue && stockCount.StartedById > 0))
                {
                    throw new InvalidOperationException("This stock count has already started");
                }

                // Check this hasn't been cancelled
                if (stockCount.CancelledById != null)
                {
                    throw new InvalidOperationException("This stock count has been cancelled");
                }

                if (stockCount.CountDate.Date > DateTime.Now.Date)
                {
                    throw new ArgumentException("This stock count has been scheduled for a future date");
                }
            }

            return true;
        }

        public void CreateStockProducts(int stockCountId, int userId)
        {
            using (var scope = Context.Write())
            {
                var stockCount = scope.Context.StockCount.Single(s => s.Id == stockCountId);
                string productIds = string.Empty;
                if (CheckStockCount(stockCount))
                {
                    //MerchandisingStockCount objMerchandisingStockCount = new MerchandisingStockCount();
                    //productIds = objMerchandisingStockCount.PopulateStockCount(stockCountId, userId);

                    StockCountStartRepository objstockcountstart = new StockCountStartRepository();
                    productIds = objstockcountstart.PopulateStockCount(stockCountId, userId);
                }
                audit.LogAsync(
                        new
                        {
                            StockCountId = stockCountId,
                            //Products = string.Join(",", productIds.ToArray()),
                            Products = productIds,
                            QuestionsConfirmed = settings.StockCountQuestions,
                            User = userId
                        },
                        StockCountEvents.Start,
                        EventCategories.Merchandising);

                scope.Complete();
            }


            /*
            using (var scope = Context.Write())
            {
                var stockCount = scope.Context.StockCount.Single(s => s.Id == stockCountId);
                if (CheckStockCount(stockCount))
                {
                    var hierarchy = scope.Context.StockCountHierarchyView
                        .Where(s => s.StockCountId == stockCountId && s.HierarchyTagId != null).ToList();

                    var query = scope.Context.StockCountPreviewView
                    .Where(p => p.LocationId == stockCount.LocationId);
                    if (hierarchy.Count > 0)
                    {
                        query = CalculateHierarchy(hierarchy.ToDictionary(k => k.HierarchyLevelId, v => v.HierarchyTagId.Value), query);
                    }
                    var productIds = query.Select(p => p.ProductId).Distinct().ToList();
                    var stockLevels = productStockRepository.Get(stockCount.LocationId, hierarchy.Count > 0 ? productIds : null).ToList();

                    var productHierarchy = scope.Context.ProductHierarchyView.Where(p => productIds.Contains(p.ProductId)).ToList();

                    var createdAt = DateTime.UtcNow;
                    stockCount.StartedDate = createdAt;
                    scope.Context.StockCountProduct.AddRange(
                        productIds.Select(
                            p =>
                            new StockCountProduct()
                            {
                                ProductId = p,
                                Count = 0,
                                CreatedDate = createdAt,
                                StartStockOnHand =
                                        stockLevels.Any(s => s.ProductId == p) ? stockLevels.Single(s => s.ProductId == p).StockOnHand : 0,
                                StockCountId = stockCountId,
                                SystemAdjustment = 0,
                                Hierarchy = productHierarchy.OrderByDescending(h => h.LevelId).First(h => h.ProductId == p).Tag
                            }));

                    stockCount.StartedById = userId;
                    stockCount.StartedDate = DateTime.UtcNow;

                    scope.Context.SaveChanges();

                    audit.LogAsync(
                        new
                        {
                            StockCountId = stockCountId,
                            Products = string.Join(",", productIds),
                            QuestionsConfirmed = settings.StockCountQuestions,
                            User = userId
                        },
                        StockCountEvents.Start,
                        EventCategories.Merchandising);
                }
                scope.Complete();
            }
            */

        }

        public StockCountView GetSchedule(int locationId, DateTime countDate)
        {
            using (var scope = Context.Read())
            {
                var date = countDate.Date;
                return scope.Context.StockCountView.FirstOrDefault(s => s.LocationId == locationId && s.CountDate == date && s.Status != "Cancelled");
            }
        }

        public PagedSearchResult<StockCountPreviewView> Preview(StockCountCreateModel model, int pageSize, int pageIndex)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context
                    .StockCountPreviewView
                    .Where(p => p.LocationId == model.LocationId);

                var results = CalculateHierarchy(model.Hierarchy.ToDictionary(k => k.Key, v => int.Parse(v.Value)), query).ToList();

                // select the most specific hierachy level for each product
                results = results
                    .GroupBy(p => p.ProductId)
                    .Select(g => g.OrderByDescending(p => p.HierarchyLevelId).FirstOrDefault())
                    .OrderBy(p => p.HierarchyTagId)
                    .ThenBy(p => p.ProductId).ToList();

                var count = results.Count();
                var page = results.Skip(pageSize * pageIndex).Take(pageSize).ToList();

                return new PagedSearchResult<StockCountPreviewView> { Count = count, Page = page };
            }
        }

        private IQueryable<StockCountPreviewView> CalculateHierarchy(Dictionary<int, int> hierarchySource, IQueryable<StockCountPreviewView> query)
        {
            var hierarchy = hierarchySource.Select(h => new { LevelId = h.Key, TagId = h.Value }).ToList();

            var prodIds = query.GroupBy(q => q.ProductId).ToList().Where(p => !hierarchy.Any() ||
                    hierarchy.All(l => p.Any(a => a.HierarchyLevelId == l.LevelId && a.HierarchyTagId == l.TagId))).Select(p => p.Key);

            return query.Where(p => prodIds.Contains(p.ProductId));
        }

        public StockCountSearchModel Search(StockCountSearchQueryModel search, int pageSize, int pageIndex)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.StockCountView.Where(v =>
                    (search.MinStockCountId == null || v.Id >= search.MinStockCountId)
                    && (search.MaxStockCountId == null || v.Id <= search.MaxStockCountId)
                    && (search.MinScheduledDate == null || v.CountDate >= search.MinScheduledDate)
                    && (search.MaxScheduledDate == null || v.CountDate <= search.MaxScheduledDate)
                    && (search.Status == null || v.Status == search.Status)
                    && (search.Type == null || v.Type == search.Type)
                    && (search.LocationId == null || v.LocationId == search.LocationId));

                if (search.MinStartedDate.HasValue)
                {
                    var fromUTC = search.MinStartedDate.Value.ToUniversalTime();
                    query = query.Where(x => x.StartedDate >= fromUTC);
                }

                if (search.MaxStartedDate.HasValue)
                {
                    var toUTC = search.MaxStartedDate.Value.ToUniversalTime().AddDays(1);
                    query = query.Where(x => x.StartedDate < toUTC);
                }

                var total = query.Count();
                var page = query.OrderByDescending(v => v.Id).Skip(pageSize * pageIndex).Take(pageSize).ToList();

                var results = Mapper.Map<List<StockCountSearchResultModel>>(page);
                var model = new StockCountSearchModel { Results = results, TotalResults = total };
                return model;
            }
        }

        public void AutoClose(int cronUserId)
        {
            var today = DateTime.Now.Date;
            var closeBeforeDate = today.AddDays(0 - new Settings().DaysUntilAutoCloseStockCount);
            string message = string.Empty;
            using (var scope = Context.Write())
            {
                var oldStockCounts = scope.Context.StockCount.Where(c => c.StartedDate <= closeBeforeDate && !c.CancelledById.HasValue && !c.ClosedById.HasValue).ToList();
                foreach (var c in oldStockCounts)
                {
                    Close(c.Id, cronUserId, out message, false);
                }
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public PagedSearchResult<StockCountProductViewModel> ProductSearch(int id, int? productId, int pageSize, int pageIndex)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.StockCountProductView.Where(s =>
                        s.StockCountId == id &&
                        (s.ProductId == productId || productId == null));

                var count = query.Count();
                query = query.OrderBy(v => v.Hierarchy).ThenBy(v => v.ProductId).Skip(pageSize * pageIndex);

                List<StockCountProductView> products;
                if (pageSize > 0)
                {
                    products = query.Take(pageSize).ToList();
                }
                else
                {
                    products = query.ToList();
                }

                var page = Mapper.Map<List<StockCountProductViewModel>>(products);
                var model = new PagedSearchResult<StockCountProductViewModel> { Count = count, Page = page };

                return model;
            }
        }

        public List<StockCountProductExportModel> Export(int id)
        {
            using (var scope = Context.Read())
            {
                return Mapper.Map<List<StockCountProductExportModel>>(scope.Context.StockCountProductView.Where(s => s.StockCountId == id));
            }
        }

        public PagedSearchResult<StockCountPreviousItemViewModel> GetPrevious(int productId, int pageSize, int pageIndex)
        {
            using (var scope = Context.Read())
            {
                var previousCounts = scope.Context
                    .StockCountProductView
                    .Where(p => p.ProductId == productId && p.ClosedDate.HasValue);

                var results = previousCounts
                    .OrderBy(p => p.CountDate)
                    .Skip(pageIndex * pageSize).Take(pageSize)
                    .ToList();

                return new PagedSearchResult<StockCountPreviousItemViewModel> { Count = previousCounts.Count(), Page = Mapper.Map<List<StockCountPreviousItemViewModel>>(results) };
            }
        }

        public bool IsPerpetualStockCount(int stockCountId)
        {
            bool isPerpetualType;
            using (var scope = Context.Write())
            {
                isPerpetualType = scope.Context.StockCount.Any(x => x.Id == stockCountId && x.Type == "Perpetual");
                scope.Complete();
            }
            return isPerpetualType;
        }
    }
}