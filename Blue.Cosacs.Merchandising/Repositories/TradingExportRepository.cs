using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Blue.Cosacs.Merchandising.Models;

namespace Blue.Cosacs.Merchandising.Repositories
{
    public interface ITradingExportRepository
    {
    }

    public class TradingExportRepository : ITradingExportRepository
    {
        public List<TradingExportModel> MapToExportModel(TradingExportDatesModel exportDates, string sortOrder, List<TradingExportGenericModel> records)
        {
            var lastYearDay = exportDates.LastYearDay == null ? exportDates.LastYearToDate : exportDates.LastYearDay;

            using (var scope = Context.Read())
            {
                var exportResults = records
                                    .GroupBy(p => new
                                                    {
                                                        Branch = p.LocationId,
                                                        BranchName = p.LocationName,
                                                        Class = p.Class == null ? string.Empty : p.Class,
                                                        ClassCode = p.ClassCode == null ? string.Empty : p.ClassCode.ToString(),
                                                        Division = p.Division,
                                                        Department = p.Department == null ? string.Empty : p.Department,
                                                        DepartmentCode = p.DepartmentCode == null ? string.Empty : p.DepartmentCode.ToString(),
                                                        SortOrder = sortOrder,
                                                        SalesType = p.SaleType
                                                    })
                                    .Select(p =>
                                            {
                                                var result = new TradingExportModel
                                                             {
                                                                 Branch = p.Key.Branch,
                                                                 BranchName = p.Key.BranchName,
                                                                 Class = p.Key.Class,
                                                                 ClassCode = p.Key.ClassCode,
                                                                 Division = p.Key.Division,
                                                                 Department = p.Key.Department,
                                                                 DepartmentCode = p.Key.DepartmentCode,
                                                                 SortOrder = p.Key.SortOrder,
                                                                 SalesType = p.Key.SalesType
                                                             };
                                                //Daily Prices and Gross Profit
                                                var price = 0D;
                                                var grossProfit = 0D;
                                                var priceLY = 0D;
                                                var grossProfitLY = 0D;

                                                var dayActualValueRow = p
                                                                         .FirstOrDefault(r => r.TransactionDate == exportDates.Day || r.TransactionDate == new DateTime(1901, 2, 3));
                                                if (dayActualValueRow != null)
                                                {
                                                    price = dayActualValueRow.Price.Value;
                                                    grossProfit = dayActualValueRow.GrossProfit.Value;
                                                }
                                                //If this day did not exist in the Financial calendar from last year the export 0
                                                if (exportDates.LastYearDay != null)
                                                {
                                                    var dayActualValueLYRow = p
                                                                               .FirstOrDefault(r => r.TransactionDate == lastYearDay || r.TransactionDate == new DateTime(1901, 2, 3));
                                                    if (dayActualValueLYRow != null)
                                                    {
                                                        priceLY = dayActualValueLYRow.Price.Value;
                                                        grossProfitLY = dayActualValueLYRow.GrossProfit.Value;
                                                    }
                                                }

                                                result.DayActualValue = price.ToString();
                                                result.DayActualValueLY = priceLY.ToString();
                                                result.DayVariance = (price - priceLY).ToString();

                                                result.DayActualGP = grossProfit.ToString();
                                                result.DayActualGPLY = grossProfitLY.ToString();
                                                result.DayVarianceGP = (grossProfit - grossProfitLY).ToString();

                                                //Weekly Prices and Gross Profit
                                                var weekActualValue = p
                                                                       .Where(l =>
                                                                                 l.TransactionDate >= exportDates.Week
                                                                                 && l.TransactionDate <= exportDates.Day)
                                                                       .Sum(l => l.Price);
                                                var weekActualValueLY = p
                                                                         .Where(l =>
                                                                                    l.TransactionDate >= exportDates.LastYearWeek
                                                                                    && l.TransactionDate <= lastYearDay)
                                                                         .Sum(l => l.Price);
                                                var weekActualGP = p
                                                                    .Where(l =>
                                                                                 l.TransactionDate >= exportDates.Week
                                                                                 && l.TransactionDate <= exportDates.Day)
                                                                    .Sum(l => l.GrossProfit);
                                                var weekActualGPLY = p
                                                                      .Where(l =>
                                                                                 l.TransactionDate >= exportDates.LastYearWeek
                                                                                 && l.TransactionDate <= lastYearDay)
                                                                      .Sum(l => l.GrossProfit);
                                                result.WeekActualValue = weekActualValue.ToString();
                                                result.WeekActualValueLY = weekActualValueLY.ToString();
                                                result.WeekVariance = (weekActualValue - weekActualValueLY).ToString();

                                                result.WeekActualGP = weekActualGP.ToString();
                                                result.WeekActualGPLY = weekActualGPLY.ToString();
                                                result.WeekVarianceGP = (weekActualGP - weekActualGPLY).ToString();

                                                //Period Prices and Gross Profit
                                                var periodActualValue = p
                                                                         .Where(l =>
                                                                                    l.TransactionDate >= exportDates.Period
                                                                                    && l.TransactionDate <= exportDates.Day)
                                                                         .Sum(l => l.Price);
                                                var periodActualValueLY = p
                                                                           .Where(l =>
                                                                                      l.TransactionDate >= exportDates.LastYearPeriod
                                                                                      && l.TransactionDate <= lastYearDay)
                                                                           .Sum(l => l.Price);
                                                var periodActualGP = p
                                                                      .Where(l =>
                                                                                 l.TransactionDate >= exportDates.Period
                                                                                 && l.TransactionDate <= exportDates.Day)
                                                                      .Sum(l => l.GrossProfit);
                                                var periodActualGPLY = p
                                                                        .Where(l =>
                                                                                   l.TransactionDate >= exportDates.LastYearPeriod
                                                                                   && l.TransactionDate <= lastYearDay)
                                                                        .Sum(l => l.GrossProfit);
                                                result.PeriodActualValue = periodActualValue.ToString();
                                                result.PeriodActualValueLY = periodActualValueLY.ToString();
                                                result.PeriodVariance = (periodActualValue - periodActualValueLY).ToString();

                                                result.PeriodActualGP = periodActualGP.ToString();
                                                result.PeriodActualGPLY = periodActualGPLY.ToString();
                                                result.PeriodVarianceGP = (periodActualGP - periodActualGPLY).ToString();

                                                //Year To Date Prices and Gross Profit
                                                if (sortOrder == "T" || sortOrder == "V")
                                                {
                                                    var ytdActualValue = p
                                                                      .Where(l =>
                                                                                 l.TransactionDate == exportDates.Day)
                                                                      .Sum(l => l.Price);
                                                    var ytdActualValueLY = p
                                                                            .Where(l =>
                                                                                       l.TransactionDate == lastYearDay)
                                                                            .Sum(l => l.Price);
                                                    var ytdActualGP = p
                                                                       .Where(l =>
                                                                                  l.TransactionDate == exportDates.Day)
                                                                       .Sum(l => l.GrossProfit);
                                                    var ytdActualGPLY = p
                                                                         .Where(l =>
                                                                                    l.TransactionDate == lastYearDay)
                                                                         .Sum(l => l.GrossProfit);
                                                    result.YTDActualValue = ytdActualValue.ToString();
                                                    result.YTDActualValueLY = ytdActualValueLY.ToString();
                                                    result.YTDVariance = (ytdActualValue - ytdActualValueLY).ToString();

                                                    result.YTDActualGP = ytdActualGP.ToString();
                                                    result.YTDActualGPLY = ytdActualGPLY.ToString();
                                                    result.YTDVarianceGP = (ytdActualGP - ytdActualGPLY).ToString();
                                                }
                                                else
                                                {
                                                    var ytdActualValue = p
                                                                      .Where(l =>
                                                                                 l.TransactionDate >= exportDates.Year
                                                                                 && l.TransactionDate <= exportDates.Day)
                                                                      .Sum(l => l.Price);
                                                    var ytdActualValueLY = p
                                                                            .Where(l =>
                                                                                       l.TransactionDate >= exportDates.LastYearYear
                                                                                       && l.TransactionDate <= lastYearDay)
                                                                            .Sum(l => l.Price);
                                                    var ytdActualGP = p
                                                                       .Where(l =>
                                                                                  l.TransactionDate >= exportDates.Year
                                                                                  && l.TransactionDate <= exportDates.Day)
                                                                       .Sum(l => l.GrossProfit);
                                                    var ytdActualGPLY = p
                                                                         .Where(l =>
                                                                                    l.TransactionDate >= exportDates.LastYearYear
                                                                                    && l.TransactionDate <= lastYearDay)
                                                                         .Sum(l => l.GrossProfit);
                                                    result.YTDActualValue = ytdActualValue.ToString();
                                                    result.YTDActualValueLY = ytdActualValueLY.ToString();
                                                    result.YTDVariance = (ytdActualValue - ytdActualValueLY).ToString();

                                                    result.YTDActualGP = ytdActualGP.ToString();
                                                    result.YTDActualGPLY = ytdActualGPLY.ToString();
                                                    result.YTDVarianceGP = (ytdActualGP - ytdActualGPLY).ToString();
                                                }                                                

                                                return result;
                                            })
                                       .ToList();
                return exportResults;
            }
        }

