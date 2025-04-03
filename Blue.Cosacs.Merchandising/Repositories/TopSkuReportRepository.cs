namespace Blue.Cosacs.Merchandising.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Models;

    using Newtonsoft.Json;

    public interface ITopSkuReportRepository
    {
        TopSkuViewModel TopSkuReport(TopSkuSearchModel search);

        IEnumerable<ExpandoObject> GetTopSkuExport(
            List<Level> levels,
            List<TopSkuLocationViewModel> locations);
    }

    public class TopSkuTopSkuReportRepository : ITopSkuReportRepository
    {
        private readonly IMerchandisingHierarchyRepository merchHierarchyRepository;

        public TopSkuTopSkuReportRepository(IMerchandisingHierarchyRepository merchHierarchyRepository)
        {
            this.merchHierarchyRepository = merchHierarchyRepository;
        }

        public TopSkuViewModel TopSkuReport(TopSkuSearchModel search)
        {
            var orders = FilterOrders(search);
            var mapped = MapForPerformanceType(search, orders);

            // Get product tags and hierarchies
            var levels = merchHierarchyRepository.GetAllLevels().ToList();
            var mappedIds = mapped.SelectMany(o => o.Value.Select(v => v.ProductId));
            using (var scope = ReportingContext.Read())
            {
                var prodHier = scope.Context.ProductHierarchyView.Where(p => mappedIds.Contains(p.ProductId)).ToList();
                foreach (var group in mapped)
                {
                    foreach (var prod in group.Value)
                    {
                        // Hierarchy tags
                        var hierarchyTags = prodHier.Where(p => p.ProductId == prod.ProductId);
                        var matchLevel = levels.Select(lev => hierarchyTags.FirstOrDefault(ht => ht.LevelId == lev.Id));
                        foreach (var match in matchLevel)
                        {
                            prod.HierarchyTags.Add(match == null ? string.Empty : match.Tag);
                        }

                        // Product tags
                        var tags = orders.Single(o => o.Key == group.Key).Single(o => o.Id == prod.Id).Tags;
                        if (!string.IsNullOrWhiteSpace(tags))
                        {
                            prod.Tags = Regex.Replace(tags, @"[\[\]""]+", string.Empty);
                        }
                    }
                }
            }

            return new TopSkuViewModel()
            {
                Levels = levels,
                Locations = mapped.Select(m => 
                    new TopSkuLocationViewModel()
                        {
                            Location = m.Key,
                            Products = m.Value
                        }).ToList()
            };
        }

        public IEnumerable<ExpandoObject> GetTopSkuExport(List<Level> levels, List<TopSkuLocationViewModel> locations)
        {
            var export = new List<ExpandoObject>();
            foreach (var loc in locations)
            {
                foreach (var prod in loc.Products)
                {
                    var exportRow = new ExpandoObject() as IDictionary<string, object>;
                    exportRow.Add("LocationName", prod.LocationName);
                    exportRow.Add("Fascia", prod.Fascia);

                    for (var i = 0; i < levels.Count; i++)
                    {
                        exportRow.Add(levels[i].Name, prod.HierarchyTags[i]);
                    }

                    exportRow.Add("Sku", prod.Sku);
                    exportRow.Add("LongDescription", prod.LongDescription);
                    exportRow.Add("BrandName", prod.BrandName);
                    exportRow.Add("Tags", prod.Tags);
                    exportRow.Add("QuantityDelivered", prod.QuantityDelivered);
                    exportRow.Add("ValueDelivered", prod.ValueDelivered);
                    exportRow.Add("NetValueDelivered", prod.NetValueDelivered);
                    exportRow.Add("Contribution", prod.Contribution);
                    exportRow.Add("CumulativeValueDelivered", prod.CumulativeValueDelivered);
                    exportRow.Add("CumulativeNetValueDelivered", prod.CumulativeNetValueDelivered);
                    export.Add((ExpandoObject)exportRow);
                }
            }

            return export;
        }

        private IEnumerable<IGrouping<string, TopSkuReportView>> FilterOrders(TopSkuSearchModel search)
        {
            using (var scope = ReportingContext.Read())
            {
                // Limit by date, location and fascia
                var orders =
                    scope.Context.TopSkuReportView.Where(
                        o =>
                        (string.IsNullOrEmpty(search.Fascia) || o.Fascia == search.Fascia)
                        && (!search.LocationId.HasValue || o.LocationId == search.LocationId)
                        && (!search.FromDate.HasValue || o.TransactionDate >= search.FromDate)
                        && (!search.ToDate.HasValue || o.TransactionDate <= search.ToDate));

                // Limit by store type
                if (search.LocationId.HasValue)
                {
                    var locStoreType = string.Format("\"{0}\"", scope.Context.Location.Single(l => l.Id == search.LocationId).StoreType);
                    orders = orders.Where(o => string.IsNullOrEmpty(o.StoreTypes) || o.StoreTypes.Contains(locStoreType)); 
                }
               
                // Limit by product hierarchy
                if (search.Hierarchy != null)
                {
                    foreach (var level in search.Hierarchy)
                    {
                        var productIds = orders.Select(o => o.ProductId);
                        var remainingIds =
                            scope.Context.ProductHierarchyView.Where(
                                p =>
                                productIds.Contains(p.ProductId))
                                .AsEnumerable()
                                .Where(p => p.LevelId == int.Parse(level.Key) && p.TagId == int.Parse(level.Value))
                                .Select(p => p.ProductId);
                        orders = orders.Where(o => remainingIds.Contains(o.ProductId));
                    }
                }

                // Limit by tags 
                var result = ((search.Tags != null)
                                 ? orders.Where(o => o.Tags != null)
                                       .AsEnumerable()
                                       .Where(
                                           o =>
                                           ((List<string>)
                                            new JsonSerializer().Deserialize(
                                                new StringReader(o.Tags),
                                                typeof(List<string>))).Any(t => search.Tags.Any(s => s == t)))
                                 : orders).ToList();

                // Give meaningful location and fascia names if none selected
                if (!search.GroupByLocation)
                {
                    result.ForEach(
                        o =>
                            {
                                o.Fascia = search.Fascia == null ? "All Fascias" : o.Fascia;
                                o.LocationName = search.LocationId == null ? "All Locations" : o.LocationName;
                            });
                }

                return result.GroupBy(m => m.Fascia + " - " + m.LocationName).ToList();
            }
        }
        
        private Dictionary<string, List<TopSkuProductViewModel>> MapForPerformanceType(TopSkuSearchModel search, IEnumerable<IGrouping<string, TopSkuReportView>> orders)
        {
            var mapped = new Dictionary<string, List<TopSkuProductViewModel>>();
            foreach (var group in orders)
            {
                var mappedGroup = Mapper.Map<List<TopSkuProductViewModel>>(group);
                switch (search.PerformanceType)
                {
                    case TopSkuPerformanceType.GrossMargin:

                        var productIds = group.Select(o => o.ProductId).ToList();
                        using (var scope = ReportingContext.Read())
                        {
                            var costs = scope.Context.CostPriceView.Where(c => productIds.Contains(c.ProductId));

                            foreach (var ord in group)
                            {
                                var cost =
                                    costs.Where(
                                        c => c.ProductId == ord.ProductId && c.AverageWeightedCostUpdated <= ord.TransactionDate)
                                        .OrderByDescending(c => c.AverageWeightedCostUpdated)
                                        .FirstOrDefault();

                                var mappedOrder = mappedGroup.Single(m => m.Id == ord.Id);
                                mappedOrder.Cost = cost == null ? 0 : cost.AverageWeightedCost ?? 0;
                            }
                        }
                        mappedGroup = CalculatePerformance(
                            mappedGroup,
                            o =>
                                {
                                    o.Margin = o.ValueDelivered - (o.Cost * o.QuantityDelivered);
                                    return o.Margin;
                                }, 
                                search.PerformancePercentage);
                        break;
                    case TopSkuPerformanceType.NetSalesValue:
                        mappedGroup = CalculatePerformance(mappedGroup, o => o.NetValueDelivered, search.PerformancePercentage);
                        break;
                    case TopSkuPerformanceType.Units:
                        mappedGroup = CalculatePerformance(mappedGroup, o => o.QuantityDelivered, search.PerformancePercentage);
                        break;
                }
                mapped.Add(group.Key, mappedGroup);
            }
            return mapped;
        }

        private List<TopSkuProductViewModel> CalculatePerformance(List<TopSkuProductViewModel> products, Func<TopSkuProductViewModel, decimal> contributionProperty, int percentage)
        {
            var profitTally = 0M;
            var grossTally = 0M;
            var netTally = 0M;
            
            // Aggregate by product
            var prodsGroups = products.Where(p => contributionProperty(p) > 0M).GroupBy(g => g.ProductId);
            var prodsToUse = new List<TopSkuProductViewModel>();
            foreach (var group in prodsGroups)
            {
                var prodTally = Mapper.Map<TopSkuProductViewModel>(group.First());
                prodTally.QuantityDelivered = group.Sum(g => g.QuantityDelivered);
                prodTally.ValueDelivered = group.Sum(g => g.ValueDelivered);
                prodTally.NetValueDelivered = group.Sum(g => g.NetValueDelivered);
                prodTally.Cost = group.Sum(g => g.Cost * g.QuantityDelivered) / group.Sum(g => g.QuantityDelivered);
                prodsToUse.Add(prodTally);
            }
          
            // Calculate contributions
            var totalValue = Math.Max(prodsToUse.Sum(contributionProperty), 1);
            prodsToUse = prodsToUse.OrderByDescending(contributionProperty).ToList();
            foreach (var item in prodsToUse)
            {
                grossTally += item.ValueDelivered;
                netTally += item.NetValueDelivered;
                item.CumulativeNetValueDelivered = netTally;
                item.CumulativeValueDelivered = grossTally;
                item.Contribution = contributionProperty(item) / totalValue;
                profitTally += item.Contribution * 100;
                if (profitTally >= percentage)
                {
                    break;
                }
            }
         
            return prodsToUse.Where(p => p.Contribution > 0).ToList();
        }
    }
}
