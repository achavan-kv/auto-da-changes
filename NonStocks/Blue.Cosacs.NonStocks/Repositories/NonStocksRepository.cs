using Blue.Cosacs.NonStocks.Export;
using Blue.Cosacs.NonStocks.ExternalHttpService;
using Blue.Cosacs.NonStocks.Models;
using Blue.Cosacs.NonStocks.Models.WinCosacs;
using Blue.Events;
using Blue.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Blue.Cosacs.NonStocks
{
    public class NonStocksRepository : INonStocksRepository
    {
        private readonly IClock clock = null;
        private readonly ICourtsNetWS courtsNetWS = null;
        private readonly IHttpClientJson httpClientJson = null;
        private readonly IPriceRepository priceRepository = null;
        private readonly IProductLinkRepository productLinkRepository = null;
        private readonly IEventStore audit;

        public NonStocksRepository(IClock clock, ICourtsNetWS courtsNetWS, IHttpClientJson httpClientJson,
            IPriceRepository priceRepository, IProductLinkRepository productLinkRepository, IEventStore audit)
        {
            this.clock = clock;
            this.courtsNetWS = courtsNetWS;
            this.httpClientJson = httpClientJson;
            this.priceRepository = priceRepository;
            this.productLinkRepository = productLinkRepository;
            this.audit = audit;
        }

        public Models.NonStockModel Load(int id)
        {
            using (var scope = Context.Read())
            {
                var nonStock = Load(
                    new int[] { id }
                    )
                    .FirstOrDefault();

                var productLink = (from pl in scope.Context.LinkNonStock
                                   where pl.NonStockId == id
                                   select pl).FirstOrDefault();

                nonStock.HasProductLink = productLink != null ? true : false;

                return nonStock;
            }
        }

        public NonStock Load(string sku)
        {
            using (var scope = Context.Read())
            {
                var nonStock = (from n in scope.Context.NonStock
                                where n.SKU == sku
                                select n).FirstOrDefault();

                return nonStock;
            }
        }

        public List<Models.NonStockModel> Load(int[] ids)
        {
            using (var scope = Context.Read())
            {
                var retVals = new List<Models.NonStockModel>();

                var nss = (from n in scope.Context.NonStock
                           where ids.Contains(n.Id)
                           select n
                          )
                          .ToLookup(e => e.Id);

                foreach (var id in ids)
                {
                    var hVals = (from h in scope.Context.NonStockHierarchy
                                 where h.NonStockId == id
                                 select h
                                 )
                                 .OrderBy(e => e.Level)
                                 .ToList();

                    retVals.Add(Models.NonStockModel.ToModel(nss[id].FirstOrDefault(), hVals));
                }

                return retVals;
            }
        }

        public List<Models.NonStockModel> LoadAll()
        {
            using (var scope = Context.Read())
            {
                var retVals = new List<Models.NonStockModel>();

                var nss = (from n in scope.Context.NonStock
                           select new Models.NonStockModel()
                           {
                               Id = n.Id,
                               Type = n.Type,
                               SKU = n.SKU,
                               VendorUPC = n.VendorUPC,
                               ShortDescription = n.ShortDescription,
                               LongDescription = n.LongDescription,
                               Active = n.Active,
                               TaxRate = n.TaxRate
                           })
                          .ToList();

                return nss;
            }
        }


        public int SaveNonStockDetails(NonStock nonStock, List<NonStockHierarchy> hierarchy)
        {
            if (DuplicateInItemMaster(nonStock.SKU))
            {
                throw new OperationCanceledException("Duplicate SKU.");
            }
            using (var scope = Context.Write())
            {
                if (nonStock.Id > 0)
                {
                    var ns = (from n in scope.Context.NonStock
                              where n.Id == nonStock.Id
                              select n)
                              .FirstOrDefault();

                    // apply changes
                    ns.Type = nonStock.Type;
                    ns.SKU = nonStock.SKU;
                    ns.VendorUPC = nonStock.VendorUPC;
                    ns.ShortDescription = nonStock.ShortDescription;
                    ns.LongDescription = nonStock.LongDescription;
                    ns.Length = nonStock.Length;
                    ns.DiscountRecurringPeriod = nonStock.DiscountRecurringPeriod;
                    ns.IsStaffDiscount = nonStock.IsStaffDiscount;
                    ns.Active = nonStock.Active;
                    ns.TaxRate = nonStock.TaxRate;
                    ns.IsFullRefund = nonStock.IsFullRefund;
                    ns.CoverageValue = nonStock.CoverageValue;
                }
                else
                {
                    var duplicateVendorUPC = (from n in scope.Context.NonStock
                                              where n.VendorUPC == nonStock.VendorUPC
                                              select n).FirstOrDefault();

                    if (duplicateVendorUPC != null && duplicateVendorUPC.VendorUPC != string.Empty)
                    {
                        throw new OperationCanceledException("Duplicate VendorUPC.");
                    }
                    else
                    {
                        scope.Context.NonStock.Add(nonStock);
                    }
                }

                scope.Context.SaveChanges();
                SaveNonStockHierarchy(nonStock.Id, hierarchy);

                scope.Complete();

                return nonStock.Id;
            }
        }

        public List<NonStockModel> GetActiveNonStocks(IList<string> types)
        {
            using (var scope = Context.Read())
            {
                var retVals = new List<NonStockModel>();

                var values = scope.Context.NonStock
                    .Select(p=> p);

                if(types != null && types.Any())
                {
                    values = values
                        .Where(p => types.Contains(p.Type));
                }

                foreach (var nonStock in values)
                {
                    var hVals = (from h in scope.Context.NonStockHierarchy
                                 where h.NonStockId == nonStock.Id
                                 select h
                                 )
                                 .OrderBy(e => e.Level)
                                 .ToList();

                    retVals.Add(NonStockModel.ToModel(nonStock, hVals));
                }

                return retVals;
            }
        }

        public string ExportProductsFile(string user)
        {
            var exportRetString = string.Empty;
            var fileBuilder = new StringBuilder();

            var winCosacsBranchLookup = CosacsBranchInfo.Get()
                .ToLookup(e => e.BranchNumber);

            var exporter = new Fact2000Interface();
            using (var scope = Context.Read())
            {
                var countryTaxType = GetTaxInfo(user).TaxType;

                var dateForExport = clock.Now.Hour < 12 ? clock.Now.Date : clock.Now.AddDays(1).Date; //If after 12PM then include to export those with Effect Date of the following day

                var nonStocksModels = LoadAll();

                foreach (var nsm in nonStocksModels)
                {
                    var id = nsm.Id;
                    var hVals = scope.Context.NonStockHierarchy
                                .Select(p => p)
                                .Where(p => p.NonStockId == id)
                                .OrderBy(e => e.Level)
                                .ToList();
                                

                    nsm.Hierarchy = NonStockModel.ToModel(nsm.ToEntity(), hVals).Hierarchy;

                    var prices = priceRepository.GetPrices(id)
                        .Where(e => e.EffectiveDate <= dateForExport) // check if within effective date period
                        .ToList();

                    if (nsm.Type == NonStockTypes.Discount)
                    {
                        // Still not sure what to do if there are multiple generic price details for discounts
                        prices = prices
                            .Where(e =>
                                e.NonStockType == NonStockTypes.Discount &&
                                (e.EndDate >= clock.Now.Date || e.EndDate.HasValue == false)) //Migrated discounts did not have an end date
                            .ToList();
                    }

                    var pricesExport = new List<NonStockPriceModel>();
                    if (prices.Count > 0)
                    {
                        pricesExport = ProcessPricesForExport(prices, winCosacsBranchLookup);
                    }

                    pricesExport.Sort((i, j) =>
                    {
                        var num1 = i.WarehouseNumber.HasValue ? i.WarehouseNumber.Value : 0;
                        var num2 = j.WarehouseNumber.HasValue ? j.WarehouseNumber.Value : 0;
                        return num1 - num2;
                    });

                    foreach (var price in pricesExport)
                    {
                        if (price.BranchNumber.HasValue)
                        {
                            var branchNumber = price.BranchNumber.Value;

                            var warehouseNumber = winCosacsBranchLookup[branchNumber]
                                .FirstOrDefault()
                                .WarehouseNumber;

                            var category = nsm.Level3 ?? new NonStockModel.NonStockHierarchyModel();
                            var barcode = nsm.VendorUPC ?? string.Empty;

                            if (barcode != null && barcode.Length > 13)
                            {
                                barcode = barcode.Substring(0, 13);
                            }

                            var retailPrice = price.RetailPrice.HasValue ? price.RetailPrice.Value : 0.00m;
                            if (countryTaxType == "I")
                            {
                                retailPrice = price.TaxInclusivePrice.HasValue ? price.TaxInclusivePrice.Value : 0.00m;
                            }

                            var costPrice = price.CostPrice.HasValue ? price.CostPrice.Value : 0.00m;

                            if (nsm.Type == NonStockTypes.Discount)
                            {
                                retailPrice = 0;
                                if (price.DiscountValue.HasValue)
                                {
                                    var discountValue = price.DiscountValue.Value;
                                    if (discountValue > 0)
                                    {
                                        retailPrice = discountValue * -1;
                                    }
                                }
                            }

                            var countryTaxRate = GetTaxInfo(user).TaxRate;

                            var pafLine = new ProductAmendmentFile()
                            {
                                WarehouseNumber = (short)warehouseNumber,
                                ProductCode = nsm.SKU,
                                SupplierProductDescription = nsm.VendorUPC ?? string.Empty,
                                ProductMainDescription = nsm.Active ? (nsm.ShortDescription ?? string.Empty) : "DELETED",
                                ProductExtraDescription = nsm.LongDescription ?? string.Empty,
                                HPPrice = retailPrice,
                                CashPrice = retailPrice,
                                ProductCategory = category.SelectedKey ?? string.Empty,
                                SupplierACNo = string.Empty,
                                ProductStatus = ' ',
                                WarrantableProduct = 'N', // TODO: Check if non-stocks are warrantable or not
                                SpecialPrice = retailPrice,
                                WarrantyReference = "00",
                                ProductEANCode = barcode,
                                ProductLeadTime = string.Empty,
                                WarrantyRenewal = ' ',
                                ReadyToAssemble = ' ',
                                DeletionIndicator = nsm.Active ? 'N' : 'Y',
                                CostPrice = costPrice,
                                SupplierName = string.Empty,
                                TaxRate = nsm.TaxRate.HasValue ? nsm.TaxRate.Value :
                                    (countryTaxRate.HasValue ? countryTaxRate.Value : 0)
                            };

                            fileBuilder = fileBuilder.AppendLine(exporter.CreateProductAmendmentFileLine(pafLine));
                        }
                    }
                }

                exportRetString = fileBuilder.ToString();
                audit.Log(@event: new { LineCount = GetStringLineCount(exportRetString) },
                    category: AuditCategories.NonStocksExport, type: AuditEventTypes.ExportNonStocks);
            }

            return exportRetString;
        }

        public string ExportPromotionsFile(string user)
        {
            var exportRetString = string.Empty;
            var fileBuilder = new StringBuilder();

            var winCosacsBranchLookup = CosacsBranchInfo.Get()
                .ToLookup(e => e.BranchNumber);

            var exporter = new Fact2000Interface();
            using (var scope = Context.Read())
            {
                var nonStocksModels = LoadAll();

                foreach (var nsm in nonStocksModels)
                {
                    if (!nsm.Active)
                    {
                        continue;
                    }

                    var nonStockId = nsm.Id;

                    var allBranches = CosacsBranchInfo.Get();
                    var prices = priceRepository.GetActiveExpandedPrices(nonStockId, allBranches);

                    // select any promotions for this non-stock that didn't end yet
                    var promotions = (from p in scope.Context.NonStockPromotion
                                      where p.NonStockId == nonStockId && p.EndDate > clock.Now
                                      select p)
                                      .ToList()
                                      .Select(p => new Models.NonStockPromotionModel()
                                      {
                                          Id = p.Id,
                                          StartDate = p.StartDate,
                                          EndDate = p.EndDate,
                                          RetailPrice = p.RetailPrice,
                                          PercentageDiscount = p.PercentageDiscount,
                                          Fascia = p.Fascia,
                                          BranchNumber = p.BranchNumber.HasValue ?
                                              p.BranchNumber.Value.ToString() : string.Empty,
                                          NonStockId = p.NonStockId
                                      })
                                      .ToList();

                    if (promotions == null || promotions.Count == 0)
                    {
                        continue;
                    }

                    var pricesExport = ProcessPricesForExport(prices, winCosacsBranchLookup);
                    pricesExport.Sort((i, j) =>
                    {
                        var num1 = i.WarehouseNumber.HasValue ? i.WarehouseNumber.Value : 0;
                        var num2 = j.WarehouseNumber.HasValue ? j.WarehouseNumber.Value : 0;
                        return num1 - num2;
                    });

                    var promotionsExport = ProcessPromotionsForExport(promotions, winCosacsBranchLookup);

                    foreach (var promo in promotionsExport)
                    {
                        var tmpBranchNumber = (short)0;

                        if (short.TryParse(promo.BranchNumber, out tmpBranchNumber))
                        {
                            var warehouseNumber = winCosacsBranchLookup[tmpBranchNumber]
                                .FirstOrDefault()
                                .WarehouseNumber;

                            var effectivePrice = pricesExport
                                .Where(e => e.BranchNumber == tmpBranchNumber)
                                .FirstOrDefault();

                            if (effectivePrice == null)
                                continue;

                            var tmpCashPrice = CalculatePriceForCashType(nsm, effectivePrice, promo, user);
                            var ppfLine = new PromotionalPriceFile()
                            {
                                WarehouseNumber = (short)warehouseNumber,
                                ProductCode = nsm.SKU,

                                HPPrice1 = tmpCashPrice,
                                HPDateFrom1 = promo.StartDate,
                                HPDateTo1 = promo.EndDate,

                                HPPrice2 = 0.000m,
                                HPDateFrom2 = null,
                                HPDateTo2 = null,

                                HPPrice3 = 0.000m,
                                HPDateFrom3 = null,
                                HPDateTo3 = null,

                                CashPrice1 = tmpCashPrice,
                                CashDateFrom1 = promo.StartDate,
                                CashDateTo1 = promo.EndDate,

                                CashPrice2 = 0.000m,
                                CashDateFrom2 = null,
                                CashDateTo2 = null,

                                CashPrice3 = 0.000m,
                                CashDateFrom3 = null,
                                CashDateTo3 = null,
                            };

                            fileBuilder = fileBuilder.AppendLine(exporter.CreatePromotionalPriceFileLine(ppfLine));
                        }
                    }
                }

                exportRetString = fileBuilder.ToString();
                audit.Log(@event: new { LineCount = GetStringLineCount(exportRetString) },
                    category: AuditCategories.NonStocksExport, type: AuditEventTypes.ExportNonStocksPromotions);
            }

            return exportRetString;
        }

        private IEqualityComparer<ProductLinkFile> productFileComparer =
            EqualityComparerFactory<ProductLinkFile>.CreateComparer(
            i =>
                (string.IsNullOrWhiteSpace(i.ProductGroup) ? 0 : i.ProductGroup.GetHashCode()) +
                (string.IsNullOrWhiteSpace(i.Category) ? 0 : i.Category.GetHashCode()) +
                (string.IsNullOrWhiteSpace(i.Class) ? 0 : i.Class.GetHashCode()) +
                (string.IsNullOrWhiteSpace(i.SubClass) ? 0 : i.SubClass.GetHashCode()) +
                (string.IsNullOrWhiteSpace(i.AssociatedItemId) ? 0 : i.AssociatedItemId.GetHashCode())
            , (j, k) =>
                (
                    (string.IsNullOrWhiteSpace(j.ProductGroup) && string.IsNullOrWhiteSpace(k.ProductGroup)) ||
                    string.Compare(j.ProductGroup, k.ProductGroup) == 0)
                && (
                    (string.IsNullOrWhiteSpace(j.Category) && string.IsNullOrWhiteSpace(k.Category)) ||
                    string.Compare(j.Category, k.Category) == 0)
                && (
                    (string.IsNullOrWhiteSpace(j.Class) && string.IsNullOrWhiteSpace(k.Class)) ||
                    string.Compare(j.Class, k.Class) == 0)
                && (
                    (string.IsNullOrWhiteSpace(j.SubClass) && string.IsNullOrWhiteSpace(k.SubClass)) ||
                    string.Compare(j.SubClass, k.SubClass) == 0)
                && (
                    (string.IsNullOrWhiteSpace(j.AssociatedItemId) && string.IsNullOrWhiteSpace(k.AssociatedItemId)) ||
                    string.Compare(j.AssociatedItemId, k.AssociatedItemId) == 0)
        );

        public string ExportProductLinksFile(string user)
        {
            var exportRetString = string.Empty;
            var fileBuilder = new StringBuilder();
            var allProductLinksToExport = new List<ProductLinkFile>();

            ////var winCosacsHierarchyLookup = courtsNetWS.GetDboInfoHierarchy()
            ////    .Where(e => e.Name == "Division")
            ////    .SelectMany(e => e.Data)
            ////    .ToLookup(e => e.Key);

            var exporter = new Fact2000Interface();
            using (var scope = Context.Read())
            {
                var nonStocksLookup = LoadAll().ToLookup(e => e.Id);
                var allProductLinks = productLinkRepository.LoadAllLinks(null);

                foreach (var link in allProductLinks)
                {
                    foreach (var product in link.linkProducts)
                    {
                        foreach (var nonStock in link.linkNonStocks)
                        {
                            var tmpNonStock = nonStocksLookup[nonStock.NonStockId].FirstOrDefault();
                            if (tmpNonStock != null)
                            {
                                var plfLine = new ProductLinkFile()
                                {
                                    ProductGroup = product.Level_1 ?? string.Empty,
                                    Category = product.Level_2 ?? string.Empty,
                                    Class = product.Level_3 ?? string.Empty,
                                    SubClass = string.Empty,
                                    AssociatedItemId = tmpNonStock.SKU,
                                };

                                allProductLinksToExport.Add(plfLine);
                                //fileBuilder = fileBuilder.AppendLine(exporter.CreateProductLinkFileLine(plfLine));
                            }
                        }
                    }
                }

                var productLinksToExportNoDuplicates = new List<ProductLinkFile>();
                if (allProductLinksToExport.Count > 0)
                {
                    productLinksToExportNoDuplicates = allProductLinksToExport.Distinct(productFileComparer).ToList();

                    foreach (var f in productLinksToExportNoDuplicates)
                    {
                        fileBuilder = fileBuilder.AppendLine(exporter.CreateProductLinkFileLine(f));
                    }
                }

                exportRetString = fileBuilder.ToString();

                audit.Log(@event: new { LineCount = GetStringLineCount(exportRetString) },
                    category: AuditCategories.NonStocksExport, type: AuditEventTypes.ExportNonStocksProductLinks);
            }

            return exportRetString;
        }

        private decimal CalculatePriceForCashType(NonStockModel nsm, NonStockPriceModel npm, NonStockPromotionModel promo, string user)
        {
            var countryTaxType = GetTaxInfo(user).TaxType;

            var _npmRetailPrice = npm.RetailPrice;
            if (countryTaxType == "I")
            {
                _npmRetailPrice = npm.TaxInclusivePrice.HasValue ? npm.TaxInclusivePrice.Value : 0.00m;
            }

            var nonStockRetailPrice = (_npmRetailPrice.HasValue && _npmRetailPrice.Value > 0.000m) ? _npmRetailPrice.Value : 0.000m;
            if (nonStockRetailPrice == 0.000m)
            {
                return 0.000m; // non-stock price is zero
            }

            var promotionPrecentValue = promo.PercentageDiscount;
            if (promotionPrecentValue.HasValue && promotionPrecentValue.Value > 0.000m)
            {
                return nonStockRetailPrice * promotionPrecentValue.Value / 100.0m; // return promotional price (percentage discount)
            }

            var promotionPriceValue = promo.RetailPrice;
            if (promotionPriceValue.HasValue && promotionPriceValue.Value > 0.000m)
            {
                // only return the promotional price, if smaller than current retail price
                if (promotionPriceValue.Value <= nonStockRetailPrice)
                {
                    return promotionPriceValue.Value; // return promotional price (value discount)
                }

                return nonStockRetailPrice; // return normal non-stock price
            }

            return 0.000m; // return zero, promotion has no percentage or value discounts set
        }

        private List<NonStockPriceModel> ProcessPricesForExport(List<NonStockPriceModel> prices,
            ILookup<int, BranchInfo> branchLookup)
        {
            var genericPrices = GetGenericPriceDetails(prices);
            var specificPrices = GetAllSpecificPrices(prices);

            var expandedPriceDetails = new List<NonStockPriceModel>();
            if (genericPrices != null && genericPrices.Count > 0)
            {
                expandedPriceDetails = ExpandGenericPriceDetails(genericPrices, branchLookup);
            }

            if (specificPrices.Count > 0)
            {
                if (specificPrices.Where(e => !e.BranchNumber.HasValue).Any())
                {
                    throw new Exception("Found invalid specific price detail!!!");
                }

                expandedPriceDetails = ApplyMoreExpecificPrices(expandedPriceDetails, specificPrices);
            }

            return expandedPriceDetails;
        }

        private List<NonStockPromotionModel> ProcessPromotionsForExport(List<NonStockPromotionModel> promotions,
            ILookup<int, BranchInfo> branchLookup)
        {
            var genericPromotions = GetGenericPromotionsDetails(promotions);
            var specificPromotions = GetAllSpecificPromotions(promotions);

            var expandedPromotionDetails = new List<NonStockPromotionModel>();
            if (genericPromotions != null && genericPromotions.Count > 0)
            {
                expandedPromotionDetails = ExpandGenericPromotionDetails(genericPromotions, branchLookup);
            }

            if (specificPromotions.Count > 0)
            {
                if (specificPromotions.Where(e => string.IsNullOrWhiteSpace(e.BranchNumber)).Any())
                {
                    throw new Exception("Found invalid specific promotion detail!!!");
                }

                expandedPromotionDetails = ApplyMoreExpecificPromotions(expandedPromotionDetails, specificPromotions);
            }

            return expandedPromotionDetails;
        }

        // A NonStockPriceModel object is equal to another if their branches are the same
        private IEqualityComparer<NonStockPriceModel> priceComparer =
            EqualityComparerFactory<NonStockPriceModel>.CreateComparer(
                i => i.BranchNumber.HasValue ? i.BranchNumber.Value : i.GetHashCode(),
                (j, k) =>
                    j.BranchNumber.HasValue && k.BranchNumber.HasValue &&
                    j.BranchNumber.Value == k.BranchNumber.Value
                );

        // A NonStockPromotionModel object is equal to another if their branches are the same
        private IEqualityComparer<NonStockPromotionModel> promoComparer =
            EqualityComparerFactory<NonStockPromotionModel>.CreateComparer(
                i =>
                {
                    var branchNum = 0;
                    return int.TryParse(i.BranchNumber, out branchNum) ? branchNum : i.GetHashCode();
                },
                (j, k) =>
                    !string.IsNullOrWhiteSpace(j.BranchNumber) &&
                    !string.IsNullOrWhiteSpace(k.BranchNumber) &&
                    j.BranchNumber == k.BranchNumber &&
                    j.StartDate == k.StartDate

                );

        private List<NonStockPriceModel> ApplyMoreExpecificPrices(List<NonStockPriceModel> expandedPriceDetailsSet,
            List<NonStockPriceModel> specificPrices)
        {
            if (expandedPriceDetailsSet.Where(e => !e.BranchNumber.HasValue).Any())
            {
                throw new Exception("Invalid expanded price detail!!!");
            }

            if (expandedPriceDetailsSet.Count > 0)
            {
                expandedPriceDetailsSet = expandedPriceDetailsSet.Except(specificPrices, priceComparer).ToList();
            }
            expandedPriceDetailsSet.AddRange(specificPrices);

            return expandedPriceDetailsSet;
        }

        private List<NonStockPromotionModel> ApplyMoreExpecificPromotions(
            List<NonStockPromotionModel> expandedPromotionDetailsSet,
            List<NonStockPromotionModel> specificPromotions)
        {
            if (expandedPromotionDetailsSet.Where(e => e.BranchNumber.Trim().Length == 0).Any())
            {
                throw new Exception("Invalid expanded promotion detail!!!");
            }

            if (expandedPromotionDetailsSet.Count > 0)
            {
                expandedPromotionDetailsSet = expandedPromotionDetailsSet.Except(specificPromotions, promoComparer).ToList();
            }
            expandedPromotionDetailsSet.AddRange(specificPromotions);

            return expandedPromotionDetailsSet;
        }


        private List<NonStockPriceModel> ExpandGenericPriceDetails(List<NonStockPriceModel> genericPrices,
            ILookup<int, BranchInfo> branchLookup)
        {
            var allBranchKeys = branchLookup
                .Select(e => e.Key)
                .ToList();

            var priceAll = (NonStockPriceModel)null;
            var priceFasciaC = (NonStockPriceModel)null;
            var priceFasciaN = (NonStockPriceModel)null;
            // Process the three cases for generic prices
            ProcessAllGenericPriceDetails(genericPrices, out priceAll, out priceFasciaC, out priceFasciaN);

            // Expand price all (most generic detail)
            var priceAllExpandedSet = ExpandGenericPriceDetail(priceAll, branchLookup);
            // Expand fascia details (most generic detail)
            var priceFasciaCExpandedSet = ExpandGenericPriceDetail(priceFasciaC, branchLookup);
            var priceFasciaNExpandedSet = ExpandGenericPriceDetail(priceFasciaN, branchLookup);


            var expandedPrices = priceAllExpandedSet;

            // disconts with fascia are more specific than
            expandedPrices = expandedPrices.Except(priceFasciaCExpandedSet, priceComparer).ToList();
            expandedPrices.AddRange(priceFasciaCExpandedSet);
            expandedPrices = expandedPrices.Except(priceFasciaNExpandedSet, priceComparer).ToList();
            expandedPrices.AddRange(priceFasciaNExpandedSet);

            return expandedPrices;
        }

        private List<NonStockPromotionModel> ExpandGenericPromotionDetails(List<NonStockPromotionModel> genericPromos,
            ILookup<int, BranchInfo> branchLookup)
        {
            var allBranchKeys = branchLookup
                .Select(e => e.Key)
                .ToList();

            var promoAll = (List<NonStockPromotionModel>)null;
            var promoFasciaC = (List<NonStockPromotionModel>)null;
            var promoFasciaN = (List<NonStockPromotionModel>)null;
            // Process the three cases for generic promotions
            ProcessAllGenericPromotionDetails(genericPromos, out promoAll, out promoFasciaC, out promoFasciaN);

            // Expand promo all (most generic detail)
            var promoAllExpandedSet = ExpandGenericPromotionDetail(promoAll, branchLookup);
            // Expand fascia details (most generic detail)
            var promoFasciaCExpandedSet = ExpandGenericPromotionDetail(promoFasciaC, branchLookup);
            var promoFasciaNExpandedSet = ExpandGenericPromotionDetail(promoFasciaN, branchLookup);

            var expandedPromotions = promoAllExpandedSet;

            // disconts with fascia are more specific than
            expandedPromotions = expandedPromotions.Except(promoFasciaCExpandedSet, promoComparer).ToList();
            expandedPromotions.AddRange(promoFasciaCExpandedSet);
            expandedPromotions = expandedPromotions.Except(promoFasciaNExpandedSet, promoComparer).ToList();
            expandedPromotions.AddRange(promoFasciaNExpandedSet);

            return expandedPromotions;
        }

        private void ProcessAllGenericPriceDetails(List<NonStockPriceModel> genericPriceDetails,
            out NonStockPriceModel priceAll,
            out NonStockPriceModel priceFasciaC,
            out NonStockPriceModel priceFasciaN)
        {
            priceAll = genericPriceDetails
                .Where(e => string.IsNullOrWhiteSpace(e.Fascia) && e.BranchNumber == null)
                .FirstOrDefault();
            priceFasciaC = genericPriceDetails.Where(e => e.Fascia != null &&
                string.Compare((e.Fascia ?? string.Empty).Trim(), "C", true) == 0 && e.BranchNumber == null)
                .FirstOrDefault();
            priceFasciaN = genericPriceDetails.Where(e => e.Fascia != null &&
                string.Compare((e.Fascia ?? string.Empty).Trim(), "N", true) == 0 && e.BranchNumber == null)
                .FirstOrDefault();
        }

        private void ProcessAllGenericPromotionDetails(List<NonStockPromotionModel> genericPromotionsDetails,
            out List<NonStockPromotionModel> promoAll,
            out List<NonStockPromotionModel> promoFasciaC,
            out List<NonStockPromotionModel> promoFasciaN)
        {
            promoAll = genericPromotionsDetails
                .Where(e => string.IsNullOrWhiteSpace(e.Fascia) && string.IsNullOrWhiteSpace(e.BranchNumber))
                .ToList();
            promoFasciaC = genericPromotionsDetails.Where(e =>
                string.Compare((e.Fascia ?? string.Empty).Trim(), "C", true) == 0 && string.IsNullOrWhiteSpace(e.BranchNumber))
                .ToList();
            promoFasciaN = genericPromotionsDetails.Where(e =>
                string.Compare((e.Fascia ?? string.Empty).Trim(), "N", true) == 0 && string.IsNullOrWhiteSpace(e.BranchNumber))
                .ToList();
        }

        private List<NonStockPriceModel> ExpandGenericPriceDetail(NonStockPriceModel genericPrice,
            ILookup<int, BranchInfo> branchLookup)
        {
            var expandedPrices = new List<NonStockPriceModel>();

            var allBranchKeys = branchLookup
                .Select(e => e.Key)
                .ToList();

            if (genericPrice == null)
            {
                return expandedPrices;
            }

            if (genericPrice.BranchNumber != null)
            {
                throw new ArgumentException("The price branch number has to be null.");
            }

            foreach (var key in allBranchKeys)
            {
                var branchInfo = branchLookup[key].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(genericPrice.Fascia) &&
                    genericPrice.Fascia != branchInfo.StoreType)
                {
                    continue;
                }

                var priceClone = GetGenericPriceDetailWithBranchInfo(genericPrice, branchInfo);
                expandedPrices.Add(priceClone);
            }

            return expandedPrices;
        }

        private List<NonStockPromotionModel> ExpandGenericPromotionDetail(List<NonStockPromotionModel> genericPromotion,
            ILookup<int, BranchInfo> branchLookup)
        {
            var expandedPromotions = new List<NonStockPromotionModel>();

            var allBranchKeys = branchLookup
                .Select(e => e.Key)
                .ToList();

            if (genericPromotion == null)
            {
                return expandedPromotions;
            }

            foreach (var promotion in genericPromotion)
            {
                if (!string.IsNullOrWhiteSpace(promotion.BranchNumber))
                {
                    throw new ArgumentException("The Promotion branch number has to be null.");
                }

                foreach (var key in allBranchKeys)
                {
                    var branchInfo = branchLookup[key].FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(promotion.Fascia) &&
                        promotion.Fascia != branchInfo.StoreType)
                    {
                        continue;
                    }

                    var promotionClone = GetGenericPromotionDetailWithBranchInfo(promotion, branchInfo);
                    expandedPromotions.Add(promotionClone);
                }

            }

            return expandedPromotions;
        }

        private List<NonStockPriceModel> GetGenericPriceDetails(List<NonStockPriceModel> prices)
        {
            var genericPriceDetails = new List<NonStockPriceModel>();
            if (prices != null)
            {
                genericPriceDetails = prices
                    .Where(e => e.BranchNumber == null)
                    .ToList();
            }

            return genericPriceDetails;
        }

        private List<NonStockPromotionModel> GetGenericPromotionsDetails(List<NonStockPromotionModel> promotions)
        {
            var genericPromotionsDetails = new List<NonStockPromotionModel>();
            if (promotions != null)
            {
                genericPromotionsDetails = promotions
                    .Where(e => string.IsNullOrWhiteSpace(e.BranchNumber))
                    .ToList();
            }

            return genericPromotionsDetails;
        }

        private List<NonStockPriceModel> GetAllSpecificPrices(List<NonStockPriceModel> prices)
        {
            var specificPriceDetails = new List<NonStockPriceModel>();
            if (prices != null)
            {
                specificPriceDetails = prices
                    .Where(e => e.BranchNumber != null)
                    .ToList();
            }

            return specificPriceDetails.ToList();
        }

        private List<NonStockPromotionModel> GetAllSpecificPromotions(List<NonStockPromotionModel> promotions)
        {
            var specificPromotionsDetails = new List<NonStockPromotionModel>();
            if (promotions != null)
            {
                specificPromotionsDetails = promotions
                    .Where(e => !string.IsNullOrWhiteSpace(e.BranchNumber))
                    .ToList();
            }

            return specificPromotionsDetails.ToList();
        }

        private static NonStockPriceModel GetGenericPriceDetailWithBranchInfo(NonStockPriceModel genericPrice,
            BranchInfo branchInfo)
        {
            var tmpPrice = new NonStockPriceModel();

            tmpPrice.Id = genericPrice.Id;
            tmpPrice.NonStockId = genericPrice.NonStockId;
            tmpPrice.CostPrice = genericPrice.CostPrice;
            tmpPrice.RetailPrice = genericPrice.RetailPrice;
            tmpPrice.TaxInclusivePrice = genericPrice.TaxInclusivePrice;
            tmpPrice.DiscountValue = genericPrice.DiscountValue;
            tmpPrice.EffectiveDate = genericPrice.EffectiveDate;
            tmpPrice.EndDate = genericPrice.EndDate;

            tmpPrice.BranchNumber = branchInfo.BranchNumber;
            tmpPrice.WarehouseNumber = branchInfo.WarehouseNumber;
            tmpPrice.Fascia = branchInfo.StoreType;

            return tmpPrice;
        }

        private static NonStockPromotionModel GetGenericPromotionDetailWithBranchInfo(
            NonStockPromotionModel genericPromotion,
            BranchInfo branchInfo)
        {
            var tmpPromotion = new NonStockPromotionModel();

            tmpPromotion.Id = genericPromotion.Id;
            tmpPromotion.NonStockId = genericPromotion.NonStockId;
            tmpPromotion.StartDate = genericPromotion.StartDate;
            tmpPromotion.EndDate = genericPromotion.EndDate;
            tmpPromotion.PercentageDiscount = genericPromotion.PercentageDiscount;
            tmpPromotion.RetailPrice = genericPromotion.RetailPrice;

            tmpPromotion.BranchNumber = branchInfo.BranchNumber.ToString("000");
            tmpPromotion.WarehouseNumber = branchInfo.WarehouseNumber;
            tmpPromotion.Fascia = branchInfo.StoreType;

            return tmpPromotion;
        }

        private void SaveNonStockHierarchy(int nonStockId, List<NonStockHierarchy> hierarchy)
        {
            using (var scope = Context.Write())
            {
                // delete old hierarchy
                var oldHierarchy = (from h in scope.Context.NonStockHierarchy
                                    where h.NonStockId == nonStockId
                                    select h)
                            .ToList();

                if (oldHierarchy.Count() > 0)
                {
                    scope.Context.NonStockHierarchy.RemoveRange(oldHierarchy);
                }

                // prepare new hierarchy
                for (int i = 0; i < hierarchy.Count; i++)
                {
                    hierarchy[i].NonStockId = nonStockId;
                }

                // create new hierarchy
                scope.Context.NonStockHierarchy.AddRange(hierarchy);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private CountryTaxInfo TaxTypeInfo = null;

        private CountryTaxInfo GetTaxInfo(string user)
        {
            if (TaxTypeInfo == null)
            {
                TaxTypeInfo = new CountryTaxInfo(user, clock);
            }
            return TaxTypeInfo;
        }

        /// <summary>
        /// WTF is this? This is the most stupid possible way to count the number of records
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private int GetStringLineCount(string file)
        {
            if (file != null)
            {
                return file.Split(new char[] { '\n' }).Length;
            }
            else
            {
                return 0;
            }
        }
        #region Product Import failure validation
        /// <summary>
        /// Duplicate SKU - Nonstock and Item Master create same SKU code
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        public bool DuplicateInItemMaster(string sku)
        {
            int res = 0;
            SqlParameter[] parameters = new SqlParameter[1];
            using (var scope = Context.Write())
            {
                parameters[0] = new SqlParameter("@SKU", SqlDbType.VarChar,18); 
                parameters[0].Value = sku;
                res = scope.Context.Database.SqlQuery<int>("EXEC NonStockDuplicateInItemMaster @SKU", parameters).Single();
            }
            if (res > 0)
                return true;
            else
                return false;
        }

        #endregion
    }
}