        private async Task<T> ExecuteAsync<T>(Func<T> toExecute) where T : class
        {
            return await Task<T>.Run<T>(toExecute).ConfigureAwait(false);
        }

        public List<TradingExportModel> GetExportData(DateTime date)
        {
            List<TradingExportModel> data = new List<TradingExportModel>();

            var exportDates = GetPeriodDates(date);

            //Export dates for stock levels
            //Get the current month end as it uses a different view                
            TradingExportDatesModel exportDatesForStock;
            Task<List<TradingExportGenericModel>> stockValueBranchTask = null;
            Task<List<TradingExportGenericModel>> stockValueTask = null;

            DateTime endOfMonth = new DateTime(exportDates.Day.Year, exportDates.Day.Month, DateTime.DaysInMonth(exportDates.Day.Year, exportDates.Day.Month));
            var endOfMonthInt = int.Parse(exportDates.Day.ToString("yyyyMMdd"));
            if ((exportDates.Day - endOfMonth).TotalDays == 0)
            {
                exportDatesForStock = GetPeriodDates(endOfMonth);
            }
            else
            {
                exportDatesForStock = GetPeriodDates(DateTime.Now.Date);
            }

            //Warranty details 
            var warrantiesTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRWarrantyView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = string.Empty,
                            ClassCode = "0",
                            DateKey = p.DateKey,
                            Department = string.Empty,
                            DepartmentCode = "0",
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = p.GrossProfit,
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        })
                        .ToList();
                }
            });

            //Installation Details
            var installationsTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRInstallationsView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = string.Empty,
                            ClassCode = "0",
                            DateKey = p.DateKey,
                            Department = p.Department,
                            DepartmentCode = "0",
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = p.GrossProfit,
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Affinity
            var affinityTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRAffinityView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = string.Empty,
                            ClassCode = "0",
                            DateKey = p.DateKey,
                            Department = p.Department,
                            DepartmentCode = "0",
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = p.GrossProfit,
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Generic Services
            var genericServicesTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRIGenericServicesView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = string.Empty,
                            ClassCode = "0",
                            DateKey = p.DateKey,
                            Department = p.Department,
                            DepartmentCode = p.DepartmentCode.ToString(),
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = p.GrossProfit,
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Loans
            var loansTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRLoansView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = string.Empty,
                            ClassCode = "0",
                            DateKey = p.DateKey,
                            Department = p.Department,
                            DepartmentCode = "0",
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = System.Convert.ToDouble(p.GrossProfit),
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Service Charge
            var serviceChargeTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRServiceChargeView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = string.Empty,
                            ClassCode = "0",
                            DateKey = p.DateKey,
                            Department = p.Department,
                            DepartmentCode = "0",
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = System.Convert.ToDouble(p.GrossProfit),
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Rebates
            var rebatesTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRRebatesView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = string.Empty,
                            ClassCode = "0",
                            DateKey = p.DateKey,
                            Department = p.Department,
                            DepartmentCode = "0",
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = System.Convert.ToDouble(p.GrossProfit),
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Discounts
            var discountsTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRDiscountsView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = string.Empty,
                            ClassCode = "0",
                            DateKey = p.DateKey,
                            Department = p.Department,
                            DepartmentCode = p.DepartmentCode.ToString(),
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = p.GrossProfit,
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Sales Sort Order A
            var salesATask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRSalesAView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = p.Class,
                            ClassCode = p.ClassCode,
                            DateKey = p.DateKey,
                            Department = p.Department,
                            DepartmentCode = p.DepartmentCode,
                            Division = p.Division,
                            Fascia = string.Empty,
                            GrossProfit = System.Convert.ToDouble(p.GrossProfit),
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Sales Sort Order B
            var salesBTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRSalesBView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = p.Class,
                            ClassCode = p.ClassCode,
                            DateKey = p.DateKey,
                            Department = p.Department,
                            DepartmentCode = p.DepartmentCode.ToString(),
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = System.Convert.ToDouble(p.GrossProfit),
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Sales Sort Order F
            var salesFTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRSalesFView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = string.Empty,
                            ClassCode = "0",
                            DateKey = p.DateKey,
                            Department = string.Empty,
                            DepartmentCode = "0",
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = System.Convert.ToDouble(p.GrossProfit),
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Penalties(Fees)
            var penaltyTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRPenaltyView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = string.Empty,
                            ClassCode = "0",
                            DateKey = p.DateKey,
                            Department = string.Empty,
                            DepartmentCode = "0",
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = System.Convert.ToDouble(p.GrossProfit),
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Credit and Cash Difference
            var creditCashDifferenceTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
            {
                using (var scope = Context.Read())
                {
                    //HACK i know but we need to make this work
                    //i hope i have time to find out why this is not getting the correct value 
                    scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                    return scope.Context.WTRCreditCashDifferenceView
                        .Where(p => p.TransactionDate <= exportDates.Day)
                        .ToList()
                        .Select(p => new TradingExportGenericModel
                        {
                            Class = p.Class,
                            ClassCode = p.ClassCode.ToString(),
                            DateKey = p.DateKey,
                            Department = p.Department,
                            DepartmentCode = p.DepartmentCode.ToString(),
                            Division = p.Division,
                            Fascia = p.Fascia,
                            GrossProfit = System.Convert.ToDouble(p.GrossProfit),
                            Id = p.Id,
                            LocationId = p.LocationId.ToString(),
                            LocationName = p.LocationName,
                            Price = System.Convert.ToDouble(p.Price),
                            SaleType = p.SaleType,
                            TransactionDate = p.TransactionDate
                        }).ToList();
                }
            });

            //Stock Values Company and Branch Level

            if ((exportDates.Day - endOfMonth).TotalDays == 0)
            {
                stockValueBranchTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
                {
                    using (var scope = Context.Read())
                    {
                        //HACK i know but we need to make this work
                        //i hope i have time to find out why this is not getting the correct value 
                        scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                        return scope.Context.WTREndOfMonthStockItemsValueBranchView
                            .Where(p => p.TransactionDate == exportDatesForStock.Day || p.TransactionDate == exportDatesForStock.LastYearDay)
                            .ToList()
                            .Select(p => new TradingExportGenericModel
                            {
                                Class = string.Empty,
                                ClassCode = "0",
                                DateKey = p.DateKey,
                                Department = string.Empty,
                                DepartmentCode = "0",
                                Division = p.Division,
                                Fascia = p.Fascia,
                                GrossProfit = p.GrossProfit,
                                Id = p.Id,
                                LocationId = p.LocationId.ToString(),
                                LocationName = p.LocationName,
                                Price = System.Convert.ToDouble(p.Price),
                                SaleType = p.SaleType,
                                TransactionDate = p.TransactionDate
                            }).ToList();
                    }
                });

                stockValueTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
                {
                    using (var scope = Context.Read())
                    {
                        //HACK i know but we need to make this work
                        //i hope i have time to find out why this is not getting the correct value 
                        scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                        return scope.Context.WTREndOfMonthStockItemsValueView
                            .Where(p => p.TransactionDate == exportDatesForStock.Day || p.TransactionDate == exportDatesForStock.LastYearDay)
                            .ToList()
                            .Select(p => new TradingExportGenericModel
                            {
                                Class = p.Class,
                                ClassCode = p.ClassCode,
                                DateKey = p.DateKey,
                                Department = p.Department,
                                DepartmentCode = p.DepartmentCode,
                                Division = p.Division,
                                Fascia = p.Fascia,
                                GrossProfit = p.GrossProfit,
                                Id = p.Id,
                                LocationId = p.LocationId.ToString(),
                                LocationName = p.LocationName,
                                Price = System.Convert.ToDouble(p.Price),
                                SaleType = p.SaleType,
                                TransactionDate = p.TransactionDate
                            }).ToList();
                    }
                });
            }
            else
            {
                stockValueBranchTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
                {
                    using (var scope = Context.Read())
                    {
                        //HACK i know but we need to make this work
                        //i hope i have time to find out why this is not getting the correct value 
                        scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                        return scope.Context.WTRCurrentStockItemsValueBranchView
                            .ToList()
                            .Select(p => new TradingExportGenericModel
                            {
                                Class = string.Empty,
                                ClassCode = "0",
                                DateKey = p.DateKey,
                                Department = string.Empty,
                                DepartmentCode = "0",
                                Division = p.Division,
                                Fascia = p.Fascia,
                                GrossProfit = p.GrossProfit,
                                Id = p.Id,
                                LocationId = p.LocationId.ToString(),
                                LocationName = p.LocationName,
                                Price = System.Convert.ToDouble(p.Price),
                                SaleType = p.SaleType,
                                TransactionDate = p.TransactionDate
                            }).ToList();
                    }
                });

                stockValueTask = ExecuteAsync<List<TradingExportGenericModel>>(() =>
                {
                    using (var scope = Context.Read())
                    {
                        //HACK i know but we need to make this work
                        //i hope i have time to find out why this is not getting the correct value 
                        scope.Context.Database.CommandTimeout = 7200;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["LongRunningQueryCommandTimeoutInSecs"]);
                        return scope.Context.WTRCurrentStockItemsValueView
                            .ToList()
                            .Select(p => new TradingExportGenericModel
                            {
                                Class = p.Class,
                                ClassCode = p.ClassCode,
                                DateKey = p.DateKey,
                                Department = p.Department,
                                DepartmentCode = p.DepartmentCode,
                                Division = p.Division,
                                Fascia = p.Fascia,
                                GrossProfit = p.GrossProfit,
                                Id = p.Id,
                                LocationId = p.LocationId.ToString(),
                                LocationName = p.LocationName,
                                Price = System.Convert.ToDouble(p.Price),
                                SaleType = p.SaleType,
                                TransactionDate = p.TransactionDate
                            }).ToList();
                    }
                });
            }

            var taskList = new List<Task> 
            {
                warrantiesTask, 
                installationsTask, 
                affinityTask, 
                genericServicesTask, 
                loansTask, 
                serviceChargeTask, 
                rebatesTask, 
                discountsTask, 
                salesATask, 
                salesBTask, 
                salesFTask, 
                penaltyTask, 
                creditCashDifferenceTask,
                stockValueBranchTask,
                stockValueTask
            };

            //Add all tasks to the list and wait for them to complete
            Task.WaitAll(new Task[] 
            {
                warrantiesTask, installationsTask, affinityTask, genericServicesTask, loansTask, 
                serviceChargeTask, rebatesTask, discountsTask, salesATask, salesBTask, 
                salesFTask, penaltyTask, creditCashDifferenceTask, stockValueBranchTask, stockValueTask
            });
            
            //Concatenate all data            
            data.AddRange(MapToExportModel(exportDates, "W", warrantiesTask.Result));                  //Warranties
            data.AddRange(MapToExportModel(exportDates, "I", installationsTask.Result));               //Installations
            data.AddRange(MapToExportModel(exportDates, "N", affinityTask.Result));                    //Affinity
            data.AddRange(MapToExportModel(exportDates, "O", genericServicesTask.Result));             //Other Generic Services
            data.AddRange(MapToExportModel(exportDates, "L", loansTask.Result));                       //Loans
            data.AddRange(MapToExportModel(exportDates, "S", serviceChargeTask.Result));               //Service Charge
            data.AddRange(MapToExportModel(exportDates, "R", rebatesTask.Result));                     //Rebates
            data.AddRange(MapToExportModel(exportDates, "D", discountsTask.Result));                   //Discounts
            data.AddRange(MapToExportModel(exportDates, "A", salesATask.Result));                      //SalesA - Sales by Fascia
            data.AddRange(MapToExportModel(exportDates, "B", salesBTask.Result));                      //SalesB - Sales by Branch
            data.AddRange(MapToExportModel(exportDates, "F", salesFTask.Result));                      //SalesF - Sales by Sale Type
            data.AddRange(MapToExportModel(exportDates, "P", penaltyTask.Result));                     //Penalty(Interest)
            data.AddRange(MapToExportModel(exportDates, "G", creditCashDifferenceTask.Result));        //Credit/Cash Difference
            data.AddRange(MapToExportModel(exportDatesForStock, "T", stockValueBranchTask.Result));    //Stock Values at Branch Level
            data.AddRange(MapToExportModel(exportDatesForStock, "V", stockValueTask.Result));          //Stock Values at Company Level
                        
            return data;
        }

        private TradingExportDatesModel GetPeriodDates(DateTime date)
        {
            using (var scope = Context.Read())
            {
                TradingExportDatesModel exportDates = new TradingExportDatesModel();
                var allDates = scope.Context.DatesView.ToList();

                var day = allDates.FirstOrDefault(p => p.FullDateAlternateKey == date);

                if (day == null)
                {
                    throw new Exception("Date does not exist in period data");
                }

                Func<DatesView, DateTime> getDate = (view) =>
                {
                    if (view == null)
                    {
                        throw new Exception("Dates Periods not setup. Please make sure the previous and current year have end dates.");
                    }
                    return view.FullDateAlternateKey;
                };

                exportDates.Day = day.FullDateAlternateKey;

                exportDates.Week = getDate(allDates.Where(p => p.FiscalWeek == day.FiscalWeek && p.FiscalYear == day.FiscalYear)
                                           .OrderBy(p => p.DateKey)
                                           .FirstOrDefault());

                exportDates.Period = getDate(allDates.Where(p => p.FiscalPeriod == day.FiscalPeriod && p.FiscalYear == day.FiscalYear)
                                             .OrderBy(p => p.DateKey)
                                             .FirstOrDefault());

                exportDates.Year = getDate(allDates.Where(p => p.FiscalYear == day.FiscalYear)
                                           .OrderBy(p => p.DateKey)
                                           .FirstOrDefault());

                var lastYearDay = allDates.FirstOrDefault(p => p.DayNumberOFWeek == day.DayNumberOFWeek && p.FiscalWeek == day.FiscalWeek && p.FiscalYear == day.FiscalYear - 1);

                if (lastYearDay == null)
                {
                    exportDates.LastYearToDate = getDate(allDates.Where(p => p.FiscalWeek == day.FiscalWeek && p.FiscalYear == day.FiscalYear - 1)
                                                 .OrderByDescending(p => p.DateKey)
                                                 .FirstOrDefault());
                }
                else
                {
                    exportDates.LastYearDay = lastYearDay.FullDateAlternateKey;
                }

                exportDates.LastYearWeek = getDate(allDates.Where(p => p.FiscalWeek == day.FiscalWeek && p.FiscalYear == day.FiscalYear - 1)
                                   .OrderBy(p => p.DateKey)
                                   .FirstOrDefault());

                exportDates.LastYearPeriod = getDate(allDates.Where(p => p.FiscalPeriod == day.FiscalPeriod && p.FiscalYear == day.FiscalYear - 1)
                                     .OrderBy(p => p.DateKey)
                                     .FirstOrDefault());

                exportDates.LastYearYear = getDate(allDates.Where(p => p.FiscalYear == day.FiscalYear - 1)
                                   .OrderBy(p => p.DateKey)
                                   .FirstOrDefault());

                return exportDates;
            }
        }
    }
}
