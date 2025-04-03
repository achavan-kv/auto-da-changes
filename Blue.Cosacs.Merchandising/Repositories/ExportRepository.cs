using AutoMapper;
using Blue.Cosacs.Merchandising.Models;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Merchandising.Repositories
{
    using System;
    using System.Data.Entity;
    using Blue.Config.Repositories;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Transactions;
    using Settings = Blue.Cosacs.Merchandising.Settings;
    using System.Data.Entity.Infrastructure;
    using Newtonsoft.Json;

    public interface IExportRepository
    {
        List<PurchaseOrderExportModel> ExportOrders();

        List<SetExportModel> ExportSets();

        List<ProductExportModel> ExportProducts();

        List<StockLevelExportModel> ExportStockLevels();

        List<AssociatedProductExportModel> ExportAssociatedProducts();

        List<PromoExportViewModel> ExportPromotions();

        List<HyperionExportModel> ExportToHyperion();

        List<MagentoExportIneligibleModel> ExportMagentoIneligible();

        List<MagentoExportEligbleModel> ExportMagentoEligible();

        List<AbbreviatedStockExportModel> ExportAbbreviatedStock();
    }

    public class ExportRepository : IExportRepository
    {
        private readonly IRetailPriceRepository retailPriceRepository;

        private readonly Settings settings;

        private readonly PromoPrice promoCalc;

        private readonly ICostRepository costRepository;

        private readonly IProductRepository productRepository;

        private readonly IMerchandisingHierarchyRepository hierarchyRepository;

        private readonly ISettings settingsRepo;

        private readonly IPromotionRepository promotionRepository;

        private readonly Config.Settings blueSettings;
        private readonly IClock clock;

        public ExportRepository(
            IRetailPriceRepository retailPriceRepository,
            Settings settings,
            PromoPrice promoCalc,
            ICostRepository costRepository,
            IProductRepository productRepository,
            IMerchandisingHierarchyRepository hierarchyRepository,
            ISettings settingsRepo,
            IPromotionRepository promotionRepository, 
            Config.Settings blueSettings,
            IClock clock)
        {
            this.retailPriceRepository = retailPriceRepository;
            this.settings = settings;
            this.promoCalc = promoCalc;
            this.costRepository = costRepository;
            this.productRepository = productRepository;
            this.hierarchyRepository = hierarchyRepository;
            this.settingsRepo = settingsRepo;
            this.promotionRepository = promotionRepository;
            this.blueSettings = blueSettings;
            this.clock = clock;
        }

        public List<PurchaseOrderExportModel> ExportOrders()
        {
            using (var scope = Context.Read())
            {
                var orders = scope.Context.OrderExportView.ToList();
                var export = Mapper.Map<IList<PurchaseOrderExportModel>>(orders).ToList();
                return export;
            }
        }

        public List<SetExportModel> ExportSets()
        {
            using (var scope = Context.Read())
            {
                var sets = scope.Context.SetExportView.ToList();
                var export = Mapper.Map<IList<SetExportModel>>(sets).ToList();
                return export;
            }
        }

        public List<ProductExportModel> ExportProducts()
        {
            using (var scope = Context.Read())
            {
                var result = scope.Context.ProductExportView.ToList()
                    .Select(p => 
                        {
                            var att = JsonConvert.DeserializeObject<List<FieldSchema>>(p.Attributes ?? string.Empty);
                            return new ProductExportModel
                {
                                AssemblyRequired = (att == null ? string.Empty : att.FirstOrDefault(a => string.Equals(a.Name, "RTA", StringComparison.OrdinalIgnoreCase)).ToYesNo()).PadRight(1),
                                BarCode = (p.BarCode ?? string.Empty).PadRight(16),
                                Category = p.Category.PadRight(4),
                                Class = p.Class,
                                CostPrice = p.CostPrice.FormatForExport(8, 2),
                                Deleted = p.Deleted.PadRight(1),
                                DutyFreePrice = p.DutyFreePrice.FormatForExport(8, 2),
                                ItemDescr1 = p.ItemDescr1.PadRight(30),
                                ItemDescr2 = p.ItemDescr2.PadRight(45),
                                ItemNo = p.ItemNo.PadRight(8),
                                LeadTime = p.LeadTime.PadRight(3),
                                ProdStatus = p.ProdStatus.PadRight(2),
                                ProdType = p.ProdType.PadRight(2),
                                RefCode = p.RefCode.PadRight(3),
                                SubClass = p.SubClass,
                                Supplier = p.Supplier.PadRight(12),
                                SupplierCode = (p.SupplierCode ?? string.Empty).PadRight(18),
                                SupplierName = (p.SupplierName ?? string.Empty).PadRight(32),
                                TaxRate = p.taxrate.Value.ToString(),
                                UnitPriceCash = p.UnitPriceCash.FormatForExport(8, 2),
                                UnitPriceHP = p.UnitPriceHP.FormatForExport(8, 2),
                                WarehouseNo = p.WarehouseNo.PadRight(3),
                                Warrantable = p.Warrantable.PadRight(2),
                                WarrantyRenewalFlag = p.WarrantyRenewalFlag.PadRight(1)
                            };
                     })
                    .ToList();

                return result;
            }
        }

        public List<StockLevelExportModel> ExportStockLevels()
        {
            using (var scope = Context.Read())
            {
                var stock = scope.Context.StockExportView.ToList();
                var export = Mapper.Map<IList<StockLevelExportModel>>(stock).ToList();
                return export;
            }
        }

        public List<AssociatedProductExportModel> ExportAssociatedProducts()
        {
            var products = this.productRepository.GetAssociatedProductsForExport(); //JSON
            var levels = this.hierarchyRepository.GetAllLevels().ToList();
            var tags = this.hierarchyRepository.GetAllTags().ToList();
            var productsToReturn = products.Select(
                    p =>
                    new AssociatedProductExportModel
                        {
                            Sku = p.SKU,
                            Level1 = this.GetTagCode(levels, tags, p, 1, false), //Get the code from the name
                            Level2 = this.GetTagCode(levels, tags, p, 3, true), //Get legacy category based on the class mapping
                            Level3 = this.GetTagCode(levels, tags, p, 3, false),
                            Level4 = string.Empty // Currently we don't have SubClass
                        }).Distinct().ToList();
            return productsToReturn;
        }

        private string GetTagCode(List<Level> levels, List<Tag> tags, Models.AssociatedProduct prod, int index, bool getLegacyCode)
        {
            if (levels.Count >= index)
            {
                //choose which level based on the index passed
                var levs = (index > 1) ? levels.Skip(index - 1) : levels;
                //get the name of each level
                var key = levs.First().Name.ToLower();

                //if the hierarchy contains the level, get the code for that level
                //if (prod.Hierarchy != null && prod.Hierarchy.ContainsKey(key))
                //{
                    //If the first level, return name not code
                    if (index == 1)
                    {
                        var name = tags.Where(t => t.Level.Name.ToLower() == key && t.Name == prod.HierarchyClass.Division).DefaultIfEmpty(new Tag()).First().Name;
                        if (name == null)
                        {
                            //no name matches, return blank ie match anything 
                            return string.Empty;
                        }
                        return name;
                    }
                    //get the code for the matching tag name within that level
                var code = tags.Where(t => t.Level.Name.ToLower() == key && t.Name == (index == 2? prod.HierarchyClass.Division : prod.HierarchyClass.Class)).DefaultIfEmpty(new Tag()).First().Code;
                    //Check for a legacy code mapping ie department
                    if (getLegacyCode)
                    {
                    return this.GetLegacyCode(code);
                }
                    else
                    {
                        return code;
                    }
                //}
            }
            //no code matches, return blank ie match anything 
            return string.Empty;
        }

        private string GetLegacyCode(string code)
        {
            using (var scope = Context.Read())
            {
                var mapping = scope.Context.ClassMapping.FirstOrDefault(d => d.ClassCode == code);

                if (mapping != null)
                {
                    return mapping.LegacyCode;
                }
                //if there is no mapping, return the tags code
                return code;
            }
        }

        public List<PromoExportViewModel> ExportPromotions()
        {
            var results = this.promotionRepository.GetCurrentPromotions();
            var export = Mapper.Map<IList<PromoExportViewModel>>(results).ToList();
            return export;
        }

        public List<HyperionExportModel> ExportToHyperion()
        {
            var entityCode = string.Format("{0}", new Config.Repositories.Settings().Get("ISOCountryCode"));

            using (var scope = Context.Read())
            {
                var result = new List<HyperionExportModel>();
                var products = scope.Context.HyperionCurrentPriceView.ToList();
                var extractDate = clock.Now;
                var stringDate = extractDate.ToString("yyyyMMdd");
                var month = extractDate.AddMonths(-1).ToString("MMM").ToUpper();
                var financialYear = (extractDate.Month < 4 ? extractDate.Year - 1 : extractDate.Year).ToString();
                var year = clock.Now.Year.ToString();

                products.ForEach(
                    p => result.Add(new HyperionExportModel()
                        {
                            EntityCode = entityCode.AlphaNumericOnly(),
                            ChainCode = p.ChainCode.AlphaNumericOnly(),
                            SalesLocation = p.SalesLocation.AlphaNumericOnly(),
                            CalendarYear = year,
                            FinancialYear = financialYear,
                            InventoryUnits = p.InventoryUnits,
                            InventoryCosts = p.InventoryCosts,
                            InventorySalePrice = p.InventorySalePrice,
                            LastUpdated = stringDate,
                            PurchaseUnits = p.TotalReceived,
                            PurchaseCost = p.TotalCost,
                            PurchaseSalePrice = p.PurchaseSalePrice,
                            SaleUnits = p.TotalSalesUnits,
                            SalesTotal = p.TotalSalesValue,
                            SalesCostWithoutTax = p.SalesCostWithoutTax,
                            InventoryProductMargin = p.InventoryProductMargin,
                            ProductBrandCode = p.BrandCode.AlphaNumericOnly(),
                            ProductBrandDescription = p.BrandName.AlphaNumericOnly(),
                            ProductClassCode = p.ClassCode.AlphaNumericOnly(),
                            ProductClassDescription = p.ClassDescription.AlphaNumericOnly(),
                            Period = month
                        }));
                return result;
            }
        }

        public List<AbbreviatedStockExportModel> ExportAbbreviatedStock()
        {
            using (var scope = Context.Read())
            {
                var stock = scope.Context.AbbreviatedStockExportView.ToList();
                var productIds = stock.Select(s => s.ProductId).Distinct().ToList();
                var levels = hierarchyRepository.GetAllLevels().ToList();
                var hierarchy = scope.Context.ProductHierarchyView.Where(p => productIds.Contains(p.ProductId)).ToList();
                
                var export = MapAbbreviatedStockToExport(stock, hierarchy); 
                export.Insert(
                    0,
                    new AbbreviatedStockExportModel
                    {
                        Hierarchy = string.Join(",", levels.OrderBy(h => h.Id).Select(h => h.Name).ToList()),
                        LocationName = "Location",
                        Sku = "Sku",
                        LongDescription = "Description",
                        VendorName = "Vendor",
                        VendorUPC = "Vendor UPC",
                        StockOnHand = "Stock On Hand",
                        RegularPrice = "Regular Price",
                        CashPrice = "Cash Price",
                        LastLandedCost = "Last Landed Cost",
                        AverageWeightedCost = "Average Weighted Cost"
                    });
                return export;
            }
        }

        private List<AbbreviatedStockExportModel> MapAbbreviatedStockToExport(List<AbbreviatedStockExportView> stock, List<ProductHierarchyView> hierarchy)
        {
            return stock.Select(s => new AbbreviatedStockExportModel
                {
                    Hierarchy = string.Join(",", hierarchy.Where(h => s.ProductId == h.ProductId).OrderBy(h => h.LevelId).Select(h => h.Tag).ToList()),
                    LocationName = s.LocationName,
                    Sku = s.Sku,
                    LongDescription = s.LongDescription,
                    VendorName = s.VendorName,
                    VendorUPC = s.VendorUPC,
                    StockOnHand = s.StockOnHand.ToString(),
                    RegularPrice = s.RegularPrice.ToString(),
                    CashPrice = s.CashPrice.ToString(),
                    LastLandedCost = s.LastLandedCost.ToString(),
                    AverageWeightedCost = s.AverageWeightedCost.ToString()
                }).ToList();
        }

        public List<MagentoExportIneligibleModel> ExportMagentoIneligible()
        {
            using (var scope = Context.Read())
            {
                return
                    scope.Context.Product.Where(
                        p => (p.Status == (int)ProductStatuses.Deleted || p.Status == (int)ProductStatuses.Discontinued || p.ProductType == ProductTypes.RepossessedStock) && p.OnlineDateAdded.HasValue)
                        .Select(p => new MagentoExportIneligibleModel { ProductCode = p.SKU, Desc01 = p.POSDescription, OnlineDateAdded = p.OnlineDateAdded }).ToList();
            }
        }

        public List<MagentoExportEligbleModel> ExportMagentoEligible()
        {
            using (var scope = Context.Read())
            {
                var products =
                    scope.Context.Product.Where(
                        p =>
                        p.Status != (int)ProductStatuses.Deleted && p.Status != (int)ProductStatuses.Discontinued && p.ProductType != ProductTypes.RepossessedStock
                        && p.OnlineDateAdded.HasValue).ToList();

                var productIds = products.Select(p => p.Id).ToList();

                var stockLevels = scope.Context.ProductStockLocationView.Where(l => productIds.Contains(l.ProductId)).ToList();

                var currentRetailPrices = scope.Context.PriceByLocationView.Where(v => productIds.Contains(v.ProductId)).ToList();

                var exports = new List<MagentoExportEligbleModel>();

                var productPromotions = promotionRepository.GetProductPromotions().ToList();

                var productHierarchy = scope.Context.ProductHierarchyConcatView.Where(h => productIds.Contains(h.ProductId)).ToList();

                var countryCode = settingsRepo.Get("ISOCountryCode");

                var decimals = blueSettings.DecimalPlaces;

                foreach (var product in products)
                {
                    var export = new MagentoExportEligbleModel { IsoCountryCode = countryCode, ProductCode = product.SKU, Desc01 = product.POSDescription };

                    var cashPrices = currentRetailPrices.Where(p => p.ProductId == product.Id).ToList();

                    if (cashPrices.Any())
                    {
                        //convert fascias to location prices
                        var retailPrices = currentRetailPrices.Where(p => p.ProductId == product.Id && p.CashPrice != null).ToList();

                        var cashPriceExlTax = retailPrices.GroupBy(p => p.CashPrice).OrderByDescending(g => g.Count()).First().First();
                        export.CashPriceIncTax =
                            decimal.Round(TaxHelper.ApplyTax(cashPriceExlTax.CashPrice.Value, cashPriceExlTax.TaxRate, settings.TaxInclusive, true), decimals)
                                .ToString("0." + string.Join(string.Empty, Enumerable.Repeat("0", decimals)));

                        var hierarchy = productHierarchy.SingleOrDefault(h => h.ProductId == product.Id);

                        var applicablePromo =
                            promotionRepository.CalculatePromotions(
                                new PromotionalPrice
                                    {
                                        Fascia = "Courts",
                                        NormalCashPrice = cashPriceExlTax.CashPrice.Value,
                                        SKU = product.SKU,
                                        Hierarchy = hierarchy != null ? hierarchy.Hierarchy : null,
                                        ProductId = product.Id,
                                        TaxRate = cashPriceExlTax.TaxRate.Value,
                                        ForceApplyTax = true
                                    },
                                productPromotions);

                        if (applicablePromo != null)
                        {
                            if (applicablePromo.PromoCashPrice.HasValue)
                            {
                                export.PromoCashPriceInclTax =
                                    decimal.Round(applicablePromo.PromoCashPrice.Value, decimals).ToString("0." + string.Join(string.Empty, Enumerable.Repeat("0", decimals)));
                            }

                            export.PromoStartDate = applicablePromo.StartDate.ToString("d/MM/yyyy");
                            export.PromoEndDate = applicablePromo.EndDate.ToString("d/MM/yyyy");
                        }
                    }

                    // we dont include products with no online date (and thus have a status of not available)
                    var productStockLevels = stockLevels.Where(l => l.ProductId == product.Id && (product.MagentoExportType == MagentoExportTypes.All || l.Warehouse)).ToList();

                    export.StockQty = productStockLevels.Any() ? productStockLevels.Sum(l => l.StockOnHand) : 0;
                    export.MagMgStock = product.ProductType == ProductTypes.ProductWithoutStock ? 0 : 1;

                    exports.Add(export);
                }

                return exports;
            }
        }
    }
}