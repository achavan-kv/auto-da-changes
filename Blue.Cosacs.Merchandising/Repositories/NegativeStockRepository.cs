namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public interface INegativeStockRepository
    {
        List<NegativeStockLocationModel> GetNegativeStockReport(NegativeStockSearchModel model);

        List<NegativeStockExportItem> GetNegativeStockExport(NegativeStockSearchModel model);

        void SummariseData();
    }

    public class NegativeStockRepository : INegativeStockRepository
    {
        private readonly IPeriodDataRepository periodDataRepository;
        private readonly IStockMovementRepository stockMovementRepository;
        public NegativeStockRepository(IPeriodDataRepository periodDataRepository, IStockMovementRepository stockMovementRepository)
        {
            this.periodDataRepository = periodDataRepository;
            this.stockMovementRepository = stockMovementRepository;
        }

        public List<NegativeStockLocationModel> GetNegativeStockReport(NegativeStockSearchModel model)
        {
            var mappedStock = CreateReport(model);
            if (model.IsGrouped)
            {
                return
                    mappedStock.GroupBy(m => new { m.Location, m.Fascia })
                        .Select(
                            m =>
                            new NegativeStockLocationModel()
                            {
                                Location = m.Key.Location,
                                Fascia = m.Key.Fascia,
                                Items = m.ToList()
                            })
                        .ToList();
            }

            // No groupings
            return new List<NegativeStockLocationModel>
            {
                new NegativeStockLocationModel
                {
                    Location = "All Locations",
                    Fascia = "All Fascias",
                    Items = mappedStock
                }
            };
        }

        public List<NegativeStockExportItem> GetNegativeStockExport(NegativeStockSearchModel model)
        {
            return Mapper.Map<List<NegativeStockExportItem>>(CreateReport(model));
        }

        private List<NegativeStockReportModel> CreateReport(NegativeStockSearchModel model)
        {
            var dateKey = ConvertToDateKey(model.PeriodEndDate);

            using (var scope = ReportingContext.Read())
            {
                var prodHierarchy =
                     scope.Context.ProductHierarchyView.Where(
                         p =>
                         (string.IsNullOrEmpty(model.Division) || p.Level == "Division" && p.Tag == model.Division)
                         && (string.IsNullOrEmpty(model.Department)
                             || p.Level == "Department" && p.Tag == model.Department)
                         && (string.IsNullOrEmpty(model.Class) || p.Level == "Class" && p.Tag == model.Class)).ToList();

                var prodIds = prodHierarchy.Select(p => p.ProductId).ToList();

                var negativeStock =
                     scope.Context.NegativeStockSnapshotView.Where(
                         n =>
                         (!model.LocationId.HasValue || n.LocationId == model.LocationId.Value)
                         && (string.IsNullOrEmpty(model.Fascia) || n.Fascia == model.Fascia)
                         && (string.IsNullOrEmpty(model.StockType) || model.StockType == n.ProductType)
                         && n.SnapshotDateId == dateKey).ToList();

                negativeStock = negativeStock.Where(n => prodIds.Contains(n.ProductId)).ToList();

                if (model.Tags != null && model.Tags.Any())
                {
                    negativeStock =
                        negativeStock.Where(o => o.Tags != null)
                            .Where(
                                o =>
                                JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(o.Tags)
                                    .Any(t => model.Tags.Any(s => s == t)))
                            .ToList();
                }

                var mappedStock = Mapper.Map<List<NegativeStockReportModel>>(negativeStock);

                var negativeProductIds = mappedStock.Select(m => m.ProductId);
                var negativeLocationIds = mappedStock.Select(m => m.LocationId);

                //Ritesh - CR 5372800 - Introduced StockMovement detail fetching from stored procedure instead of existing StockMovementReportView
                //to improve performance of negative stock report.
                var dtProductIds = new DataTable();
                var dtProductLocationIds = new DataTable();
                dtProductIds.Columns.Add("ProductIds", typeof(int));
                dtProductLocationIds.Columns.Add("ProductLocations", typeof(int));
                negativeProductIds.Distinct().Each(r => dtProductIds.Rows.Add(r));
                negativeLocationIds.Distinct().Each(r => dtProductLocationIds.Rows.Add(r));
                List<StockMovementReportModel> movementTemp = stockMovementRepository.GetStockMovementForNegativeStockReport(model.PeriodEndDate, 
                                                                                                                dtProductIds, dtProductLocationIds);

                var movement = (from temp in movementTemp
                                join negative in negativeStock
                                on temp.ProductId equals negative.ProductId
                                where temp.LocationId == negative.LocationId
                                select temp).ToList();


                foreach (var stock in mappedStock)
                {
                    var productMovements =
                        movement.Where(m => m.ProductId == stock.ProductId && m.LocationId == stock.LocationId).ToList();
                    if (productMovements.Count > 0)
                    {
                        var lastMovement = productMovements.OrderBy(p => p.DateProcessedUTC).Last();
                        stock.LastTransactionDate = lastMovement.DateProcessedUTC;
                        stock.LastTransactionId = lastMovement.TransactionId;
                        stock.LastTransactionNarration = lastMovement.Narration;
                        stock.LastTransactionType = lastMovement.Type;
                    }

                    var division =
                        prodHierarchy.FirstOrDefault(h => h.ProductId == stock.ProductId && h.Level == "Division");
                    if (division != null)
                    {
                        stock.Division = division.Tag;
                    }

                    var department =
                        prodHierarchy.FirstOrDefault(h => h.ProductId == stock.ProductId && h.Level == "Department");
                    if (department != null)
                    {
                        stock.Department = department.Tag;
                    }

                    var className =
                        prodHierarchy.FirstOrDefault(h => h.ProductId == stock.ProductId && h.Level == "Class");
                    if (className != null)
                    {
                        stock.Class = className.Tag;
                    }
                }
                return mappedStock;
            }
        }

        public void SummariseData()
        {
            using (var scope = Context.Write())
            {
                var endDate = periodDataRepository.GetNextDate(DateTime.Today.Date);

                var dateKey = ConvertToDateKey(endDate.EndDate.Value);
                var summary = scope.Context.NegativeStockSummaryView.ToList().Select(v =>
                    new NegativeStockSnapshot
                    {
                        ProductId = v.ProductId,
                        LocationId = v.LocationId,
                        AverageWeightedCost = v.AverageWeightedCost,
                        StockOnHandQuantity = v.StockOnHandQuantity,
                        StockOnHandSalesValue = v.StockOnHandSalesValue,
                        StockOnHandValue = v.StockOnHandValue,
                        SnapshotDateId = dateKey
                    })
               .ToList();

                scope.Context.NegativeStockSnapshot.RemoveRange(scope.Context.NegativeStockSnapshot.Where(s => s.SnapshotDateId == dateKey).ToList());
                scope.Context.NegativeStockSnapshot.AddRange(summary);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private int ConvertToDateKey(DateTime date)
        {
            return Convert.ToInt32(date.ToString("yyyyMMdd"));
        }

        private List<NegativeStockSnapshot> MapViewtoSnapshot(List<NegativeStockSummaryView> values, int dateKey)
        {
            return values
               .Select(v =>
                    new NegativeStockSnapshot
                    {
                        ProductId = v.ProductId,
                        LocationId = v.LocationId,
                        AverageWeightedCost = v.AverageWeightedCost,
                        StockOnHandQuantity = v.StockOnHandQuantity,
                        StockOnHandSalesValue = v.StockOnHandSalesValue,
                        StockOnHandValue = v.StockOnHandValue,
                        SnapshotDateId = dateKey
                    })
               .ToList();
        }
    }
}