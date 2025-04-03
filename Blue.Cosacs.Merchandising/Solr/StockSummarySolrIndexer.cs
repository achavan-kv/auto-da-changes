namespace Blue.Cosacs.Merchandising.Solr
{
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Repositories;
    using Blue.Solr;
    using Blue.Transactions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IStockSummarySolrIndexer
    {
        Task Index(IEnumerable<int> productIds = null);
        Task ReIndex(bool updateOnly);
    }

    public class StockSummarySolrIndexer : IStockSummarySolrIndexer
    {
        private readonly IProductSalesRepository productSalesRepository;
        private readonly IPromotionRepository promotionRepository;
        private readonly Settings settings;
        private readonly Config.Settings config;
        private readonly ICostRepository cost;
        private readonly ILocationRepository locationRepository;

        private const string SolrType = "MerchandiseStockSummary";

        public StockSummarySolrIndexer(IProductSalesRepository productSalesRepository,
            IPromotionRepository promotionRepository,
            Settings settings,
            Config.Settings config,
            ICostRepository cost,
            ILocationRepository locationRepository)
        {
            this.productSalesRepository = productSalesRepository;
            this.promotionRepository = promotionRepository;
            this.settings = settings;
            this.config = config;
            this.cost = cost;
            this.locationRepository = locationRepository;
        }

        private async Task<T> ExecuteAsync<T>(Func<T> toExecute) where T : class
        {
            return await Task<T>.Run<T>(toExecute).ConfigureAwait(false);
        }

        public Task ReIndex(bool updateOnly)
        {
            ReIndexingProduct objReindexingProduct = new ReIndexingProduct();
            List<string> defaultValue = null;
            List<ReIndexedStockSummaryModel> finalResult = new List<ReIndexedStockSummaryModel>();
            try
            {
                var reIndexedProduct = ExecuteAsync<DataTable>(() => objReindexingProduct.GetReIndexedProducts(updateOnly));
                finalResult = (from DataRow dr in reIndexedProduct.Result.AsEnumerable()
                               select new ReIndexedStockSummaryModel()
                               {
                                   Id = string.Format("{0}:{1}", SolrType, Convert.ToInt32(dr.Field<int>("ProductId"))),
                                   ProductId = Convert.ToInt32(dr.Field<int>("ProductId")),
                                   Sku = Convert.ToString(dr.Field<string>("SKU")),
                                   LongDescription = Convert.ToString(dr.Field<string>("LongDescription")),
                                   PosDescription = Convert.ToString(dr.Field<string>("POSDescription")),
                                   ProductType = Convert.ToString(dr.Field<string>("ProductType")),
                                   PreviousProductType = dr.Field<string>("PreviousProductType"),
                                   ProductStatus = Convert.ToString(dr.Field<string>("Status")),
                                   RepossessedCondition = Convert.ToString(dr.Field<string>("Condition")),
                                   Type = SolrType,
                                   PriceData = dr.IsNull("PriceData") ? defaultValue : (dr.Field<string>("PriceData").Contains("]") ? dr.Field<string>("PriceData").Split(']').ToList() : new List<string> { dr.Field<string>("PriceData") }),
                                   PromoData = dr.IsNull("PromoData") ? defaultValue : (dr.Field<string>("PromoData").Contains("]") ? dr.Field<string>("PromoData").Split(']').ToList() : new List<string> { dr.Field<string>("PromoData") }),
                                   CreatedOn = dr.IsNull("CreatedDate") ? Convert.ToDateTime("01/01/1990").ToSolrDate() : dr.Field<DateTime>("CreatedDate").ToSolrDate(),
                                   StockAvailable = Convert.ToInt32(dr.Field<int?>("StockAvailable")),
                                   StockOnHand = Convert.ToInt32(dr.Field<int?>("StockOnHand")),
                                   StockOnOrder = Convert.ToInt32(dr.Field<int?>("StockOnOrder")),
                                   StockAllocated = Convert.ToInt32(dr.Field<int?>("StockAllocated")),
                                   LabelRequired = Convert.ToBoolean(dr.Field<Boolean>("LabelRequired")),
                                   BranchesWithStock = Convert.ToInt32(dr.Field<int?>("BranchesWithStock")),
                                   Tags = dr.IsNull("Tags") ? defaultValue : JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(dr.Field<string>("Tags")),
                                   HierarchyTags = dr.Field<string>("LevelTags"),
                                   StoreTypes = dr.IsNull("StoreTypes") ? defaultValue : JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(dr.Field<string>("StoreTypes")),
                                   Vendors = dr.IsNull("venders") ? defaultValue : JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(dr.Field<string>("venders")),
                                   SalesLastPeriod = dr.IsNull("SalesLastPeriod") ? 0 : Convert.ToInt32(dr.Field<int>("SalesLastPeriod")),
                                   SalesThisYTD = dr.IsNull("SalesThisYTD") ? 0 : Convert.ToInt32(dr.Field<int>("SalesThisYTD")),
                                   SalesLastYTD = dr.IsNull("SalesLastYTD") ? 0 : Convert.ToInt32(dr.Field<int>("SalesLastYTD")),
                                   SalesThisPeriod = dr.IsNull("SalesThisPeriod") ? 0 : Convert.ToInt32(dr.Field<int>("SalesThisPeriod")),
                                   CorporateUPC = dr.Field<string>("CorporateUPC"),
                                   VendorUPC = dr.Field<string>("VendorUPC"),
                                   MerchandisingLevel_1 = dr.Field<string>("MerchandisingLevel_1"),
                                   MerchandisingLevel_2 = dr.Field<string>("MerchandisingLevel_2"),
                                   MerchandisingLevel_3 = dr.Field<string>("MerchandisingLevel_3")
                               }).ToList();
                return Task.Run(() =>
                new WebClient().Update(finalResult)
            );
            }
            catch (Exception ex)
            {
                throw new Exception("See inner exception details.", ex);
            }
        }

        public Task Index(IEnumerable<int> productIds = null)
        {
            using (var scope = Context.Read())
            {
                var products = GetProducts(productIds);

                var stockLevelsTask = ExecuteAsync<List<LocationStockLevelView>>(() => GetStockLevels(productIds, scope));
                var costsTask = ExecuteAsync<List<CurrentCostPriceView>>(() => GetCosts(productIds, scope));
                var pricesTask = ExecuteAsync<List<CurrentRetailPriceView>>(() => GetPrices(productIds, scope));
                var salesTask = ExecuteAsync<List<ProductSalesViewModel>>(() => productSalesRepository.Get(productIds, scope, true));

                Task.WaitAll(new Task[] { stockLevelsTask, costsTask, pricesTask, salesTask });

                var stockLevels = stockLevelsTask.Result;
                var costs = costsTask.Result.ToLookup(x => x.ProductId);
                var prices = pricesTask.Result.ToLookup(x => x.ProductId);

                var allStoreTypes = locationRepository.GetStoreTypes();


                var sales = ((from s in salesTask.Result
                              from sale in s.Sales
                              where sale.Type == "Sales (Delivered)"
                              select new { id = s.ProductId, sales = sale }).GroupBy(g => g.id)
                                                                           .Select(s => new { productid = s.Key, sales = s.FirstOrDefault().sales }))
                                                                           .ToDictionary(d => d.productid, d => d.sales);

                var documents = products.Select(p =>
                {
                    var vendors = JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(p.Suppliers);
                    vendors.Add(p.PrimaryVendor);

                    return CreateDocument(p, prices, costs, stockLevels, vendors, sales, allStoreTypes);
                });

                return Task.Run(() =>
                    new WebClient().Update(documents)
                );
            }
        }

        private dynamic CreateDocument(ForceMerchandiseProductEnquiryIndexView p,
                                       ILookup<int, CurrentRetailPriceView> productPrices,
                                       ILookup<int, CurrentCostPriceView> productCosts,
                                       List<LocationStockLevelView> stockLevels,
                                       List<string> vendors,
                                       IDictionary<int, ProductSalesModel> productSales,
                                       IEnumerable<string> allStoreTypes)
        {
            dynamic document = new ExpandoObject();
            document.Id = string.Format("{0}:{1}", SolrType, p.ProductId);
            document.ProductId = p.ProductId;
            document.Sku = p.SKU;
            document.LongDescription = p.LongDescription;
            document.PosDescription = p.POSDescription;
            document.ProductType = p.ProductType;
            document.PreviousProductType = p.PreviousProductType;
            document.ProductStatus = p.Status;
            document.RepossessedCondition = p.Condition;
            document.Type = SolrType;
            document.PriceData = this.PriceData(p.ProductId, p.ProductType, productPrices[p.ProductId], productCosts[p.ProductId].FirstOrDefault() ?? new CurrentCostPriceView { AverageWeightedCost = 0 });
            document.PromoData = this.PromoData(productPrices[p.ProductId], p);
            document.CreatedOn = p.CreatedDate.ToSolrDate();
            document.StockAvailable = p.StockAvailable;
            document.StockOnHand = p.StockOnHand;
            document.StockOnOrder = p.StockOnOrder;
            document.StockAllocated = p.StockAllocated;
            document.LabelRequired = p.LabelRequired;
            document.BranchesWithStock = CountOfBranchesWithStock(stockLevels, p.ProductId);
            document.Tags = JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(p.Tags);
            document.HierarchyTags = p.LevelTags;
            document.StoreTypes = p.StoreTypes == null ? allStoreTypes : JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(p.StoreTypes);
            document.Vendors = vendors.ToArray();
            document.SalesThisPeriod = productSales.ContainsKey(p.ProductId) ? productSales[p.ProductId].ThisPeriod : 0;
            document.SalesLastPeriod = productSales.ContainsKey(p.ProductId) ? productSales[p.ProductId].LastPeriod : 0;
            document.SalesThisYTD = productSales.ContainsKey(p.ProductId) ? productSales[p.ProductId].ThisYTD : 0;
            document.SalesLastYTD = productSales.ContainsKey(p.ProductId) ? productSales[p.ProductId].LastYTD : 0;
            document.CorporateUPC = p.CorporateUPC;
            document.VendorUPC = p.VendorUPC;

            AddHierarchyLevels(document, p);

            return document;
        }

        private void AddHierarchyLevels(dynamic document, ForceMerchandiseProductEnquiryIndexView p)
        {
            IDictionary<string, object> dic = document;
            var hierarchyLevels = JsonConvertHelper.DeserializeObjectOrDefault<List<HierarchyLevel>>(p.Hierarchy);
            hierarchyLevels.ForEach(l => dic.Add("MerchandisingLevel_" + l.Id, l.Name ?? "Unassigned"));
        }

        private IEnumerable<string> PriceData(int productId, string productType, IEnumerable<CurrentRetailPriceView> productPrices, CurrentCostPriceView productCosts)
        {

            Func<decimal?, decimal?, decimal?> calcMargin = (cashPrice, awc) =>
            {
                if (!cashPrice.HasValue)  // No cashPrice no margin.
                {
                    return null;
                }

                if (cashPrice > 0 && (!awc.HasValue || awc.Value == 0)) // 100 % as no awc.
                {
                    return 1;
                }

                if (cashPrice.Value == 0) // Avoid div by 0 and set to 0. -- Leona 4/3/16
                {
                    return 0;
                }
                return (cashPrice.Value - awc.Value) / cashPrice.Value;
            };

            return productPrices.Select(x => JsonConvertHelper.SerializeObject(new
            {
                x.LocationId,
                LocationName = x.Name,
                x.Fascia,
                RegularPrice = this.ApplyTax(x.RegularPrice, x.TaxRate),
                CashPrice = this.ApplyTax(x.CashPrice, x.TaxRate),
                DutyFreePrice = this.ApplyTax(x.DutyFreePrice, x.TaxRate),
                productCosts.AverageWeightedCost,
                Margin = calcMargin(x.CashPrice, productCosts.AverageWeightedCost),
                x.EffectiveDate,
                x.TaxRate
            }));
        }

        private IEnumerable<string> PromoData(IEnumerable<CurrentRetailPriceView> productPrices, ForceMerchandiseProductEnquiryIndexView product)
        {
            var retailPrices = productPrices.Select(x => new RetailPriceViewModel
            {
                CashPrice = x.CashPrice.HasValue ? x.CashPrice.Value : 0,
                DutyFreePrice = x.DutyFreePrice.HasValue ? x.DutyFreePrice.Value : 0,
                //   EffectiveDate = x.DutyFreePrice.HasValue ? x.DutyFreePrice.Value : 0, Not needed.
                Fascia = x.Fascia,
                Id = x.Id,
                Location = x.Name,
                LocationId = x.LocationId,
                ProductId = x.ProductId,
                RegularPrice = x.RegularPrice.HasValue ? x.RegularPrice.Value : 0,
                TaxRate = x.TaxRate.HasValue ? x.TaxRate.Value : 0
            }).ToList();

            var promoPrices = promotionRepository.GetForProduct(product.ProductId, product.SKU, product.Hierarchy, retailPrices);

            return promoPrices.Select(x =>
            {
                return JsonConvertHelper.SerializeObject(new
                {
                    x.LocationId,
                    LocationName = x.Location,
                    x.Fascia,
                    CashPrice = FormatPromoPrice(ApplyTax(x.CashPrice, x.TaxRate)),
                    RegularPrice = FormatPromoPrice(ApplyTax(x.RegularPrice, x.TaxRate)),
                    DutyFreePrice = FormatPromoPrice(ApplyTax(x.DutyFreePrice, x.TaxRate)),
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    x.EffectiveDate,
                    x.TaxRate
                });
            }).Where(x => x != null);
        }

        private string FormatPromoPrice(decimal? price)
        {
            if (price == 0 || price == null)
            {
                return "None";
            }
            return decimal.Round(price.Value, config.DecimalPlaces).ToString("F" + config.DecimalPlaces);
        }

        private decimal ApplyTax(decimal? price, decimal? taxRate)
        {
            var p = price ?? 0;
            var t = taxRate ?? 0;
            //return settings.TaxInclusive ? p * (1 + t) : p;
            return p;
        }

        private static int CountOfBranchesWithStock(List<LocationStockLevelView> stockLevels, int productId)
        {
            return stockLevels.Count(l => l.ProductId == productId);
        }

        private List<ForceMerchandiseProductEnquiryIndexView> GetProducts(IEnumerable<int> productIds)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.ForceMerchandiseProductEnquiryIndexView.AsNoTracking().AsQueryable();
                if (productIds != null)
                {
                    query = query.Where(s => productIds.Contains(s.ProductId));
                }
                return query.ToList();
            }
        }

        private List<LocationStockLevelView> GetStockLevels(IEnumerable<int> productIds, ReadScope<Context> scope)
        {
            var query = scope.Context.LocationStockLevelView.AsNoTracking().Where(s => s.StockOnHand != 0 && s.VirtualWarehouse == false);
            if (productIds != null)
            {
                query = query.Where(s => productIds.Contains(s.ProductId));
            }
            return query.ToList();
        }

        private List<CurrentCostPriceView> GetCosts(IEnumerable<int> productIds, ReadScope<Context> scope)
        {
            var query = scope.Context.CurrentCostPriceView.AsNoTracking().AsQueryable();
            if (productIds != null)
            {
                query = query.Where(s => productIds.Contains(s.ProductId));
            }
            return query.ToList();
        }

        private List<CurrentRetailPriceView> GetPrices(IEnumerable<int> productIds, ReadScope<Context> scope)
        {
            var query = scope.Context.CurrentRetailPriceView.AsNoTracking().AsQueryable();
            if (productIds != null)
            {
                query = query.Where(s => productIds.Contains(s.ProductId));
            }
            return query.ToList();
        }
    }
}