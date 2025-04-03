using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;

    using Blue.Cosacs.Merchandising.DataWarehousing;
    using Blue.Cosacs.Merchandising.DataWarehousing.Models;
    using Blue.Cosacs.Merchandising.Models;
    using Percolator.AnalysisServices;
    using Percolator.AnalysisServices.Linq;

    using InventoryModel = Blue.Cosacs.Merchandising.DataWarehousing.Models.InventoryModel;

    public interface ITradingReportRepository
    {
        List<SalesReport> GetMerchandiseSales(DateTime date);

        List<SalesReport> GetDiscounts(DateTime date);

        List<SalesReport> GetWarranty(DateTime date);

        List<SalesReport> GetAffinity(DateTime date);

        List<SalesReport> GetCharges(DateTime date);

        SalesReport GetLoanDisbursement(DateTime date);

        List<SalesReport> GetMerchandisingAccountSales(DateTime date);

        List<InventoryReport> GetInventory();
    }

    public class TradingReportRepository : ITradingReportRepository
    {
        private readonly CosacsDW cosacsDW;

        public TradingReportRepository(CosacsDW cosacsDw)
        {
            this.cosacsDW = cosacsDw;
        }

        public List<SalesReport> GetMerchandiseSales(DateTime date)
        {
            var result = this.GetCategoryWithFirstAndSecondLevelDrillDown(
                "Merchandise Sales", 
                date, 
                x => x.MerchandisePrice, 
                x => x.MerchandiseGrossProfit, 
                x => x.MerchandiseSales.ProductHierarchy.CurrentMember.Parent.Name, 
                x => x.MerchandiseSales.ProductHierarchy.Division, 
                x => x.MerchandiseSales.ProductHierarchy.CurrentMember.Name, 
                x => x.MerchandiseSales.ProductHierarchy.Department);

            return result;
        }

        public List<SalesReport> GetDiscounts(DateTime date)
        {
            var result = this.GetCategoryWithFirstAndSecondLevelDrillDown(
                "Discounts", 
                date, 
                x => x.DiscountPrice, 
                x => x.DiscountGrossProfit, 
                x => x.Discounts.Discounts.CurrentMember.Parent.Name, 
                x => x.Discounts.Discounts.SaleType, 
                x => x.Discounts.Discounts.CurrentMember.Name, 
                x => x.Discounts.Discounts.Division);

            result.Add(this.GetTotalNetMerchandiseSales(date));

            return result;
        }

        public List<SalesReport> GetWarranty(DateTime date)
        {
            var result = this.GetCategoryWithFirstAndSecondLevelDrillDown(
                "Warranties", 
                date, 
                x => x.WarrantyPrice, 
                x => x.WarrantyGrossProfit, 
                x => x.WarrantySales.WarrantySales.CurrentMember.Parent.Name, 
                x => x.WarrantySales.WarrantySales.SaleType, 
                x => x.WarrantySales.WarrantySales.CurrentMember.Name, 
                x => x.WarrantySales.WarrantySales.Division);

            result.Add(this.GetTotalMerchandiseSalesPlusWarranty(date));

            return result;
        }

        public List<SalesReport> GetAffinity(DateTime date)
        {
            var result = this.GetCategoryWithFirstLevelDrillDown(
                "Affinity", 
                date, 
                x => x.AffinityPrice, 
                x => x.AffinityGrossProfit, 
                x => x.AffinitySales.AffinitySales.CurrentMember.Name, 
                x => x.AffinitySales.AffinitySales.Type);

            var merchPlusWarranty = this.GetTotalMerchandiseSalesPlusWarrantyPlusAffinity(date);

            result.Add(merchPlusWarranty);

            return result;
        }

        private SalesReport GetTotalMerchandiseSalesPlusWarrantyPlusAffinity(DateTime date)
        {
            var totalNetMerchandiseSalesPlusWarrantyPlusAffinity = this.GetTotalMerchandiseSalesPlusWarranty(date);
            var totalAffinity = this.GetSalesTotal("Affinity", date, x => x.AffinityPrice, x => x.AffinityGrossProfit);

            var merchPlusWarranty = new SalesReport
                                        {
                                            Totals =
                                                new SalesStatisticsDetails
                                                    {
                                                        Name = "Net Merchandising Sales plus Warranty plus Affinity",
                                                        IsGrandTotal = true,
                                                        Statistics =
                                                            this.Process(
                                                                totalNetMerchandiseSalesPlusWarrantyPlusAffinity.Totals.Statistics,
                                                                totalAffinity.Totals.Statistics)
                                                    }
                                        };
            return merchPlusWarranty;
        }

        public List<SalesReport> GetCharges(DateTime date)
        {
            var serviceCharges = this.GetCategoryWithFirstLevelDrillDown(
                "Service Charges", 
                date, 
                x => x.ServiceChargePrice, 
                x => x.ServiceChargeGrossProfit, 
                x => x.ServiceCharges.ServiceCharges.CurrentMember.Name, 
                x => x.ServiceCharges.ServiceCharges.Type);

            var rebates = this.GetSingleCategory("Rebates", date, x => x.RebatesPrice, x => x.RebatesGrossProfit);

            var totalRebates = this.GetSalesTotal("Rebates Total", date, x => x.RebatesPrice, x => x.RebatesGrossProfit);
            var totalServiceCharge = this.GetSalesTotal("Service Charge", date, x => x.ServiceChargePrice, x => x.ServiceChargeGrossProfit);
            
            var otherCharges = this.GetCategoryWithFirstLevelDrillDown(
                "Other Charges", 
                date, 
                x => x.OtherChargesPrice, 
                x => x.OtherChargesGrossProfit, 
                x => x.OtherCharges.OtherCharges.CurrentMember.Name, 
                x => x.OtherCharges.OtherCharges.Type);

            var netServiceCharge = new SalesReport
            {
                Totals =
                    new SalesStatisticsDetails
                    {
                        Name = "Net Service Charge",
                        Statistics =
                            this.Process(
                                totalServiceCharge.Totals.Statistics,
                                rebates.Rows.First().Statistics)
                    }
            };

            var interest = this.GetCategoryWithFirstLevelDrillDown(
                "Interest", 
                date, 
                x => x.InterestChargesPrice, 
                x => x.InterestChargesGrossProfit, 
                x => x.InterestCharges.InterestCharges.CurrentMember.Name, 
                x => x.InterestCharges.InterestCharges.Type);

            var interestTotal = this.GetSalesTotal("Interest", date, x => x.InterestChargesPrice, x => x.InterestChargesGrossProfit);

            var priceDifferential = this.GetSingleCategory("Price Differential", date, x => x.PriceDifferential, x => x.PriceDifferentialGrossProfit);

            var priceDifferentialTotal = this.GetSalesTotal("Total Price Differential", date, x => x.PriceDifferential, x => x.PriceDifferentialGrossProfit);

            var totalMerchandiseSalesPlusWarrantyPlusAffinity = this.GetTotalMerchandiseSalesPlusWarrantyPlusAffinity(date);
            var totalOtherCharges = this.GetSalesTotal("Total Other Charges", date, x => x.OtherChargesPrice, x => x.OtherChargesGrossProfit);

            var totalCharges = new SalesReport
            {
                Totals =
                    new SalesStatisticsDetails
                    {
                        Name = "Charges",
                        IsGrandTotal = true,
                        Statistics =
                            this.Process(
                                netServiceCharge.Totals.Statistics,
                                totalOtherCharges.Totals.Statistics,
                                interestTotal.Totals.Statistics,
                                priceDifferentialTotal.Totals.Statistics)
                    }
            };

            var netSales = new SalesReport
                               {
                                   Totals =
                                       new SalesStatisticsDetails
                                           {
                                               Name = "Net Sales",
                                               IsGrandTotal = true,
                                               Statistics = this.Process(totalMerchandiseSalesPlusWarrantyPlusAffinity.Totals.Statistics, totalCharges.Totals.Statistics)
                                           }
                               };

            var result = new List<SalesReport>();
            result.AddRange(serviceCharges);
            result.Add(rebates);
            result.Add(netServiceCharge);
            result.AddRange(otherCharges);
            result.AddRange(interest);
            result.Add(priceDifferential);
            result.Add(totalCharges);
            result.Add(netSales);

            return result;
        }

        public SalesReport GetLoanDisbursement(DateTime date)
        {
            return this.GetSingleCategory("Loan Disbursement", date, x => x.LoanDisbursementPrice, x => x.LoanDisbursementGrossProfit);
        }

        public List<SalesReport> GetMerchandisingAccountSales(DateTime date)
        {
            var result = this.GetCategoryWithFirstAndSecondLevelDrillDown(
                "Merchandise Account Sales", 
                date, 
                x => x.MerchandisePrice, 
                x => x.MerchandiseGrossProfit, 
                x => x.AccountSales.AccountSales.CurrentMember.Parent.Name, 
                x => x.AccountSales.AccountSales.SaleType, 
                x => x.AccountSales.AccountSales.CurrentMember.Name, 
                x => x.AccountSales.AccountSales.Division);

            return result;
        }

        public List<InventoryReport> GetInventory()
        {
            var departmentQuery = this.GetInventoryQuery().OnAxis(Axis.ROWS, inventory => inventory.ProductStock.ProductHierarchy.Department).Percolate<InventoryModel>().ToList();
            var divisionQuery = this.GetInventoryQuery().OnAxis(Axis.ROWS, inventory => inventory.ProductStock.ProductHierarchy.Division).Percolate<InventoryModel>().ToList();
            var total = this.GetInventoryQuery().Percolate<InventoryModel>().First();

            var result =
                divisionQuery.Select(
                    r =>
                    new InventoryReport
                        {
                            Totals = new InventoryStatisticsDetails { Name = r.Name, Statistics = Mapper.Map<InventoryStatistics>(r) }, 
                            Rows =
                                departmentQuery.Where(d => d.Category == r.Name)
                                .Select(d => new InventoryStatisticsDetails { Name = d.Name, Statistics = Mapper.Map<InventoryStatistics>(d) })
                                .ToList()
                        }).ToList();

            result.Add(
                new InventoryReport { Totals = new InventoryStatisticsDetails { IsGrandTotal = true, Name = "Inventory", Statistics = Mapper.Map<InventoryStatistics>(total) } });

            return result;
        }

        private IMdxQueryable<Inventory> GetInventoryQuery()
        {
            var rootSet = Set.Create<Sales>(sales => Mdx.MdxFunction<Set>("Root"));

            var q =
                this.cosacsDW.Inventory.WithSet("RootSet", null, inventory => rootSet)
                    .WithMember("StockFraction", Axis.COLUMNS, inventory => inventory.StockValue / Mdx.Sum(rootSet, inventory.StockValue))
                    .WithMember("[Name]", Axis.COLUMNS, inventory => inventory.ProductStock.ProductHierarchy.CurrentMember.Name)
                    .WithMember("ParentName", Axis.COLUMNS, inventory => inventory.ProductStock.ProductHierarchy.CurrentMember.Parent.Name)
                    .OnAxis(Axis.COLUMNS, inventory => inventory.StockValue);
            return q;
        }

        private SalesReport GetTotalMerchandiseSalesPlusWarranty(DateTime date)
        {
            var totalWarranty = this.GetSalesTotal("Warranties", date, x => x.WarrantyPrice, x => x.WarrantyGrossProfit);

            var merchPlusWarranty = new SalesReport
                                        {
                                            Totals =
                                                new SalesStatisticsDetails
                                                    {
                                                        IsGrandTotal = true, 
                                                        Name = "Net Merchandising Sales plus Warranty", 
                                                        Statistics =
                                                            this.Process(
                                                                this.GetTotalNetMerchandiseSales(date).Totals.Statistics, 
                                                                totalWarranty.Totals.Statistics)
                                                    }
                                        };
            return merchPlusWarranty;
        }

        private SalesReport GetTotalNetMerchandiseSales(DateTime date)
        {
            var totalMerchandiseSales = this.GetSalesTotal("Merchandise Sales", date, x => x.MerchandisePrice, x => x.MerchandiseGrossProfit);

            var discountTotal = this.GetSalesTotal("Discounts", date, x => x.DiscountPrice, x => x.DiscountGrossProfit);

            var totalNetMerchandiseSales = new SalesReport
                                               {
                                                   Totals =
                                                       new SalesStatisticsDetails
                                                           {
                                                               IsGrandTotal = true, 
                                                               Name = "Net Merchandise Sales", 
                                                               Statistics =
                                                                   this.Process(
                                                                       totalMerchandiseSales.Totals.Statistics, 
                                                                       discountTotal.Totals.Statistics)
                                                               // add discounts as discounts are stored with a negative value (except when they aren't!)
                                                           }
                                               };
            return totalNetMerchandiseSales;
        }

        private SalesStatistics Process(params SalesStatistics[] salesStatistics)
        {
            var stats = new SalesStatistics
                            {
                                LastYearGrossProfit = salesStatistics.Sum(s => s.LastYearGrossProfit),
                                LastYearSales = salesStatistics.Sum(s => s.LastYearSales),
                                ThisPeriodGrossProfit = salesStatistics.Sum(s => s.ThisPeriodGrossProfit),
                                ThisPeriodLastYearGrossProfit = salesStatistics.Sum(s => s.ThisPeriodLastYearGrossProfit),
                                ThisPeriodLastYearSales = salesStatistics.Sum(s => s.ThisPeriodLastYearSales),
                                ThisPeriodSales = salesStatistics.Sum(s => s.ThisPeriodSales),
                                ThisWeekGrossProfit = salesStatistics.Sum(s => s.ThisWeekGrossProfit),
                                ThisWeekLastYearGrossProfit = salesStatistics.Sum(s => s.ThisWeekLastYearGrossProfit),
                                ThisWeekLastYearSales = salesStatistics.Sum(s => s.ThisWeekLastYearSales),
                                ThisWeekSales = salesStatistics.Sum(s => s.ThisWeekSales),
                                ThisYearGrossProfit = salesStatistics.Sum(s => s.ThisYearGrossProfit),
                                ThisYearSales = salesStatistics.Sum(s => s.ThisYearSales),
                                TodayGrossProfit = salesStatistics.Sum(s => s.TodayGrossProfit),
                                TodayLastYearGrossProfit = salesStatistics.Sum(s => s.TodayLastYearGrossProfit),
                                TodayLastYearSales = salesStatistics.Sum(s => s.TodayLastYearSales),
                                TodaySales = salesStatistics.Sum(s => s.TodaySales)
                            };

            this.CalculateVariance(stats);

            return stats;
        }

        private void CalculateVariance(SalesStatistics stats)
        {
            if (stats.TodayLastYearGrossProfit > 0)
            {
                stats.VarianceDayGrossProfit = (stats.TodayGrossProfit - stats.TodayLastYearGrossProfit) / stats.TodayLastYearGrossProfit;
            }

            if (stats.TodayLastYearSales > 0)
            {
                stats.VarianceDaySales = (stats.TodaySales - stats.TodayLastYearSales) / stats.TodayLastYearSales;
            }

            if (stats.ThisWeekLastYearGrossProfit > 0)
            {
                stats.VarianceWeekGrossProfit = (stats.ThisWeekGrossProfit - stats.ThisWeekLastYearGrossProfit) / stats.ThisWeekLastYearGrossProfit;
            }
            if (stats.ThisWeekLastYearSales > 0)
            {
                stats.VarianceWeekSales = (stats.ThisWeekSales - stats.ThisWeekLastYearSales) / stats.ThisWeekLastYearSales;
            }
            if (stats.ThisPeriodLastYearGrossProfit > 0)
            {
                stats.VariancePeriodGrossProfit = (stats.ThisPeriodGrossProfit - stats.ThisPeriodLastYearGrossProfit) / stats.ThisPeriodLastYearGrossProfit;
            }
            if (stats.ThisPeriodLastYearSales > 0)
            {
                stats.VariancePeriodSales = (stats.ThisPeriodSales - stats.ThisPeriodLastYearSales) / stats.ThisPeriodLastYearSales;
            }
            if (stats.LastYearGrossProfit > 0)
            {
                stats.VarianceYearGrossProfit = (stats.ThisYearGrossProfit - stats.LastYearGrossProfit) / stats.LastYearGrossProfit;
            }
            if (stats.LastYearSales > 0)
            {
                stats.VarianceYearSales = (stats.ThisYearSales - stats.LastYearSales) / stats.LastYearSales;
            }
        }

        private SalesReport GetSingleCategory(string reportName, DateTime date, Func<Sales, Member> price, Func<Sales, Member> grossProfit)
        {
            var data = this.GetSalesQuery(date, price, grossProfit).Percolate<ReportModel>().First();

            return new SalesReport
                       {
                           Rows =
                               new List<SalesStatisticsDetails>
                                   {
                                       new SalesStatisticsDetails
                                           {
                                               Name = reportName,
                                               Statistics = Mapper.Map<SalesStatistics>(data),
                                               IsGrandTotal = false
                                           }
                                   }
                       };
        }

        private List<SalesReport> GetCategoryWithFirstLevelDrillDown(
            string reportName, 
            DateTime date, 
            Func<Sales, Member> price, 
            Func<Sales, Member> grossProfit, 
            Func<Sales, Member> categoryName, 
            Func<Sales, ICubeObject> categoryLevel)
        {
            var categoryQuery = this.GetSalesQuery(date, price, grossProfit).WithMember("RowName", Axis.COLUMNS, x => categoryName(x)).OnAxis(1, sales => categoryLevel(sales));
            var categoryData = categoryQuery.Percolate<ReportModel>().ToList();

            var result =
                categoryData.Select(
                    r =>
                    new SalesReport
                        {
                            Rows = new List<SalesStatisticsDetails> { new SalesStatisticsDetails { Name = r.CategoryName, Statistics = Mapper.Map<SalesStatistics>(r) } }
                        }).ToList();

            result.Add(this.GetSalesTotal(reportName, date, price, grossProfit));

            return result;
        }

        private List<SalesReport> GetCategoryWithFirstAndSecondLevelDrillDown(
            string reportName, 
            DateTime date, 
            Func<Sales, Member> price, 
            Func<Sales, Member> grossProfit, 
            Func<Sales, Member> categoryName, 
            Func<Sales, ICubeObject> categoryLevel, 
            Func<Sales, Member> rowName, 
            Func<Sales, ICubeObject> rowLevel)
        {
            var rowQuery =
                this.GetSalesQuery(date, price, grossProfit)
                    .WithMember("RowName", Axis.COLUMNS, x => rowName(x))
                    .WithMember("CategoryName", Axis.COLUMNS, x => categoryName(x))
                    .OnAxis(1, sales => rowLevel(sales));

            var rowData = rowQuery.Percolate<RowModel>().ToList();

            var categoryData =
                this.GetSalesQuery(date, price, grossProfit)
                    .WithMember("RowName", Axis.COLUMNS, x => rowName(x))
                    .WithMember("CategoryName", Axis.COLUMNS, x => categoryName(x))
                    .OnAxis(1, sales => categoryLevel(sales))
                    .Percolate<ReportModel>()
                    .ToList();

            var result =
                categoryData.Select(
                    r =>
                    new SalesReport
                        {
                            Totals = new SalesStatisticsDetails { Name = r.CategoryName, Statistics = Mapper.Map<SalesStatistics>(r) }, 
                            Rows =
                                rowData.Where(d => d.CategoryName == r.CategoryName)
                                .Select(d => new SalesStatisticsDetails { Name = d.RowName, Statistics = Mapper.Map<SalesStatistics>(d) })
                                .ToList()
                        }).ToList();

            result.Add(this.GetSalesTotal(reportName, date, price, grossProfit));

            return result;
        }

        private SalesReport GetSalesTotal(string reportName, DateTime date, Func<Sales, Member> price, Func<Sales, Member> grossProfit)
        {
            var q = this.GetSalesQuery(date, price, grossProfit);

            var totalSales = q.Percolate<ReportModel>().ToList().First();

            var result = new SalesReport { Totals = new SalesStatisticsDetails { Name = reportName, Statistics = Mapper.Map<SalesStatistics>(totalSales), IsGrandTotal = true } };

            return result;
        }

        private IMdxQueryable<Sales> GetSalesQuery(DateTime date, Func<Sales, Member> price, Func<Sales, Member> grossProfit)
        {
            var cubeDate = Member.Create<Sales>(x => x.Date.FiscalDate.DateKey[date.ToString("&yyyyMMdd")]);

            // TODO: there is LINQ syntax for this but I couldn't get it working
            var varianceDaySales = new Member("IIF([TodayLastYearSales] = 0 OR IsEmpty([TodayLastYearSales]),NULL,([TodaySales] - [TodayLastYearSales]) / [TodayLastYearSales])");
            var varianceDayGrossProfit =
                new Member(
                    "IIF([TodayLastYearGrossProfit] = 0 OR IsEmpty([TodayLastYearGrossProfit]),NULL,([TodayGrossProfit] - [TodayLastYearGrossProfit]) / [TodayLastYearGrossProfit])");
            var varianceWeekSales =
                new Member("IIF([ThisWeekLastYearSales] = 0 OR IsEmpty([ThisWeekLastYearSales]),NULL,([ThisWeekSales] - [ThisWeekLastYearSales]) / [ThisWeekLastYearSales])");
            var varianceWeekGrossProfit =
                new Member(
                    "IIF([ThisWeekLastYearGrossProfit] = 0 OR IsEmpty([ThisWeekLastYearGrossProfit]),NULL,([ThisWeekGrossProfit] - [ThisWeekLastYearGrossProfit]) / [ThisWeekLastYearGrossProfit])");
            var variancePeriodSales =
                new Member(
                    "IIF([ThisPeriodLastYearSales] = 0 OR IsEmpty([ThisPeriodLastYearSales]),NULL,([ThisPeriodSales] - [ThisPeriodLastYearSales]) / [ThisPeriodLastYearSales])");
            var variancePeriodGrossProfit =
                new Member(
                    "IIF([ThisPeriodLastYearGrossProfit] = 0 OR IsEmpty([ThisPeriodLastYearGrossProfit]),NULL,([ThisPeriodGrossProfit] - [ThisPeriodLastYearGrossProfit]) / [ThisPeriodLastYearGrossProfit])");
            var varianceYearSales = new Member("IIF([LastYearSales] = 0 OR IsEmpty([LastYearSales]),NULL,([ThisYearSales] - [LastYearSales]) / [LastYearSales])");
            var varianceYearGrossProfit =
                new Member("IIF([LastYearGrossProfit] = 0 OR IsEmpty([LastYearGrossProfit]),NULL,([TodayGrossProfit] - [LastYearGrossProfit]) / [LastYearGrossProfit])");

            var q =
                this.cosacsDW.Sales.WithMember("TodaySales", Axis.COLUMNS, x => price(x) & x.Date.DateKey[date.ToString("&yyyyMMdd")])
                    .WithMember("TodayGrossProfit", Axis.COLUMNS, x => grossProfit(x) & x.Date.DateKey[date.ToString("&yyyyMMdd")])
                    .WithMember("ThisWeekSales", Axis.COLUMNS, x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalWeek, cubeDate), price(x)))
                    .WithMember("ThisWeekGrossProfit", Axis.COLUMNS, x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalWeek, cubeDate), grossProfit(x)))
                    .WithMember("ThisPeriodSales", Axis.COLUMNS, x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalPeriod, cubeDate), price(x)))
                    .WithMember("ThisPeriodGrossProfit", Axis.COLUMNS, x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalPeriod, cubeDate), grossProfit(x)))
                    .WithMember("ThisYearSales", Axis.COLUMNS, x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalYear, cubeDate), price(x)))
                    .WithMember("ThisYearGrossProfit", Axis.COLUMNS, x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalYear, cubeDate), grossProfit(x)))
                    .WithMember("TodayLastYearSales", Axis.COLUMNS, x => price(x) & Mdx.ParallelPeriod(x.Date.FiscalDate.FiscalYear, 1, cubeDate))
                    .WithMember("TodayLastYearGrossProfit", Axis.COLUMNS, x => grossProfit(x) & Mdx.ParallelPeriod(x.Date.FiscalDate.FiscalYear, 1, cubeDate))
                    .WithMember(
                        "ThisWeekLastYearSales", 
                        Axis.COLUMNS, 
                        x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalWeek, Mdx.ParallelPeriod(x.Date.FiscalDate.FiscalYear, 1, cubeDate)), price(x)))
                    .WithMember(
                        "ThisWeekLastYearGrossProfit", 
                        Axis.COLUMNS, 
                        x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalWeek, Mdx.ParallelPeriod(x.Date.FiscalDate.FiscalYear, 1, cubeDate)), grossProfit(x)))
                    .WithMember(
                        "ThisPeriodLastYearSales", 
                        Axis.COLUMNS, 
                        x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalPeriod, Mdx.ParallelPeriod(x.Date.FiscalDate.FiscalYear, 1, cubeDate)), price(x)))
                    .WithMember(
                        "ThisPeriodLastYearGrossProfit", 
                        Axis.COLUMNS, 
                        x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalPeriod, Mdx.ParallelPeriod(x.Date.FiscalDate.FiscalYear, 1, cubeDate)), grossProfit(x)))
                    .WithMember(
                        "LastYearSales", 
                        Axis.COLUMNS, 
                        x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalYear, Mdx.ParallelPeriod(x.Date.FiscalDate.FiscalYear, 1, cubeDate)), price(x)))
                    .WithMember(
                        "LastYearGrossProfit", 
                        Axis.COLUMNS, 
                        x => Mdx.Aggregate(Mdx.PeriodsToDate(x.Date.FiscalDate.FiscalYear, Mdx.ParallelPeriod(x.Date.FiscalDate.FiscalYear, 1, cubeDate)), grossProfit(x)))
                    .WithMember("VarianceDaySales", Axis.COLUMNS, x => varianceDaySales)
                    .WithMember("VarianceDayGrossProfit", Axis.COLUMNS, x => varianceDayGrossProfit)
                    .WithMember("VarianceWeekSales", Axis.COLUMNS, x => varianceWeekSales)
                    .WithMember("VarianceWeekGrossProfit", Axis.COLUMNS, x => varianceWeekGrossProfit)
                    .WithMember("VariancePeriodSales", Axis.COLUMNS, x => variancePeriodSales)
                    .WithMember("VariancePeriodGrossProfit", Axis.COLUMNS, x => variancePeriodGrossProfit)
                    .WithMember("VarianceYearSales", Axis.COLUMNS, x => varianceYearSales)
                    .WithMember("VarianceYearGrossProfit", Axis.COLUMNS, x => varianceYearGrossProfit);

            return q;
        }
    }
}