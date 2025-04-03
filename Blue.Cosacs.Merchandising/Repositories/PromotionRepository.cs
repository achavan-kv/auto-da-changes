namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using Blue.Transactions;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Product = Blue.Cosacs.Merchandising.Product;
    using PromotionHierarchy = Blue.Cosacs.Merchandising.PromotionHierarchy;

    public interface IPromotionRepository
    {
        bool ProductExistsInAnotherPromo(int productId, string priceType, int? promoId, DateTime startDate, DateTime endDate, int? locationId, string fascia);

        List<PromotionProductView> GetProductPromotions();

        Promotion Get(int id);

        List<PromotionalPriceViewModel> GetForProduct(int productId, string sku, string hierarchy, List<RetailPriceViewModel> prices = null);

        Promotion Save(Promotion model);

        AppliedPromo ApplyPromotion(ApplicablePromo promotion, ProductPrice pricing);

        AppliedPromo CalculatePromotions(PromotionalPrice promoQueryDetails, IEnumerable<PromotionProductView> currentPromotions = null);

        List<int> CalculateEffectedProducts(Dictionary<int, int> promotionHierarchy, IQueryable<ProductHierarchyView> query);

        List<PromotionProductModel> GetCurrentPromotions();

        List<PromotionProductModel> GetAllProductsForPromotion(List<int> promoIds = null);

        Dictionary<string, int> GetPromotionNames();

        bool IsNameUnique(string name, int? id);
    }

    public class PromotionRepository : IPromotionRepository
    {
        private readonly IEventStore audit;

        private readonly IRetailPriceRepository retailPriceRepository;

        private readonly ILocationRepository locationRepository;

        private readonly PromoPrice promoCalc;

        private readonly Blue.Cosacs.Merchandising.Settings merchandiseSettings;

        private readonly Config.ISettingsReader settingsReader;

        private readonly Blue.Config.Settings settings;

        public PromotionRepository(
            IEventStore audit,
            IRetailPriceRepository retailPriceRepository,
            ILocationRepository locationRepository,
            Config.ISettingsReader settingsReader,
            PromoPrice promoCalc,
            Blue.Cosacs.Merchandising.Settings merchandiseSettings,
            Blue.Config.Settings settings)
        {
            this.audit = audit;
            this.retailPriceRepository = retailPriceRepository;
            this.locationRepository = locationRepository;
            this.promoCalc = promoCalc;
            this.merchandiseSettings = merchandiseSettings;
            this.settings = settings;
            this.settingsReader = settingsReader;
        }

        public bool ProductExistsInAnotherPromo(int productId, string priceType, int? promoId, DateTime startDate, DateTime endDate, int? locationId, string fascia)
        {
            using (var scope = Context.Read())
            {
                return
                    scope.Context.PromotionDetailView.ToList()
                        .Any(
                            d =>
                            (!promoId.HasValue || d.PromotionId != promoId) && d.priceType == priceType && (d.LocationId == locationId || d.Fascia == fascia)
                            && d.ProductId == productId
                            && ((d.StartDate.Date <= endDate.Date && d.EndDate.Date >= endDate.Date) || (d.StartDate.Date <= startDate.Date && d.EndDate.Date >= startDate.Date)
                                || (d.StartDate.Date >= startDate.Date && d.StartDate.Date <= endDate.Date)
                                || (d.StartDate.Date >= startDate.Date && d.EndDate.Date <= endDate.Date)));
            }
        }

        public Dictionary<string, int> GetPromotionNames()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Promotion.ToDictionary(k => k.Name, v => v.Id);
            }
        }

        public bool IsNameUnique(string name, int? id)
        {
            using (var scope = Context.Read())
            {
                return !scope.Context.Promotion.Any(p => p.Name == name && (!id.HasValue || p.Id != id.Value));
            }
        }

        public List<PromotionProductView> GetProductPromotions()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.PromotionProductView.Where(p => p.EndDate >= DateTime.Today).ToList();
            }
        }

        public List<PromotionalPriceViewModel> GetForProduct(int productId, string sku, string hierarchy, List<RetailPriceViewModel> prices = null)
        {
            var productPromotions = new List<PromotionalPriceViewModel>();

            if (prices == null)
            {
                prices = retailPriceRepository.GetCurrentForProducts(new List<int> { productId });
            }

            var promos = this.GetProductPromotions().ToList();

            foreach (var price in prices)
            {
                var promo = CalculatePromo(price, sku, hierarchy, promos);
                if (promo != null && !productPromotions.Any(p => p.LocationId == promo.LocationId && p.Fascia == promo.Fascia))
                {
                    productPromotions.Add(promo);
                }

                if (price.Fascia != null)
                {
                    var fasciaLocations = locationRepository.Get(price.Fascia);
                    foreach (var location in fasciaLocations)
                    {
                        var fasciaPrice = new RetailPriceViewModel
                        {
                            Fascia = null,
                            ProductId = price.ProductId,
                            LocationId = location.Id,
                            Location = location.Name,
                            CashPrice = price.CashPrice,
                            RegularPrice = price.RegularPrice,
                            DutyFreePrice = price.DutyFreePrice,
                            TaxRate = price.TaxRate
                        };

                        var fasciaPromo = CalculatePromo(fasciaPrice, sku, hierarchy, promos);

                        if (fasciaPromo != null && !productPromotions.Any(p => p.LocationId == fasciaPromo.LocationId && p.Fascia == fasciaPromo.Fascia))
                        {
                            productPromotions.Add(fasciaPromo);
                        }
                    }
                }

                if (price.Fascia == null && price.LocationId == null)
                {
                    var fascias = settingsReader.ListAll("Fascia");
                    foreach (var fascia in fascias)
                    {
                        var allPrice = new RetailPriceViewModel
                        {
                            Fascia = fascia,
                            ProductId = price.ProductId,
                            CashPrice = price.CashPrice,
                            RegularPrice = price.RegularPrice,
                            DutyFreePrice = price.DutyFreePrice,
                            TaxRate = price.TaxRate
                        };

                        var allPromo = CalculatePromo(allPrice, sku, hierarchy, promos);

                        if (allPromo != null && !productPromotions.Any(p => p.LocationId == allPromo.LocationId && p.Fascia == allPromo.Fascia))
                        {
                            productPromotions.Add(allPromo);
                        }
                    }

                    var allLocations = locationRepository.Get();
                    foreach (var location in allLocations)
                    {
                        var allPrice = new RetailPriceViewModel
                        {
                            Fascia = null,
                            ProductId = price.ProductId,
                            LocationId = location.Id,
                            Location = location.Name,
                            CashPrice = price.CashPrice,
                            RegularPrice = price.RegularPrice,
                            DutyFreePrice = price.DutyFreePrice,
                            TaxRate = price.TaxRate
                        };

                        var allPromo = CalculatePromo(allPrice, sku, hierarchy, promos);

                        if (allPromo != null && !productPromotions.Any(p => p.LocationId == allPromo.LocationId && p.Fascia == allPromo.Fascia))
                        {
                            productPromotions.Add(allPromo);
                        }
                    }
                }
            }
            return productPromotions;
        }

        private PromotionalPriceViewModel CalculatePromo(RetailPriceViewModel price, string sku, string hierarchy, List<PromotionProductView> promos)
        {
            var promoPrice = new PromotionalPrice
            {
                Fascia = price.Fascia,
                ProductId = price.ProductId,
                SKU = sku,
                Hierarchy = hierarchy,
                LocationId = price.LocationId,
                NormalCashPrice = price.CashPrice,
                NormalRegularPrice = price.RegularPrice,
                NormalDutyFreePrice = price.DutyFreePrice,
                TaxRate = price.TaxRate,
                ForceApplyTax = false
            };

            var applicablePromo = this.CalculatePromotions(promoPrice, promos);

            if (applicablePromo != null)
            {
                var promo = new PromotionalPriceViewModel
                {
                    ProductId = price.ProductId,
                    Fascia = price.Fascia,
                    Location = price.Location,
                    LocationId = price.LocationId,
                    EffectiveDate = applicablePromo.StartDate,
                    TaxRate = price.TaxRate,
                    CashPrice = applicablePromo.PromoCashPrice,
                    RegularPrice = applicablePromo.PromoRegularPrice,
                    DutyFreePrice = applicablePromo.PromoDutyFreePrice,
                    StartDate = applicablePromo.StartDate,
                    EndDate = applicablePromo.EndDate,
                };

                return promo;
            }
            return null;
        }

        public Promotion Get(int id)
        {
            using (var scope = Context.Read())
            {
                var model = new Promotion();
                var promo = scope.Context.Promotion.Find(id);
                string locationFascia = null;

                if (promo == null)
                {
                    return null;
                }
                Mapper.Map(promo, model);

                var promoDetails = scope.Context.PromotionDetail.Where(d => d.PromotionId == id).ToList();

                if (promo.LocationId.HasValue)
                {
                    var location = scope.Context.Location.First(l => l.Id == promo.LocationId);
                    model.Location = location.Name;
                    locationFascia = location.Fascia;
                }

                var promoProductIds = promoDetails.Select(d => d.ProductId).Distinct().ToList();

                var promoProducts = new List<Product>();
                var promoProductsRetailPrices = new List<RetailPriceView>();
                var promoSetPrices = new List<SetLocationView>();
                var promoAWC = new Dictionary<int, decimal>();

                if (promoProductIds.Any())
                {
                    promoProducts = scope.Context.Product.Where(p => promoProductIds.Contains(p.Id)).ToList();

                    promoProductsRetailPrices =
                        scope.Context.RetailPriceView.Where(
                            p =>
                            promoProductIds.Contains(p.ProductId)
                            && p.EffectiveDate <= promo.StartDate
                            && (((promo.LocationId == null || p.LocationId == promo.LocationId || (locationFascia != null && p.Fascia == locationFascia))
                                    && (promo.Fascia == null || p.Fascia == promo.Fascia))
                                || (p.Fascia == null && p.LocationId == null))).ToList();

                    promoAWC = scope.Context.CostPriceView.Where(p => promoProductIds.Contains(p.ProductId) && p.AverageWeightedCost.HasValue).Select(s => new { s.ProductId, s.AverageWeightedCostUpdated, s.AverageWeightedCost }).ToList()
                                                          .GroupBy(g => g.ProductId)
                                                          .Select(s => new { AWC = s.OrderByDescending(o => o.AverageWeightedCostUpdated).Select(a => a.AverageWeightedCost).DefaultIfEmpty(0).FirstOrDefault(), ProductId = s.Key })
                                                          .ToDictionary(d => d.ProductId, d => d.AWC.Value);
                }

                foreach (var promoDetail in promoDetails)
                {
                    var detailModel = Mapper.Map<PromotionDetail>(promoDetail);
                    RetailPriceView price = null;

                    if (promoDetail.ProductId.HasValue)
                    {
                        var promoProduct = promoProducts.First(p => p.Id == detailModel.ProductId);

                        var matchingPrices = promoProductsRetailPrices
                                                .Where(p => promo.LocationId == p.LocationId && promo.Fascia == p.Fascia && detailModel.ProductId == p.ProductId);

                        if (matchingPrices.Any())
                        {
                            price = matchingPrices
                                           .OrderBy(p => p.EffectiveDate)
                                           .ToList()
                                           .Last(p => p.ProductId == detailModel.ProductId);
                        }

                        // get the price that is current at the start of the promotion
                        RetailPriceView retailPrice = null;
                        switch (promoProduct.ProductType)
                        {
                            case "Set":
                                retailPrice =
                                   promoProductsRetailPrices.First(p => p.ProductId == promoDetail.ProductId.Value);

                                break;
                            default:
                                retailPrice = price ??
                                    promoProductsRetailPrices.OrderBy(p => p.LocationId).ThenBy(p => p.Fascia).ThenBy(p => p.EffectiveDate)
                                        .ToList()
                                        .Last(p => p.ProductId == detailModel.ProductId);
                                break;
                        }

                        detailModel.TaxRate = retailPrice.TaxRate;
                        detailModel.Sku = promoProduct.SKU;
                        detailModel.Name = promoProduct.POSDescription;
                        detailModel.AverageWeightedCost = promoAWC.ContainsKey(promoDetail.ProductId.Value) ? promoAWC[promoDetail.ProductId.Value] : 0;

                        switch ((PriceType)detailModel.PriceType)
                        {
                            case PriceType.Cash:
                                detailModel.OriginalPrice = retailPrice.CashPrice.Value;
                                detailModel.PriceType = (int)PriceType.Cash;
                                detailModel.PriceTypeName = PriceType.Cash.ToString();
                                break;
                            case PriceType.DutyFree:
                                detailModel.OriginalPrice = retailPrice.DutyFreePrice.Value;
                                detailModel.PriceType = (int)PriceType.DutyFree;
                                detailModel.PriceTypeName = PriceType.DutyFree.ToString();
                                break;
                            case PriceType.Regular:
                                detailModel.OriginalPrice = retailPrice.RegularPrice.Value;
                                detailModel.PriceType = (int)PriceType.Regular;
                                detailModel.PriceTypeName = PriceType.Regular.ToString();
                                break;
                        }
                    }
                    else
                    {
                        var allLevels = scope.Context.HierarchyLevel.ToList();
                        var allTags = scope.Context.HierarchyTag.ToList();

                        var hierarchies = scope.Context.PromotionHierarchy.Where(h => h.PromotionDetailId == promoDetail.Id).ToList();
                        detailModel.Hierarchies =
                            hierarchies.Select(
                                h =>
                                new Models.PromotionHierarchy
                                {
                                    Id = h.Id,
                                    LevelId = h.LevelId.Value,
                                    TagId = h.TagId.Value,
                                    LevelName = allLevels.First(l => l.Id == h.LevelId).Name,
                                    TagName = allTags.First(t => t.Id == h.TagId).Name
                                }).ToList();
                    }

                    detailModel.PercentDiscount = promoDetail.PercentDiscount;
                    detailModel.ValueDiscount = promoDetail.ValueDiscount;
                    detailModel.SetPrice = promoDetail.Price;

                    model.Details.Add(detailModel);
                }

                return model;
            }
        }

        public Promotion Save(Promotion model)
        {
            using (var scope = Context.Write())
            {
                var promotion = scope.Context.Promotion.Find(model.Id);
                var decimalPlaces = settings.DecimalPlaces;
                string eventType;

                if (promotion == null)
                {
                    promotion = Mapper.Map<Merchandising.Promotion>(model);

                    scope.Context.Promotion.Add(promotion);
                    eventType = PromotionEvents.CreatePromotion;
                }
                else
                {
                    Mapper.Map(model, promotion);
                    eventType = PromotionEvents.EditPromotion;
                }

                var location = scope.Context.Location.FirstOrDefault(l => l.Id == promotion.LocationId);

                scope.Context.SaveChanges();
                audit.LogAsync(
                    new
                    {
                        PromotionName = promotion.Name,
                        LocationName = location != null ? location.Name : "All",
                        Fascia = string.IsNullOrWhiteSpace(promotion.Fascia) ? "All" : promotion.Fascia,
                        StartDate = promotion.StartDate.Date,
                        EndDate = promotion.EndDate.Date,
                        promotion.PromotionType
                    },
                    eventType,
                    EventCategories.Merchandising);

                // remove details that are no longer in the promotion
                var existingDetails = scope.Context.PromotionDetail.Where(d => d.PromotionId == promotion.Id).ToList();
                var detailsToBeSaved = model.Details.Where(md => md.Id.HasValue).Select(md => md.Id).ToList();

                foreach (var detailToRemove in existingDetails.Where(d => !detailsToBeSaved.Contains(d.Id)))
                {
                    var hierarchiesToRemove = scope.Context.PromotionHierarchy.Where(h => h.PromotionDetailId == detailToRemove.Id);

                    foreach (var hierarchyToRemove in hierarchiesToRemove)
                    {
                        scope.Context.PromotionHierarchy.Remove(hierarchyToRemove);
                        scope.Context.SaveChanges();
                        var level = scope.Context.HierarchyLevel.FirstOrDefault(l => l.Id == hierarchyToRemove.LevelId).Name;
                        var tag = scope.Context.HierarchyTag.FirstOrDefault(l => l.Id == hierarchyToRemove.TagId).Name;

                        audit.LogAsync(
                            new
                            {
                                PromotionName = promotion.Name,
                                detailToRemove.PriceType,
                                Price = detailToRemove.Price.HasValue ? Math.Round(detailToRemove.Price.Value, decimalPlaces) : detailToRemove.Price,
                                detailToRemove.ValueDiscount,
                                detailToRemove.PercentDiscount,
                                level,
                                tag
                            },
                            PromotionEvents.DeletePromotionHierarchy,
                            EventCategories.Merchandising);
                    }

                    scope.Context.PromotionDetail.Remove(detailToRemove);
                    scope.Context.SaveChanges();

                    if (hierarchiesToRemove == null || hierarchiesToRemove.Count() == 0)
                    {
                        var product = scope.Context.Product.FirstOrDefault(p => p.Id == detailToRemove.ProductId);
                        audit.LogAsync(
                            new
                            {
                                PromotionName = promotion.Name,
                                detailToRemove.PriceType,
                                Price = detailToRemove.Price.HasValue ? Math.Round(detailToRemove.Price.Value, decimalPlaces) : detailToRemove.Price,
                                detailToRemove.ValueDiscount,
                                detailToRemove.PercentDiscount,
                                product.SKU
                            },
                            PromotionEvents.DeletePromotionDetail,
                            EventCategories.Merchandising);
                    }
                }

                var allTags = scope.Context.TagView.ToList();
                var allLevels = scope.Context.HierarchyLevel.ToList();

                // add /update any new details
                foreach (var modelDetail in model.Details)
                {
                    Merchandising.PromotionDetail detail;
                    if (modelDetail.Id.HasValue)
                    {
                        // existing details can only have their discount changed
                        detail = scope.Context.PromotionDetail.Find(modelDetail.Id);
                        detail.PercentDiscount = modelDetail.PercentDiscount;
                        detail.ValueDiscount = modelDetail.ValueDiscount;
                        detail.Price = modelDetail.SetPrice;
                        eventType = PromotionEvents.CreatePromotionDetail;
                    }
                    else
                    {
                        detail = Mapper.Map<Merchandising.PromotionDetail>(modelDetail);
                        scope.Context.PromotionDetail.Add(detail);
                        detail.PromotionId = promotion.Id;
                        eventType = PromotionEvents.EditPromotionDetail;
                    }
                    scope.Context.SaveChanges();

                    if (modelDetail.Hierarchies == null || modelDetail.Hierarchies.Count == 0)
                    {
                        var product = scope.Context.Product.FirstOrDefault(p => p.Id == detail.ProductId);
                        audit.LogAsync(
                            new
                            {
                                PromotionName = promotion.Name,
                                detail.PriceType,
                                Price = detail.Price.HasValue ? Math.Round(detail.Price.Value, decimalPlaces) : detail.Price,
                                detail.ValueDiscount,
                                detail.PercentDiscount,
                                product.SKU
                            },
                            eventType,
                            EventCategories.Merchandising);
                    }

                    // hierarchy details cannot be updated, only added or removed 
                    // which has already happened
                    foreach (var modelHierarchy in modelDetail.Hierarchies.Where(h => !h.Id.HasValue))
                    {
                        var newHierarchy = new PromotionHierarchy
                        {
                            PromotionDetailId = detail.Id,
                            LevelId = allLevels.First(l => string.Equals(l.Name, modelHierarchy.LevelName, StringComparison.OrdinalIgnoreCase)).Id,
                            TagId = allTags.First(t => string.Equals(t.TagName, modelHierarchy.TagName, StringComparison.OrdinalIgnoreCase)
                                && string.Equals(t.LevelName, modelHierarchy.LevelName, StringComparison.OrdinalIgnoreCase)).Id
                        };
                        scope.Context.PromotionHierarchy.Add(newHierarchy);
                        scope.Context.SaveChanges();
                        var level = scope.Context.HierarchyLevel.FirstOrDefault(l => l.Id == newHierarchy.LevelId).Name;
                        var tag = scope.Context.HierarchyTag.FirstOrDefault(l => l.Id == newHierarchy.TagId).Name;

                        audit.LogAsync(
                            new
                            {
                                PromotionName = promotion.Name,
                                detail.PriceType,
                                Price = detail.Price.HasValue ? Math.Round(detail.Price.Value, decimalPlaces) : detail.Price,
                                detail.ValueDiscount,
                                detail.PercentDiscount,
                                level,
                                tag
                            },
                            PromotionEvents.CreatePromotionHierarchy,
                            EventCategories.Merchandising);
                    }
                }
                var promo = Get(promotion.Id);
                scope.Complete();
                return promo;
            }
        }

        private decimal CalculatePromoPrice(ApplicablePromo promotion, decimal? exclusivePrice, decimal? taxRate, bool? forceApplyTax)
        {
            decimal price;

            if (promotion.Price != null)
            {
                price = this.ApplyTax(promotion.Price.Value, taxRate.GetValueOrDefault(0), forceApplyTax);
            }
            else if (promotion.ValueDiscount.HasValue)
            {
                var normalPrice = this.ApplyTax(exclusivePrice.Value, taxRate.GetValueOrDefault(0), null);
                var valueOff = this.ApplyTax(promotion.ValueDiscount.Value, taxRate.GetValueOrDefault(0), null);
                var promoPrice = normalPrice - valueOff;

                if (forceApplyTax.HasValue && forceApplyTax.Value != merchandiseSettings.TaxInclusive)
                {
                    if (forceApplyTax.Value && !merchandiseSettings.TaxInclusive)
                    {
                        price = this.ApplyTax(promoPrice, taxRate.GetValueOrDefault(0), true);
                    }
                    else
                    {
                        price = this.RemoveTax(promoPrice, taxRate.GetValueOrDefault(0));
                    }
                }
                else
                {
                    price = promoPrice;
                }
            }
            else
            {
                var normalPrice = this.ApplyTax(exclusivePrice.Value, taxRate.GetValueOrDefault(0), null);
                var promoPrice = this.promoCalc.RoundPromoPrice(Convert.ToDecimal(normalPrice * (1 - promotion.PercentDiscount)), normalPrice);

                if (forceApplyTax.HasValue && forceApplyTax.Value != merchandiseSettings.TaxInclusive)
                {
                    if (forceApplyTax.Value && !merchandiseSettings.TaxInclusive)
                    {
                        price = this.ApplyTax(promoPrice, taxRate.GetValueOrDefault(0), true);
                    }
                    else
                    {
                        price = this.RemoveTax(promoPrice, taxRate.GetValueOrDefault(0));
                    }
                }
                else
                {
                    price = promoPrice;
                }
            }

            return price;
        }

        public AppliedPromo ApplyPromotion(ApplicablePromo promotion, ProductPrice pricing)
        {
            var appliedPromo = new AppliedPromo { StartDate = promotion.StartDate, EndDate = promotion.EndDate };

            switch (promotion.PriceType)
            {
                case "Cash":
                    appliedPromo.PromoCashPrice = CalculatePromoPrice(promotion, pricing.NormalCashPrice, pricing.TaxRate, pricing.ForceApplyTax);
                    break;
                case "Regular":
                    appliedPromo.PromoRegularPrice = CalculatePromoPrice(promotion, pricing.NormalRegularPrice, pricing.TaxRate, pricing.ForceApplyTax);
                    break;
                case "DutyFree":
                    appliedPromo.PromoDutyFreePrice = CalculatePromoPrice(promotion, pricing.NormalDutyFreePrice, pricing.TaxRate, pricing.ForceApplyTax);
                    break;
            }

            return appliedPromo;
        }

        public AppliedPromo CalculatePromotions(PromotionalPrice promoQueryDetails, IEnumerable<PromotionProductView> currentPromotions = null)
        {
            var deserialiseHierarchy = !string.IsNullOrWhiteSpace(promoQueryDetails.Hierarchy)
                                           ? JsonConvert.DeserializeObject<List<HierarchyLevel>>(promoQueryDetails.Hierarchy)
                                           : new List<HierarchyLevel>();

            AppliedPromo appliedPromo = null;

            if (currentPromotions == null)
            {
                currentPromotions = this.GetProductPromotions().ToList();
            }

            // find all applicable promos - ones that match on hierarchy or sku
            var applicablePromoItems = currentPromotions.Where(p => p.Sku == promoQueryDetails.SKU).ToList();
            applicablePromoItems = applicablePromoItems.Union(
                currentPromotions.Where(
                    p =>
                        {
                            // check that all hierarchies of the promo match the product hierarchies exactly
                            var hierarchy = JsonConvertHelper.DeserializeObjectOrDefault<List<HierarchyLevel>>(p.Hierarchy);
                            return hierarchy != null && hierarchy.Count > 0 && !hierarchy.Except(deserialiseHierarchy, new LevelComparer()).Any();
                        })).ToList();

            // get promos that apply for the tickets location / fascia
            applicablePromoItems =
                applicablePromoItems.Where(
                    p =>
                    ((p.LocationId == null && promoQueryDetails.LocationId == null) || p.LocationId == promoQueryDetails.LocationId)
                    && ((string.IsNullOrEmpty(p.Fascia) && string.IsNullOrEmpty(promoQueryDetails.Fascia)) || p.Fascia == promoQueryDetails.Fascia)).ToList();

            var promoGroup = applicablePromoItems.GroupBy(p => new { p.Sku, p.Hierarchy }).ToList();

            if (promoGroup.Any())
            {
                // this will contain all of the different price types in the promotion that apply to the product
                var applicablePromoGroup = promoGroup.Any(p => p.Key.Sku != null)
                                               ? promoGroup.Where(p => p.Key.Sku != null)
                                               : promoGroup.Where(p => !string.IsNullOrWhiteSpace(p.Key.Hierarchy)).Select(a => a);

                foreach (var applicablePromoType in applicablePromoGroup)
                {
                    foreach (var applicablePromoItem in applicablePromoType)
                    {
                        var applicablePromotion = new ApplicablePromo
                        {
                            PercentDiscount = applicablePromoItem.PercentDiscount,
                            Price = applicablePromoItem.Price,
                            PriceType = applicablePromoItem.PriceType,
                            ValueDiscount = applicablePromoItem.ValueDiscount,
                            StartDate = applicablePromoItem.StartDate,
                            EndDate = applicablePromoItem.EndDate
                        };

                        var pricesAtPromoStart = retailPriceRepository.GetForProductLocationEffectiveDate(
                            promoQueryDetails.LocationId,
                            promoQueryDetails.Fascia,
                            promoQueryDetails.ProductId,
                            applicablePromoItem.StartDate);

                        if (pricesAtPromoStart == null)
                        {
                            return null;
                        }

                        var productPricing = new ProductPrice
                        {
                            NormalCashPrice =
                                                         appliedPromo != null && appliedPromo.PromoCashPrice.HasValue
                                                             ? appliedPromo.PromoCashPrice
                                                             : pricesAtPromoStart.CashPrice,
                            NormalDutyFreePrice =
                                                         appliedPromo != null && appliedPromo.PromoDutyFreePrice.HasValue
                                                             ? appliedPromo.PromoDutyFreePrice
                                                             : pricesAtPromoStart.DutyFreePrice,
                            NormalRegularPrice =
                                                         appliedPromo != null && appliedPromo.PromoRegularPrice.HasValue
                                                             ? appliedPromo.PromoRegularPrice
                                                             : pricesAtPromoStart.RegularPrice,
                            TaxRate = promoQueryDetails.TaxRate,
                            ForceApplyTax = promoQueryDetails.ForceApplyTax
                        };

                        var typePromotion = this.ApplyPromotion(applicablePromotion, productPricing);

                        if (appliedPromo == null)
                        {
                            appliedPromo = typePromotion;
                        }
                        else
                        {
                            appliedPromo.PromoCashPrice = typePromotion.PromoCashPrice.HasValue ? typePromotion.PromoCashPrice : appliedPromo.PromoCashPrice;
                            appliedPromo.PromoDutyFreePrice = typePromotion.PromoDutyFreePrice.HasValue ? typePromotion.PromoDutyFreePrice : appliedPromo.PromoDutyFreePrice;
                            appliedPromo.PromoRegularPrice = typePromotion.PromoRegularPrice.HasValue ? typePromotion.PromoRegularPrice : appliedPromo.PromoRegularPrice;
                        }
                    }
                }
            }
            return appliedPromo;
        }

        private decimal ApplyTax(decimal price, decimal taxRate, bool? forceApply)
        {
            var tax = forceApply.HasValue ? forceApply.Value : merchandiseSettings.TaxInclusive;
            return tax ? price * (1 + taxRate) : price;
        }

        private decimal RemoveTax(decimal price, decimal taxRate)
        {
            return price / (1 + taxRate);
        }

        public List<PromotionProductModel> GetCurrentPromotions()
        {
            var results = new List<PromotionProductModel>();
            using (var scope = Context.Read())
            {
                var dateNow = DateTime.Now.Date.AddDays(-1);    //cannot have this statement within the LINQ query  //Subtract a day in case EOD is started late and runs after midnight
                var promos = scope.Context.Promotion.AsNoTracking().Where(p => p.EndDate >= dateNow).ToList();
                var promoIds = promos.Select(p => p.Id).ToList();

                var locations = scope.Context.Location.ToList();

                var allPromoLocations =
                    promos.Where(p => !string.IsNullOrWhiteSpace(p.Fascia))
                        .SelectMany(p => locations.Where(l => l.Fascia == p.Fascia).Select(l => new KeyValuePair<int, int>(p.Id, l.Id))) // promos that match on fascia
                        .Union(promos.Where(p => p.LocationId.HasValue).Select(p => new KeyValuePair<int, int>(p.Id, p.LocationId.Value))) // promos that match on location
                        .Union(
                            promos.Where(p => string.IsNullOrWhiteSpace(p.Fascia) && !p.LocationId.HasValue)
                                .SelectMany(p => locations.Select(l => new KeyValuePair<int, int>(p.Id, l.Id)))) // promo matches all locations
                        .GroupBy(k => k.Key, k => k.Value).ToList();

                var promoDetails = scope.Context.PromotionDetail.AsNoTracking().Where(pd => promoIds.Contains(pd.PromotionId)).ToList();

                var skuPromos = this.GetSkuBasedProductPromotion(promoDetails, promos, allPromoLocations, locations);
                var hierarchyPromos = this.GetHierarchyBasedProductPromotions(promoDetails, scope, promos, allPromoLocations, locations);

                // sku promos take precedence on hierarchy
                results = skuPromos.Union(hierarchyPromos.Where(hp => !skuPromos.Select(s => s.ProductId).Contains(hp.ProductId))).ToList();

                //only include the most specific prices
                var promosToRemove =
                    results.Where(r => r.PromotionLocation == PromotionLocation.All
                        && results.Exists(x => (x.PromotionLocation == PromotionLocation.Fascia || x.PromotionLocation == PromotionLocation.Location) && x.ProductId == r.ProductId && x.LocationId == r.LocationId))
                    .Union(
                        results.Where(r => r.PromotionLocation == PromotionLocation.Fascia
                            && results.Exists(x => (x.PromotionLocation == PromotionLocation.Location) && x.ProductId == r.ProductId && x.LocationId == r.LocationId))).ToList();

                results.RemoveAll(p => promosToRemove.Contains(p));

                var productIds = results.Select(r => r.ProductId).ToList();
                var products = scope.Context.Product.AsNoTracking().Where(p => productIds.Contains(p.Id)).ToList();

                foreach (var promo in results)
                {
                    promo.SalesId = locations.Single(l => l.Id == promo.LocationId).SalesId;
                    promo.Sku = products.Single(p => p.Id == promo.ProductId).SKU;
                }
                return results;
            }
        }

        public List<PromotionProductModel> GetAllProductsForPromotion(List<int> promoIds = null)
        {
            var results = new List<PromotionProductModel>();
            using (var scope = Context.Read())
            {
                var promos = promoIds == null ? scope.Context.Promotion.AsNoTracking().ToList() : scope.Context.Promotion.AsNoTracking().Where(p => promoIds.Contains(p.Id)).ToList();
                if (promoIds == null)
                {
                    promoIds = promos.Select(p => p.Id).ToList();
                }

                var locations = scope.Context.Location.ToList();

                var allPromoLocations =
                    promos.Where(p => !string.IsNullOrWhiteSpace(p.Fascia))
                        .SelectMany(p => locations.Where(l => l.Fascia == p.Fascia).Select(l => new KeyValuePair<int, int>(p.Id, l.Id))) // promos that match on fascia
                        .Union(promos.Where(p => p.LocationId.HasValue).Select(p => new KeyValuePair<int, int>(p.Id, p.LocationId.Value))) // promos that match on location
                        .Union(
                            promos.Where(p => string.IsNullOrWhiteSpace(p.Fascia) && !p.LocationId.HasValue)
                                .SelectMany(p => locations.Select(l => new KeyValuePair<int, int>(p.Id, l.Id)))) // promo matches all locations
                        .GroupBy(k => k.Key, k => k.Value).ToList();

                var promoDetails = scope.Context.PromotionDetail.AsNoTracking().Where(pd => promoIds.Contains(pd.PromotionId)).ToList();

                var skuPromos = this.GetSkuBasedProductPromotion(promoDetails, promos, allPromoLocations, locations);
                var hierarchyPromos = this.GetHierarchyBasedProductPromotions(promoDetails, scope, promos, allPromoLocations, locations);

                // sku promos take precedence on hierarchy
                results = skuPromos.Union(hierarchyPromos.Where(hp => !skuPromos.Select(s => s.ProductId).Contains(hp.ProductId))).ToList();

                //only include the most specific prices
                var promosToRemove =
                    results.Where(r => r.PromotionLocation == PromotionLocation.All
                        && results.Exists(x => (x.PromotionLocation == PromotionLocation.Fascia || x.PromotionLocation == PromotionLocation.Location) && x.ProductId == r.ProductId && x.LocationId == r.LocationId))
                    .Union(
                        results.Where(r => r.PromotionLocation == PromotionLocation.Fascia
                            && results.Exists(x => (x.PromotionLocation == PromotionLocation.Location) && x.ProductId == r.ProductId && x.LocationId == r.LocationId))).ToList();

                results.RemoveAll(p => promosToRemove.Contains(p));

                var productIds = results.Select(r => r.ProductId).ToList();
                var products = scope.Context.Product.AsNoTracking().Where(p => productIds.Contains(p.Id)).ToList();

                foreach (var promo in results)
                {
                    promo.SalesId = locations.Single(l => l.Id == promo.LocationId).SalesId;
                    promo.Sku = products.Single(p => p.Id == promo.ProductId).SKU;
                }
                return results;
            }
        }

        private List<PromotionProductModel> GetHierarchyBasedProductPromotions(
            List<Blue.Cosacs.Merchandising.PromotionDetail> promoDetails,
            ReadScope<Context> scope,
            List<Blue.Cosacs.Merchandising.Promotion> promos,
            List<IGrouping<int, int>> allPromoLocations,
            List<Location> locations)
        {
            var promoDetailsHierarchy = promoDetails.Where(pd => !pd.ProductId.HasValue).ToList();
            var promoDetailsHierarchyIds = promoDetailsHierarchy.Select(pdh => pdh.Id).ToList();

            var promoHierarchies =
                scope.Context.PromotionHierarchy.AsNoTracking().Where(ph => promoDetailsHierarchyIds.Contains(ph.PromotionDetailId)).GroupBy(ph => ph.PromotionDetailId).ToList();

            var productHierarchies =
                scope.Context.ProductHierarchyView.AsNoTracking()
                    .Where(p =>
                        p.Status != (int)ProductStatuses.NonActive
                        && (p.ProductType == ProductTypes.RegularStock || p.ProductType == ProductTypes.Set
                            || p.ProductType == ProductTypes.ProductWithoutStock));

            var newPromoDetailsHierarchy = new List<Blue.Cosacs.Merchandising.PromotionDetail>();

            // calculate products that match the hierarchies then treat it as if its a sku based promotion
            foreach (var promoHierarchy in promoHierarchies)
            {
                var promoDetail = promoDetails.Single(pd => pd.Id == promoHierarchy.Key);
                var promo = promos.Single(p => p.Id == promoDetail.PromotionId);

                var effectedProducts = this.CalculateEffectedProducts(
                        promoHierarchy.Where(p => p.LevelId != null && p.TagId != null)
                            .ToDictionary(k => k.LevelId.Value, v => v.TagId.Value),
                        productHierarchies).Distinct().ToList();

                newPromoDetailsHierarchy.AddRange(
                    effectedProducts.Select(
                        p =>
                        new Blue.Cosacs.Merchandising.PromotionDetail
                        {
                            PromotionId = promo.Id,
                            ProductId = p,
                            PercentDiscount = promoDetail.PercentDiscount,
                            Price = promoDetail.Price,
                            PriceType = promoDetail.PriceType,
                            ValueDiscount = promoDetail.ValueDiscount
                        }));
            }

            return this.GetSkuBasedProductPromotion(newPromoDetailsHierarchy, promos, allPromoLocations, locations);
        }

        private List<PromotionProductModel> GetSkuBasedProductPromotion(
            List<Blue.Cosacs.Merchandising.PromotionDetail> promoDetails,
            List<Blue.Cosacs.Merchandising.Promotion> promos,
            List<IGrouping<int, int>> allPromoLocations,
            List<Location> locations)
        {
            var results = new List<PromotionProductModel>();

            var promoDetailsSku = promoDetails.Where(pd => pd.ProductId.HasValue).GroupBy(pd => new { pd.PromotionId, ProductId = pd.ProductId.Value }).ToList();
            var productIds = promoDetailsSku.Select(p => p.Key.ProductId).Distinct();

            List<Product> products;
            List<CurrentCostPriceView> costs;
            List<RetailPriceView> retailPrices;
            using (var scope = Context.Read())
            {
                products = scope.Context.Product.Where(p => productIds.Contains(p.Id)).ToList();
                costs = scope.Context.CurrentCostPriceView.Where(p => productIds.Contains(p.ProductId)).ToList();
                retailPrices = scope.Context.RetailPriceView.Where(rp => productIds.Contains(rp.ProductId)).ToList();
            }

            foreach (var promoDetail in promoDetailsSku)
            {
                var promoProduct = products.First(p => p.Id == promoDetail.Key.ProductId);
                var promo = promos.Single(p => p.Id == promoDetail.Key.PromotionId);
                var promoLocations = allPromoLocations.Single(l => l.Key == promo.Id);

                foreach (var location in promoLocations)
                {
                    // lookup the price at the promo fascia/location not by the "effective location" (if using fascia)
                    decimal cashPrice = 0, regularPrice = 0, dutyFreePrice = 0, taxRate = 0, cost = 0;
                    var prices = this.retailPriceRepository.GetForProductLocationEffectiveDate(location, null, promoProduct.Id, promo.StartDate, retailPrices, locations);

                    if (prices != null)
                    {
                        cashPrice = prices.CashPrice ?? 0;
                        regularPrice = prices.RegularPrice ?? 0;
                        dutyFreePrice = prices.DutyFreePrice ?? 0;
                        taxRate = prices.TaxRate ?? 0;
                    }

                    var currentCost = costs.FirstOrDefault(c => c.ProductId == promoProduct.Id);
                    if (currentCost != null)
                    {
                        cost = currentCost.AverageWeightedCost;
                    }

                    var product = new PromotionProductModel
                    {
                        PromotionId = promo.Id,
                        PromotionName = promo.Name,
                        LongDescription = promoProduct.LongDescription,
                        AverageWeightedCost = cost,
                        StartDate = promo.StartDate,
                        EndDate = promo.EndDate,
                        LocationId = location,
                        ProductId = promoProduct.Id,
                        PromotionLocation = promo.LocationId != null ? PromotionLocation.Location : promo.Fascia != null ? PromotionLocation.Fascia : PromotionLocation.All,
                        Fascia = locations.Where(l => l.Id == location).FirstOrDefault().Fascia
                    };

                    //If location price exists, don't override with any other promo.  If Fascia Price exists, don't override with ALL location promo
                    if ((product.PromotionLocation != PromotionLocation.Location && prices != null && prices.LocationId != null)
                     || (product.PromotionLocation == PromotionLocation.All && prices != null && prices.Fascia != null))
                    {
                        continue;
                    }

                    foreach (var promoPriceType in promoDetail)
                    {
                        this.ApplyPromotionType(promo, promoProduct, promoPriceType, cashPrice, regularPrice, dutyFreePrice, taxRate, product);
                    }

                    var decimals = this.settings.DecimalPlaces;

                    product.CashPrice = decimal.Round(product.CashPrice.GetValueOrDefault(0), decimals);
                    product.RegularPrice = decimal.Round(product.RegularPrice.GetValueOrDefault(0), decimals);
                    product.DutyFreePrice = decimal.Round(product.DutyFreePrice.GetValueOrDefault(0), decimals);

                    results.Add(product);
                }
            }
            return results;
        }

        private void ApplyPromotionType(
           Blue.Cosacs.Merchandising.Promotion promo,
           Product promoProduct,
           Blue.Cosacs.Merchandising.PromotionDetail promoPriceType,
           decimal cashPrice,
           decimal regularPrice,
           decimal dutyFreePrice,
           decimal taxRate,
           PromotionProductModel product)
        {
            var applicablePromo = new ApplicablePromo
            {
                PercentDiscount = promoPriceType.PercentDiscount,
                Price = promoPriceType.Price,
                PriceType = promoPriceType.PriceType,
                ValueDiscount = promoPriceType.ValueDiscount,
                EndDate = promo.EndDate,
                StartDate = promo.StartDate
            };

            var pricing = new ProductPrice
            {
                NormalCashPrice = cashPrice,
                NormalRegularPrice = regularPrice,
                NormalDutyFreePrice = dutyFreePrice,
                TaxRate = taxRate
            };
            var appliedPromo = this.ApplyPromotion(applicablePromo, pricing);

            if (promoProduct.ProductType == ProductTypes.Set)
            {
                decimal componentsCashPrice = 0, componentsRegularPrice = 0, componentsDutyFreePrice = 0;

                using (var scope = Context.Read())
                {
                    var components = scope.Context.SetProduct.Where(s => s.SetId == promoProduct.Id).ToList();

                    foreach (var component in components)
                    {
                        var componentPrices = this.retailPriceRepository.GetForProductLocationEffectiveDate(
                            promo.LocationId,
                            promo.Fascia,
                            component.ProductId,
                            DateTime.Now); //Winforms uses the current price for the component and applies the discount
                        //as long as we calculate promo price correctly, the discount should be off current component price
                        componentsCashPrice += TaxHelper.ApplyTax(componentPrices.CashPrice.GetValueOrDefault(0), componentPrices.TaxRate, merchandiseSettings.TaxInclusive) * component.Quantity;
                        componentsRegularPrice += TaxHelper.ApplyTax(componentPrices.RegularPrice.GetValueOrDefault(0), componentPrices.TaxRate, merchandiseSettings.TaxInclusive) * component.Quantity;
                        componentsDutyFreePrice += TaxHelper.ApplyTax(componentPrices.DutyFreePrice.GetValueOrDefault(0), componentPrices.TaxRate, merchandiseSettings.TaxInclusive) * component.Quantity;
                    }

                    if (appliedPromo.PromoCashPrice.HasValue)
                    {
                        product.CashPrice = appliedPromo.PromoCashPrice.Value == 0 ? 0 : appliedPromo.PromoCashPrice - componentsCashPrice;
                    }

                    if (appliedPromo.PromoRegularPrice.HasValue)
                    {
                        product.RegularPrice = appliedPromo.PromoRegularPrice.Value == 0 ? 0 : appliedPromo.PromoRegularPrice - componentsRegularPrice;
                    }

                    if (appliedPromo.PromoDutyFreePrice.HasValue)
                    {
                        product.DutyFreePrice = appliedPromo.PromoDutyFreePrice.Value == 0 ? 0 : appliedPromo.PromoDutyFreePrice - componentsDutyFreePrice;
                    }
                }
            }
            else
            {
                if (applicablePromo.PriceType == "Cash")
                {
                    product.CashPrice = appliedPromo.PromoCashPrice.HasValue ? appliedPromo.PromoCashPrice.Value : product.CashPrice;
                }

                if (applicablePromo.PriceType == "Regular")
                {
                    product.RegularPrice = appliedPromo.PromoRegularPrice.HasValue ? appliedPromo.PromoRegularPrice.Value : product.RegularPrice;
                }

                if (applicablePromo.PriceType == "DutyFree")
                {
                    product.DutyFreePrice = appliedPromo.PromoDutyFreePrice.HasValue ? appliedPromo.PromoDutyFreePrice.Value : product.DutyFreePrice;
                }
            }
        }

        public List<int> CalculateEffectedProducts(Dictionary<int, int> promotionHierarchy, IQueryable<ProductHierarchyView> query)
        {
            // build hierarchy query
            var hierarchy = promotionHierarchy.Select(h => new { LevelId = h.Key, TagId = h.Value }).ToList();

            if (hierarchy.Any())
            {
                // start with empty query
                var removeProducts = query.Where(p => false);

                // all level ids specified in the query
                var levelIds = hierarchy.Select(h => h.LevelId);

                // find products where not all queried levels have a tag 
                removeProducts = query.GroupBy(p => p.ProductId).Where(g => !levelIds.All(l => g.Any(p => p.LevelId == l))).Select(g => g.FirstOrDefault()).Where(p => p != null);

                // find products where level's tags don't match the query
                hierarchy.ForEach(l => { removeProducts = removeProducts.Union(query.Where(p => p.LevelId == l.LevelId && p.TagId != l.TagId)); });

                var removeProductIds = removeProducts.Select(p => p.ProductId).Distinct();

                query = query.Where(p => !removeProductIds.Contains(p.ProductId));
            }
            return query.Select(q => q.ProductId).ToList();
        }
    }
}