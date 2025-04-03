namespace Blue.Cosacs.Merchandising.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Blue.Cosacs.Merchandising.Models;
    using AutoMapper;
    using System;
    using Blue.Cosacs.Merchandising.Infrastructure;
    using Blue.Cosacs.Merchandising;

    public interface IPromotionReportRepository
    {
        PagedSearchResult<PromotionReportViewModel> PromotionReport(PromotionReportSearchModel search, int pageSize = Int32.MaxValue, int pageIndex = 0);
        PromotionReportPrintModel PromotionReportPrint(PromotionReportSearchModel search);
    }

    public class PromotionReportRepository : IPromotionReportRepository
    {
        private readonly IMerchandisingHierarchyRepository merchHierarchyRepository;
        private readonly IPromotionRepository promoRepository;
        private readonly ILocationRepository locationRepository;
        private readonly Blue.Cosacs.Merchandising.Settings merchandiseSettings;

        public PromotionReportRepository(IMerchandisingHierarchyRepository merchHierarchyRepository, IPromotionRepository promoRepository, ILocationRepository locationRepository, Blue.Cosacs.Merchandising.Settings merchandiseSettings)
        {
            this.merchHierarchyRepository = merchHierarchyRepository;
            this.promoRepository = promoRepository;
            this.locationRepository = locationRepository;
            this.merchandiseSettings = merchandiseSettings;
        }

        public PromotionReportPrintModel PromotionReportPrint(PromotionReportSearchModel search)
        {
            if (search.LocationId != null)
            {
                search.LocationName = locationRepository.Get(search.LocationId.Value).Name;
            }
            else
            {
                search.LocationName = "Any";
            }
            return new PromotionReportPrintModel 
            {
                Results = PromotionReport(search),
                Query = search
            };
        }

        public PagedSearchResult<PromotionReportViewModel> PromotionReport(PromotionReportSearchModel search, int pageSize = Int32.MaxValue, int pageIndex = 0)
        {
            using (var scope = ReportingContext.Read())
            {
                var allPromotions = search.PromotionIds == null ? promoRepository.GetAllProductsForPromotion() : promoRepository.GetAllProductsForPromotion(search.PromotionIds);

                var q = allPromotions
                        .Where(r =>
                            ((search.LocationId != null && r.LocationId == search.LocationId)
                             || (search.Fascia != null && r.Fascia == search.Fascia)
                             || (search.Fascia == null && search.LocationId == null)));
                         
                var promotions = q.OrderBy(x => x.PromotionId)
                                .Skip(pageSize * pageIndex).Take(pageSize)
                                .ToList();

                //Get All Products
                var sales = scope.Context.PromotionReportView.AsNoTracking().ToList();
                var productIds = promotions.Select(p => p.ProductId).ToList();
                var allPrices = scope.Context.RetailPriceView.Where(rp => productIds.Contains(rp.ProductId)).ToList();
                var allLocations = scope.Context.Location.Select(l => l).ToList();

                var report = promotions.Select(p =>
                    {
                        var inclusive = merchandiseSettings.TaxInclusive;
                        var productSales = sales.FirstOrDefault(s => s.ProductId == p.ProductId && s.LocationId == p.LocationId && s.PromotionId == p.PromotionId);
                        var retailPrice = CalculateRetailPrice(
                            p.LocationId,
                            p.ProductId,
                            p.StartDate,
                            allPrices,
                            allLocations);

                        if (productSales == null)
                        {
                            var taxRate = retailPrice == null ? 0 : retailPrice.TaxRate;
                            var price = p.CashPrice;
                            var margin = 1 - (p.AverageWeightedCost / Math.Max(p.CashPrice.GetValueOrDefault(1), 1));
                            if (inclusive)
                            {
                                price = p.CashPrice / (1 + taxRate);
                                margin = 1 - (p.AverageWeightedCost / (Math.Max(p.CashPrice.GetValueOrDefault(1), 1) / (1 + taxRate)));
                            }
 
                            productSales = new PromotionReportView
                            {
                                Id = 0,
                                ProductId = p.ProductId,
                                LocationId = p.LocationId,
                                PromotionId = p.PromotionId,
                                PromotionStartDate = p.StartDate,
                                Quantity = 0,
                                Price = price,
                                Discount = 0,
                                GrossTotal = 0,
                                NetTotal = 0,
                                AverageWeightedCost = 0,
                                PromotionalMargin = price <= 0 ? 0 : margin
                            };
                        }
 
                        decimal cashPrice = 0.00M;
                        decimal cashTotal = 0.00M;
                        decimal cashMargin = 0.00M;

                        if (retailPrice != null)
                        {
                            cashPrice = retailPrice.CashPrice ?? 0;
                            cashTotal = cashPrice * (productSales == null ? 0 : productSales.Quantity.Value);
                            cashMargin = (cashPrice - (p.AverageWeightedCost ?? 0)) / Math.Max(cashPrice, 1);
                        }

                        return new PromotionReportViewModel
                            {
                                  ProductId = p.ProductId, 
                                  LocationId = p.LocationId,
                                  LocationName = allLocations.FirstOrDefault(l => l.Id == p.LocationId).Name,  
                                  Fascia = p.Fascia,
                                  Sku = p.Sku,
                                  LongDescription = p.LongDescription,
                                  Quantity  = productSales.Quantity.Value,
                                  PromotionStartDate = p.StartDate,
                                  PromotionalTotal = productSales.PromotionalTotal,
                                  Price = productSales.Price,
                                  Discount = productSales.Discount,
                                  GrossTotal = productSales.GrossTotal,
                                  NetTotal = productSales.NetTotal,
                                  AverageWeightedCost = p.AverageWeightedCost,
                                  PromotionalMargin = productSales.PromotionalMargin,
                                  PromotionId = p.PromotionId,
                                  PromotionName  = p.PromotionName,
                                  CashPrice = cashPrice,
                                  CashTotal = cashTotal,
                                  CashMargin  = cashMargin
                            };
                    }).ToList();

                return new PagedSearchResult<PromotionReportViewModel> { Page = report, Count = q.Count() };
            }
        }

        private RetailPriceView CalculateRetailPrice(int locationId, int productId, DateTime effectiveDate, List<RetailPriceView> retailPrices, List<Location> allLocations)
        {
            var allPrices = retailPrices.Where(rp => rp.ProductId == productId).ToList();
            string locationFascia = null;

            locationFascia = allLocations.First(l => l.Id == locationId).Fascia;
            
            var thisLocation = allPrices.Where(p => p.LocationId == locationId && p.EffectiveDate <= effectiveDate).ToList();
            if (thisLocation.Any())
            {
                return thisLocation.OrderBy(p => p.EffectiveDate).Last();
            }

            var thisFascia = allPrices.Where(p => p.Fascia == locationFascia && locationFascia != null && p.EffectiveDate <= effectiveDate).ToList();
            if (thisFascia.Any())
            {
                return thisFascia.OrderBy(p => p.EffectiveDate).Last();
            }

            var anyLocation = allPrices.Where(p => p.LocationId == null && p.Fascia == null && p.EffectiveDate <= effectiveDate).ToList();
            if (anyLocation.Any())
            {
                return anyLocation.OrderBy(p => p.EffectiveDate).Last();
            }

            return null;
        }
    }
}