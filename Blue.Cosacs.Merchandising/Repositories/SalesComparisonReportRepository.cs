namespace Blue.Cosacs.Merchandising.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Blue.Cosacs.Merchandising.Models;
    using AutoMapper;
    using Newtonsoft.Json;
    using System.IO;
    using System;
    using Blue.Cosacs.Merchandising.Enums;

    public interface ISalesComparisonReportRepository
    {
        List<SalesComparisonGroupViewModel> GroupedSalesComparisonReport(SalesComparisonSearchModel search);
        List<SalesComparisonViewModel> SalesComparisonReport(SalesComparisonSearchModel search);
    }

    public class SalesComparisonReportRepository : ISalesComparisonReportRepository
    {
        private readonly IPromotionRepository promoRepository;

        public SalesComparisonReportRepository(IPromotionRepository promoRepository)
        {
            this.promoRepository = promoRepository;
        }

        public List<SalesComparisonGroupViewModel> GroupedSalesComparisonReport(SalesComparisonSearchModel search)
        {
            return CalculateTotals(SalesComparisonReport(search));
        }

        public List<SalesComparisonViewModel> SalesComparisonReport(SalesComparisonSearchModel search)
        {
            var filteredResults = FilterRecords(search);
            if (filteredResults.Any())
            {
                var report = Mapper.Map<List<SalesComparisonViewModel>>(filteredResults);
                var promotions = promoRepository.GetCurrentPromotions()
                    .Where(p => p.StartDate.Date <= DateTime.Today.Date)
                    .ToList();

                using (var scope = ReportingContext.Read())
                {
                    var skus = report.Select(r => r.Sku).ToList();
                    var saleLocations = report.Select(r => r.SalesId).ToList();
                    var cintOrderCost = scope.Context.CintOrderCostView
                        .Where(v => skus.Contains(v.Sku) && saleLocations.Contains(v.SaleLocation)
                                    && (v.Type == CintOrderType.Delivery || v.Type == CintOrderType.Return))
                        .ToList();

                    report.ForEach(r => HydrateSalesRecord(r, promotions, cintOrderCost, search));
                    return report;
                }
            }
            return new List<SalesComparisonViewModel>();
        }

        private List<SalesComparisonGroupViewModel> CalculateTotals(List<SalesComparisonViewModel> report)
        {
            var groups = report
                .GroupBy(r => new { r.LocationName, r.Fascia })
                .Select(loc => new SalesComparisonGroupViewModel()
                {
                    Name = string.Format("{0} {1}", loc.Key.LocationName, loc.Key.Fascia),
                    ThisYearTotals = GetComparisonSalesTotals(loc.Select(l => l.ThisYear).ToList()),
                    LastYearTotals = GetComparisonSalesTotals(loc.Select(l => l.LastYear).ToList()),
                    Children = loc.GroupBy(div => div.Division)
                        .Select(div => new SalesComparisonGroupViewModel()
                        {
                            Name = div.Key ?? "All",
                            ThisYearTotals = GetComparisonSalesTotals(div.Select(l => l.ThisYear).ToList()),
                            LastYearTotals = GetComparisonSalesTotals(div.Select(l => l.LastYear).ToList()),
                            Children = div.GroupBy(dept => dept.Department)
                                .Select(dept => new SalesComparisonGroupViewModel()
                                {
                                    Name = dept.Key ?? "All",
                                    ThisYearTotals = GetComparisonSalesTotals(dept.Select(l => l.ThisYear).ToList()),
                                    LastYearTotals = GetComparisonSalesTotals(dept.Select(l => l.LastYear).ToList()),
                                    Children = dept.GroupBy(cls => cls.Class)
                                        .Select(cls => new SalesComparisonGroupViewModel()
                                        {
                                            Name = cls.Key ?? "All",
                                            ThisYearTotals = GetComparisonSalesTotals(cls.Select(l => l.ThisYear).ToList()),
                                            LastYearTotals = GetComparisonSalesTotals(cls.Select(l => l.LastYear).ToList()),
                                            Items = cls.ToList()
                                        }).ToList()
                                }).ToList()
                        }).ToList()
                })
                .ToList();
            return groups;
        }

        private void HydrateSalesRecord(SalesComparisonViewModel record, List<PromotionProductModel> promotions, List<CintOrderCostView> cintOrderCost, SalesComparisonSearchModel search)
        {
            // Get promo prices
            var currentPromotion = promotions.FirstOrDefault(p => p.ProductId == record.ProductId && p.LocationId == record.LocationId);
            if (currentPromotion != null)
            {
                record.PromotionalCashPrice = currentPromotion.CashPrice.HasValue ? currentPromotion.CashPrice.Value : 0;
                record.PromotionalRegularPrice = currentPromotion.RegularPrice.HasValue ? currentPromotion.RegularPrice.Value : 0;
            }

            // Calculate date ranges
            var thisYear = search.FiscalYear ? (search.PeriodEnd.Month >= 4 ? search.PeriodEnd.Year : search.PeriodEnd.Year - 1) : search.PeriodEnd.Year;
            var lastYear = thisYear - 1;
            var thisYearStart = search.FiscalYear ? new DateTime(thisYear, 4, 1) : new DateTime(thisYear, 1, 1);
            var lastYearStart = search.FiscalYear ? new DateTime(lastYear, 4, 1) : new DateTime(lastYear, 1, 1);
            var thisYearFinish = search.PeriodEnd;
            var lastYearFinish = search.PeriodEnd.AddYears(-1);

            // Gets sales data for date ranges
            var sales = cintOrderCost.Where(
                    d =>
                    d.Sku == record.Sku && d.SaleLocation == record.SalesId
                    && (d.Type == CintOrderType.Delivery || d.Type == CintOrderType.Return)).ToList();
            var thisYearSales = sales.Where(
                    d =>
                    d.TransactionDate.ToLocalTime().Date >= thisYearStart
                    && d.TransactionDate.ToLocalTime().Date <= thisYearFinish).ToList();
            var lastYearSales = sales.Where(
                    d =>
                    d.TransactionDate.ToLocalTime().Date >= lastYearStart
                    && d.TransactionDate.ToLocalTime().Date <= lastYearFinish).ToList();

            record.ThisYear = GetComparisonSalesModel(thisYearSales);
            record.LastYear = GetComparisonSalesModel(lastYearSales);

            // Add tags
            record.Tags = string.IsNullOrEmpty(record.Tags) ? 
                null : 
                string.Join(
                ", ", 
                (List<string>)new JsonSerializer().Deserialize(new StringReader(record.Tags), typeof(List<string>)));
        }

        private SalesComparisonSalesModel GetComparisonSalesModel(List<CintOrderCostView> sales)
        {
            var model = new SalesComparisonSalesModel
            {
                Quantity = sales.Sum(x => x.Quantity),
                GrossValue = sales.Sum(x => x.CashPrice.Value * x.Quantity),
                GrossMarginValue = sales.Sum(x => (x.CashPrice.Value - (x.AverageWeightedCost ?? 0)) * x.Quantity),
                NetValue = sales.Sum(x => (x.CashPrice.Value * x.Quantity) + (x.Discount ?? 0)),
                NetMarginValue = sales.Sum(x => (x.CashPrice.Value* x.Quantity) + (x.Discount ?? 0) - ((x.AverageWeightedCost ?? 0)* x.Quantity))
            };

            model.GrossMarginPercent = model.GrossMarginValue / (model.GrossValue > 0 ? model.GrossValue : 1);
            model.NetMarginPercent = model.NetMarginValue / (model.NetValue > 0 ? model.NetValue : 1);
            return model;
        }

        private SalesComparisonSalesModel GetComparisonSalesTotals(List<SalesComparisonSalesModel> sales)
        {
            var model = new SalesComparisonSalesModel
                            {
                                Quantity = sales.Sum(x => x.Quantity),
                                GrossValue = sales.Sum(x => x.GrossValue),
                                GrossMarginValue = sales.Sum(x => x.GrossMarginValue),
                                NetValue = sales.Sum(x => x.NetValue),
                                NetMarginValue = sales.Sum(x => x.NetMarginValue),
                                GrossMarginPercent =
                                    sales.Sum(x => x.GrossMarginPercent) / sales.Count(),
                                NetMarginPercent =
                                    sales.Sum(x => x.NetMarginPercent) / sales.Count()
                            };

            return model;
        }

        private List<SalesComparisonReportView> FilterRecords(SalesComparisonSearchModel search)
        {
            using (var scope = ReportingContext.Read())
            {
                //Get All Products
                var report = scope.Context.SalesComparisonReportView
                    .Where(r => ((search.LocationId != null && r.LocationId == search.LocationId)
                        || (search.Fascia != null && r.Fascia == search.Fascia)
                        || (search.Fascia == null && search.LocationId == null))
                        && (search.BrandId == null || r.BrandId == search.BrandId));

                // Limit by product hierarchy
                if (search.Hierarchy != null)
                {
                    var productIds = report.Select(o => o.ProductId).ToList();

                    foreach (var level in search.Hierarchy)
                    {
                        var lvlKey = int.Parse(level.Key);
                        var lvlValue = int.Parse(level.Value);

                        var remainingIds =
                            scope.Context.ProductHierarchyView
                            .Where(p => productIds.Contains(p.ProductId) 
                                && p.LevelId == lvlKey
                                && p.TagId == lvlValue)
                            .Select(p => p.ProductId);
                        report = report.Where(o => remainingIds.Contains(o.ProductId));
                    }
                }

                // Limit by tags 
                if (search.Tags != null)
                {
                    report = report
                        .Where(o => o.Tags != null &&
                                           ((List<string>)
                                            new JsonSerializer().Deserialize(
                                                new StringReader(o.Tags),
                                                typeof(List<string>))).Any(t => search.Tags.Any(s => s == t)));
                }
                
                return report.ToList();
            }
        }
    }
}