namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IBuyerSalesHistoryRepository
    {
        BuyerSalesHistoryViewModel BuyerSalesHistoryReport(BuyerSalesHistorySearchModel search);

        List<BuyerSalesHistoryExportModel> BuyerSalesHistoryExport(BuyerSalesHistorySearchModel search);
    }

    public class BuyerSalesHistoryRepository : IBuyerSalesHistoryRepository
    {
        private readonly IMerchandisingHierarchyRepository merchandisingHierarchyRepository;

        public BuyerSalesHistoryRepository(IMerchandisingHierarchyRepository merchandisingHierarchyRepository)
        {
            this.merchandisingHierarchyRepository = merchandisingHierarchyRepository;
        }

        public BuyerSalesHistoryViewModel BuyerSalesHistoryReport(BuyerSalesHistorySearchModel search)
        {
            return FilterReport(search);
        }

        public List<BuyerSalesHistoryExportModel> BuyerSalesHistoryExport(BuyerSalesHistorySearchModel search)
        {
            var export = new List<BuyerSalesHistoryExportModel>();
            var report = FilterReport(search);
            foreach (var item in report.Products)
            {
                var exportItem = Mapper.Map<BuyerSalesHistoryExportModel>(item);
                export.Add(exportItem);
            }

            return export;
        }

        private BuyerSalesHistoryViewModel FilterReport(BuyerSalesHistorySearchModel search)
        {
            using (var scope = Context.Read())
            {
                var report = scope.Context.BuyerSalesHistoryReportView
                    .Select(p => p);

                if (search.LocationId.HasValue)
                {
                    report = report
                        .Where(p => p.LocationId == search.LocationId.Value);
                }

                if (!string.IsNullOrEmpty(search.Fascia))
                {
                    report = report
                        .Where(p => (p.Fascia == search.Fascia || p.Warehouse));
                }

                if (!string.IsNullOrEmpty(search.ProductType))
                {
                    report = report
                        .Where(p => p.ProductType == search.ProductType);
                }

                var productIds = new List<int>();
                // Limit by product hierarchy
                if (search.Hierarchy != null)
                {
                    var remainingIds = scope.Context.ProductHierarchyView.AsQueryable();

                    foreach (var level in search.Hierarchy)
                    {
                        var lvl = int.Parse(level.Key);
                        remainingIds = remainingIds.Where(p => p.LevelId == lvl && p.Tag == level.Value);
                    }

                    productIds = remainingIds.Select(p => p.ProductId).ToList();
                }

                var reportList = report
                    .Where(o => productIds.Contains(o.ProductId))
                    .ToList();

                var levels = merchandisingHierarchyRepository.GetAllLevels().ToList();
                var locationIds = reportList.Select(l => l.LocationId).Distinct();

                var buyerSales = scope.Context.BuyerSalesHistoryView
                    .Where(p => productIds.Contains(p.ProductId) && locationIds.Contains(p.LocationId))
                    .ToList();

                var sales = MapSales(buyerSales, search);

                if (search.TaxInclusive.HasValue && search.TaxInclusive.Value)
                {
                    reportList = reportList.Select(s =>
                    {
                        s.CashPrice = s.CashPrice * (1 + s.taxrate);
                        return s;
                    }).ToList();
                }

                var products = MapToViewModel(reportList, sales, levels, search.Kpi);
                return new BuyerSalesHistoryViewModel
                {
                    Query = search,
                    Products = products
                };
            }
        }

        private List<BuyerSalesHistoryProductViewModel> MapToViewModel(List<BuyerSalesHistoryReportView> view, List<BuyerSalesHistoryKpiModel> sales, List<Level> levels, string kpi)
        {
            Func<IEnumerable<BuyerSalesHistoryReportView>, decimal?> getCashPrice = (saleList) =>
            {
                if (saleList == null)
                {
                    return 0;
                }
                var prices = saleList.GroupBy(v => v.CashPrice).OrderByDescending(g => g.Count()).FirstOrDefault();
                return prices == null ? 0 : prices.Key;
            };

            Func<string, Dictionary<string, decimal>, decimal> getSales = (querySale, salesDic) =>
            {
                return salesDic.ContainsKey(querySale) ? salesDic[querySale] : 0;
            };

            var salesValues = sales.GroupBy(g => string.Format("{0}-{1}-{2}", g.ProductId, g.NumericMonth, g.NumericYear))
                                   .ToDictionary(d => d.Key, d => d.Sum(s => s.Value));

            return view.GroupBy(p => new { p.ProductId, p.Year, p.NumericYear, p.Sku, p.LongDescription, p.BrandName, p.Vendor })
                .Select(p =>
                     {
                         if (!p.Any())
                         {
                             return null;
                         }

                         decimal yearToDate;
                         switch (kpi)
                         {
                             case "Margin Percentage":
                                 //Get the average margin percentage for the amount of months passed in the financial year
                                 var monthsPassed = p.Key.Year == "Last Year" ? 12 : DateTime.Now.AddMonths(-3).Month;
                                 yearToDate = sales.Where(s => p.Key.ProductId == s.ProductId && p.Key.NumericYear == s.NumericYear).Sum(s => s.Value) / monthsPassed;
                                 break;
                             default:
                                 yearToDate = sales.Where(s => p.Key.ProductId == s.ProductId && p.Key.NumericYear == s.NumericYear).Sum(s => s.Value);
                                 break;
                         }

                         return new BuyerSalesHistoryProductViewModel
                         {
                             ProductId = p.Key.ProductId,
                             Sku = p.Key.Sku,
                             Description = p.Key.LongDescription,
                             Brand = p.Key.BrandName,
                             Vendor = p.Key.Vendor,
                             Year = p.Key.Year,
                             StockOnHand = p.Sum(x => x.StockOnHand ?? 0),
                             StockOnOrder = p.Sum(x => x.StockOnOrder ?? 0),
                             AverageWeightedCost = p.Max(m => m.AverageWeightedCost),
                             CashPrice = getCashPrice(p.Where(c => c.CashPrice != null)),
                             April = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 4, p.Key.NumericYear), salesValues),
                             May = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 5, p.Key.NumericYear), salesValues),
                             June = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 6, p.Key.NumericYear), salesValues),
                             July = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 7, p.Key.NumericYear), salesValues),
                             August = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 8, p.Key.NumericYear), salesValues),
                             September = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 9, p.Key.NumericYear), salesValues),
                             October = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 10, p.Key.NumericYear), salesValues),
                             November = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 11, p.Key.NumericYear), salesValues),
                             December = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 12, p.Key.NumericYear), salesValues),
                             January = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 1, p.Key.NumericYear), salesValues),
                             February = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 2, p.Key.NumericYear), salesValues),
                             March = getSales(string.Format("{0}-{1}-{2}", p.Key.ProductId, 3, p.Key.NumericYear), salesValues),
                             YearToDate = yearToDate
                         };
                     }).ToList();
        }

        private List<BuyerSalesHistoryKpiModel> MapSales(List<BuyerSalesHistoryView> sales, BuyerSalesHistorySearchModel search)
        {
            IEnumerable<BuyerSalesHistoryKpiModel> mapped = null;
            switch (search.Kpi)
            {
                case "Sales Value":
                    if (search.TaxInclusive.HasValue && search.TaxInclusive.Value)
                    {
                        sales = sales.Select(s =>
                            {
                                s.SalesValue = s.SalesValue + s.Tax;
                                return s;
                            }).ToList();
                    }
                    if (search.DiscountInclusive.HasValue && search.DiscountInclusive.Value)
                    {
                        sales = sales.Select(s =>
                        {
                            s.SalesValue = s.SalesValue + s.Discount;
                            return s;
                        }).ToList();
                    }

                    mapped = sales.GroupBy(s => new { s.ProductId, s.NumericMonth, s.NumericYear, s.Year, s.Month })
                                .Select(s =>
                                {
                                    var group = s.First();
                                    return new BuyerSalesHistoryKpiModel
                                    {
                                        Year = s.Key.Year,
                                        NumericYear = s.Key.NumericYear,
                                        Month = s.Key.Month,
                                        NumericMonth = s.Key.NumericMonth,
                                        ProductId = s.Key.ProductId,
                                        Value = s.Sum(p => p.SalesValue.HasValue ? p.SalesValue.Value : 0)
                                    };
                                });
                    break;

                case "Sales Volume":
                    mapped = sales.GroupBy(s => new { s.ProductId, s.NumericMonth, s.NumericYear, s.Year, s.Month })
                                .Select(s =>
                                {
                                    return new BuyerSalesHistoryKpiModel
                                    {
                                        Year = s.Key.Year,
                                        NumericYear = s.Key.NumericYear,
                                        Month = s.Key.Month,
                                        NumericMonth = s.Key.NumericMonth,
                                        ProductId = s.Key.ProductId,
                                        Value = s.Sum(p => p.SalesVolume.HasValue ? p.SalesVolume.Value : 0)
                                    };
                                });
                    break;

                case "Margin Value":
                    mapped = sales.GroupBy(s => new { s.ProductId, s.NumericMonth, s.NumericYear, s.Year, s.Month })
                                .Select(s =>
                                {
                                    var group = s.First();
                                    return new BuyerSalesHistoryKpiModel
                                    {
                                        Year = s.Key.Year,
                                        NumericYear = s.Key.NumericYear,
                                        Month = s.Key.Month,
                                        NumericMonth = s.Key.NumericMonth,
                                        ProductId = s.Key.ProductId,
                                        Value = s.Sum(p => (p.SalesValue.HasValue ? p.SalesValue.Value : 0) - (p.TotalCost.HasValue ? p.TotalCost.Value : 0))
                                    };
                                });
                    break;

                case "Margin Percentage":
                    mapped = sales.GroupBy(s => new { s.ProductId, s.NumericMonth, s.NumericYear, s.Year, s.Month })
                                .Select(s =>
                                {
                                    return new BuyerSalesHistoryKpiModel
                                    {
                                        Year = s.Key.Year,
                                        NumericYear = s.Key.NumericYear,
                                        Month = s.Key.Month,
                                        NumericMonth = s.Key.NumericMonth,
                                        ProductId = s.Key.ProductId,
                                        Value = s.Sum(p => !p.SalesValue.HasValue ? 0 : p.SalesValue.Value) == 0 ? 0
                                                : s.Sum(p => !p.SalesValue.HasValue ? 0 :
                                                                (p.SalesValue.Value - (p.TotalCost.HasValue ? p.TotalCost.Value : 0)))
                                                                / s.Sum(p => !p.SalesValue.HasValue ? 0 : p.SalesValue.Value)
                                    };
                                });
                    break;

                default:
                    return null;
            }

            return mapped
                .OrderBy(p => p.ProductId)
                .ThenBy(p => p.NumericYear)
                .ToList();
        }
    }
}