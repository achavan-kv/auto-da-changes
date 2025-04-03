using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Merchandising.Repositories
{
    using Blue.Cosacs.Merchandising.DataWarehousing;
    using Blue.Cosacs.Merchandising.DataWarehousing.Models;
    using Blue.Cosacs.Merchandising.Models;
    using Percolator.AnalysisServices.Linq;

    public interface IStockValueReportRepository
    {
        List<StockValueLocationGroup> GetStockValueReport(StockValueSearchModel search);

        List<StockValueExportItem> GetStockValueExport(StockValueSearchModel search);

        void SummariseData();
    }

    public class StockValueReportRepository : IStockValueReportRepository
    {
        private readonly CosacsDW dataWarehouse;
        private readonly IRetailPriceRepository retailPriceRepository;
        private readonly IPeriodDataRepository periodDateRepository;

        public StockValueReportRepository(CosacsDW dataWarehouse, IRetailPriceRepository retailPriceRepository, IPeriodDataRepository periodDateRepository)
        {
            this.dataWarehouse = dataWarehouse;
            this.retailPriceRepository = retailPriceRepository;
            this.periodDateRepository = periodDateRepository;
        }

        public List<StockValueLocationGroup> GetStockValueReport(StockValueSearchModel search)
        {
            var classes = GetInventoryQuery<StockValueLocationModel>(
            search.PeriodEndDate.ToString("yyyy-MM-dd"),
            i => search.IsGrouped ? (i.Hierarchy.Hierarchy.ClassId * i.Location.Hierarchy.Children) : i.Hierarchy.Hierarchy.ClassId,
            search.DivisionName,
            search.DepartmentName,
            search.ClassName,
            search.Location,
            search.Fascia)
            .Where(x => x.StockOnHandQuantity != 0 || x.StockOnHandSalesValue != 0 || x.StockOnHandValue != 0)
            .ToList();

            if (!string.IsNullOrEmpty(search.Fascia))
            {
                var warehouse = GetInventoryQuery<StockValueLocationModel>(
                search.PeriodEndDate.ToString("yyyy-MM-dd"),
                i => search.IsGrouped ? (i.Hierarchy.Hierarchy.ClassId * i.Location.Hierarchy.Children) : i.Hierarchy.Hierarchy.ClassId,
                search.DivisionName,
                search.DepartmentName,
                search.ClassName,
                search.Location,
                null,
                true)
                .Where(x => x.StockOnHandQuantity != 0 || x.StockOnHandSalesValue != 0 || x.StockOnHandValue != 0)
                .ToList();

                classes.AddRange(warehouse);
            }

            if (search.IsGrouped)
            {
                return classes.GroupBy(c => new { c.LocationId, c.Location, c.Fascia })
                        .Select(loc => GetLocationValueModel(loc.Key.Location, loc.Key.Fascia, loc))
                        .ToList();
            }

            return classes.Any(x => x.StockOnHandQuantity != 0 || x.StockOnHandSalesValue != 0 || x.StockOnHandValue != 0)
                ? new List<StockValueLocationGroup> { GetLocationValueModel("All Locations", "All Fascias", classes) }
                : null;
        }

        private StockValueLocationGroup GetLocationValueModel(string location, string fascia, IEnumerable<StockValueLocationModel> locs)
        {
            return new StockValueLocationGroup()
            {
                Location = location,
                Fascia = fascia,
                Items = locs.GroupBy(c => c.GrandParent)
                    .Select(div =>
                    new StockValueReportItem()
                    {
                        Level = div.Key,
                        StockOnHandQuantity = div.Sum(s => s.StockOnHandQuantity),
                        StockOnHandValue = div.Sum(s => s.StockOnHandValue),
                        StockOnHandSalesValue = div.Sum(s => s.StockOnHandSalesValue),
                        Children = div.Where(c => c.GrandParent == div.Key.ToString())
                            .GroupBy(c => c.Parent)
                            .Select(
                                dep =>
                                new StockValueReportItem()
                                {
                                    Level = dep.Key.ToString(),
                                    StockOnHandQuantity = dep.Sum(s => s.StockOnHandQuantity),
                                    StockOnHandValue = dep.Sum(s => s.StockOnHandValue),
                                    StockOnHandSalesValue = dep.Sum(s => s.StockOnHandSalesValue),
                                    Children =
                                        dep.Where(c => c.Parent == dep.Key.ToString())
                                        .Select(c => new StockValueReportItem()
                                        {
                                            Level = c.Name,
                                            StockOnHandValue = c.StockOnHandValue,
                                            StockOnHandSalesValue = c.StockOnHandSalesValue,
                                            StockOnHandQuantity = c.StockOnHandQuantity
                                        }).ToList()
                                }).ToList()
                    }).ToList()
            };
        }

        public void SummariseData()
        {
            using (var scope = Context.Write())
            {
                var endDate = DateTime.Now.Hour > 7 ? periodDateRepository.GetNextDate(DateTime.Today.Date) : periodDateRepository.GetNextDate(DateTime.Today.AddDays(-1).Date);
                                
                scope.Context.StockEvaluationSnapshotRefresh(new StockEvaluationSnapshotRefreshInput { date = Convert.ToInt32(endDate.EndDate.Value.ToString("yyyyMMdd")) });
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private IEnumerable<T> GetInventoryQuery<T>(
            string periodEndDate,
            Func<Inventory, Set> querySet,
            string divisionName = null,
            string departmentName = null,
            string className = null,
            string location = null,
            string fascia = null,
            bool warehouse = false) where T : new()
        {
            var query = dataWarehouse.Inventory
                .OnAxis(Axis.ROWS, true, x => querySet(x))
                .WithMember("[Name]", Axis.COLUMNS, x => x.Hierarchy.Hierarchy.CurrentMember.Name)
                .WithMember("[ParentName]", Axis.COLUMNS, x => x.Hierarchy.Hierarchy.CurrentMember.Parent.Name)
                .WithMember("[GrandParentName]", Axis.COLUMNS, x => x.Hierarchy.Hierarchy.CurrentMember.Parent.Parent.Name)
                .WithMember("[LocId]", Axis.COLUMNS, x => x.Location.LocationId.CurrentMember.Name)
                .WithMember("[LocName]", Axis.COLUMNS, x => x.Location.LocationName.CurrentMember.Name)
                .WithMember("[FascName]", Axis.COLUMNS, x => x.Location.Fascia.CurrentMember.Name)
                .WithMember("[WareName]", Axis.COLUMNS, x => x.Location.Warehouse.CurrentMember.Name)
                .WithMember("StockOnHandQuantity", Axis.COLUMNS, x => x.StockOnHandQuantity)
                .WithMember("StockOnHandValue", Axis.COLUMNS, x => x.StockOnHandValue)
                .WithMember("StockOnHandSalesValue", Axis.COLUMNS, x => x.StockOnHandSalesValue)
                .Where(x => x.Date.DateKey[periodEndDate]);

            if (!string.IsNullOrEmpty(divisionName))
            {
                query = query.Where(x => x.Hierarchy.Division[divisionName]);
            }

            if (!string.IsNullOrEmpty(departmentName))
            {
                query = query.Where(x => x.Hierarchy.Department[departmentName]);
            }

            if (!string.IsNullOrEmpty(className))
            {
                query = query.Where(x => x.Hierarchy.Class[className]);
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(x => x.Location.LocationName[location]);
            }

            if (!string.IsNullOrEmpty(fascia))
            {
                query = query.Where(x => x.Location.Fascia[fascia]);
            }

            if (warehouse)
            {
                query = query.Where(x => x.Location.Warehouse["&True"]);
            }

            return query.Percolate<T>();
        }

        public List<StockValueExportItem> GetStockValueExport(StockValueSearchModel search)
        {
            var export = new List<StockValueExportItem>();
            var resultLocations = GetStockValueReport(search);
            if (resultLocations != null && resultLocations.Any())
            {
                foreach (var loc in resultLocations)
                {
                    foreach (var item in loc.Items)
                    {
                        export.AddRange(GetStockValueExportForLocation(1, loc, item));
                    }
                }
            }
            return export;
        }

        private List<StockValueExportItem> GetStockValueExportForLocation(int levelId, StockValueLocationGroup loc, StockValueReportItem item)
        {
            var exportRows = new List<StockValueExportItem>
            {
                new StockValueExportItem()
                    {
                        Location = loc.Location,
                        Fascia = loc.Fascia,
                        HierarchyLevel = levelId,
                        HierarchyTag = item.Level,
                        StockOnHandQuantity = item.StockOnHandQuantity,
                        StockOnHandValue = item.StockOnHandValue.ToString(),
                        StockOnHandSalesValue = item.StockOnHandSalesValue.ToString()
                    }
            };

            if (item.Children != null && item.Children.Any())
            {
                foreach (var child in item.Children)
                {
                    exportRows.AddRange(GetStockValueExportForLocation(levelId + 1, loc, child));
                }
            }

            return exportRows;
        }
    }
}