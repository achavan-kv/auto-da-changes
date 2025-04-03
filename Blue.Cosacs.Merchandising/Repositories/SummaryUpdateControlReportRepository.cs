namespace Blue.Cosacs.Merchandising.Repositories
{
    using Blue.Cosacs.Merchandising.Models;
    using System;
    using System.Linq;

    public interface ISummaryUpdateControlReportRepository
    {
        SummaryUpdateControlReportViewModel GetReport(SummaryUpdateControlReportQueryModel query);
    }

    public class SummaryUpdateControlReportRepository : ISummaryUpdateControlReportRepository
    {
        public SummaryUpdateControlReportViewModel GetReport(SummaryUpdateControlReportQueryModel query)
        {
            using (var scope = ReportingContext.Read())
            {
                Func<IQueryable<SummaryUpdateControlReportView>, InventoryModel> getSummary = su =>
                {
                    return su.GroupBy(x => true)
                            .Select(g => new InventoryModel
                            {
                                RegularUnits = g.Where(x => x.ProductType == "RegularStock" || x.ProductType == "ProductWithoutStock").Sum(x => x.Units) ?? 0,
                                RegularValue = g.Where(x => x.ProductType == "RegularStock" || x.ProductType == "ProductWithoutStock").Sum(x => x.Value) ?? 0,
                                SparePartsUnits = g.Where(x => x.ProductType == "SparePart").Sum(x => x.Units) ?? 0,
                                SparePartsValue = g.Where(x => x.ProductType == "SparePart").Sum(x => x.Value) ?? 0,
                                RepossessedUnits = g.Where(x => x.ProductType == "RepossessedStock").Sum(x => x.Units) ?? 0,
                                RepossessedValue = g.Where(x => x.ProductType == "RepossessedStock").Sum(x => x.Value) ?? 0,
                            }).FirstOrDefault();
                };

                var summary = scope.Context.SummaryUpdateControlReportView.AsNoTracking()
                            .Where(x => query.RunNumber == null || x.RunNumber == query.RunNumber)
                            .Where(x => query.LocationId == null || x.LocationId == query.LocationId);

                var sb = scope.Context.SucrBase.AsQueryable();

                if (query.LocationId.HasValue)
                {
                    sb = sb.Where(b => b.LocationId == query.LocationId.Value);
                }

                if (!sb.Any())
                {
                    throw new Exception("SUCR information has not been imported for selected branch. Please import.");
                }

                var sucrBased = sb.GroupBy(s => s.SummaryType)
                               .Select(s => new { key = s.Key, Units = s.Sum(x => x.Units), Value = s.Sum(x => x.Value) })
                               .ToDictionary(d => d.key);

                var model = new SummaryUpdateControlReportViewModel
                {
                    OpeningInventory = new InventoryModel
                    {
                        RegularUnits = sucrBased["RegularStock"].Units,
                        RegularValue = sucrBased["RegularStock"].Value,
                        RepossessedUnits = sucrBased["RepossessedStock"].Units,
                        RepossessedValue = sucrBased["RepossessedStock"].Value,
                        SparePartsUnits = sucrBased["SparePart"].Units,
                        SparePartsValue = sucrBased["SparePart"].Value
                    },
                    ClosingInventory = new InventoryModel
                    {
                        RegularUnits = 0,
                        RegularValue = 0,
                        RepossessedUnits = 0,
                        RepossessedValue = 0,
                        SparePartsUnits = 0,
                        SparePartsValue = 0
                    }
                };

                var rowsQuery = summary;

                if (query.ToDate.HasValue)
                {
                    var d = query.ToDate.Value.AddDays(1);
                    rowsQuery = rowsQuery.Where(x => x.TransactionDate < d);

                    var results = getSummary(summary.Where(s => s.TransactionDate < d));
                    if (results != null)
                    {
                        model.ClosingInventory = results;
                    }
                }
                else
                {
                    var results = getSummary(summary);
                    if (results != null)
                    {
                        model.ClosingInventory = results;
                    }
                }

                model.ClosingInventory.RegularUnits += model.OpeningInventory.RegularUnits;
                model.ClosingInventory.RegularValue += model.OpeningInventory.RegularValue;
                model.ClosingInventory.RepossessedUnits += model.OpeningInventory.RepossessedUnits;
                model.ClosingInventory.RepossessedValue += model.OpeningInventory.RepossessedValue;
                model.ClosingInventory.SparePartsUnits += model.OpeningInventory.SparePartsUnits;
                model.ClosingInventory.SparePartsValue += model.OpeningInventory.SparePartsValue;

                if (query.FromDate.HasValue)
                {
                    rowsQuery = rowsQuery.Where(x => x.TransactionDate >= query.FromDate.Value);
                    var balance = getSummary(summary.Where(s => s.TransactionDate < query.FromDate.Value));
                    if (balance != null)
                    {
                        model.OpeningInventory.RegularUnits += balance.RegularUnits;
                        model.OpeningInventory.RegularValue += balance.RegularValue;
                        model.OpeningInventory.RepossessedUnits += balance.RepossessedUnits;
                        model.OpeningInventory.RepossessedValue += balance.RepossessedValue;
                        model.OpeningInventory.SparePartsUnits += balance.SparePartsUnits;
                        model.OpeningInventory.SparePartsValue += balance.SparePartsValue;
                    }
                }
                model.Rows = rowsQuery.ToList();
                return model;
            }
        }
    }
}