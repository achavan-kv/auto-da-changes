namespace Blue.Cosacs.Merchandising.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using Newtonsoft.Json;
    using System.IO;

    public interface IWarehouseOversupplyRepository
    {
        WarehouseOversupplyViewModel WarehouseOverSupplyReport(WarehouseOversupplySearchModel search);

        List<WarehouseOversupplyExportModel> WarehouseOverSupplyExport(WarehouseOversupplySearchModel search);
    }

    public class WarehouseOversupplyRepository : IWarehouseOversupplyRepository
    {
        private readonly IProductRepository productRepository;
        private readonly IMerchandisingHierarchyRepository merchandisingHierarchyRepository;

        public WarehouseOversupplyRepository(IProductRepository productRepository, IMerchandisingHierarchyRepository merchandisingHierarchyRepository)
        {
            this.productRepository = productRepository;
            this.merchandisingHierarchyRepository = merchandisingHierarchyRepository;
        }

        public WarehouseOversupplyViewModel WarehouseOverSupplyReport(WarehouseOversupplySearchModel search)
        {             
            return FilterReport(search);
        }

        public List<WarehouseOversupplyExportModel> WarehouseOverSupplyExport(WarehouseOversupplySearchModel search)
        {
            var export = new List<WarehouseOversupplyExportModel>();
            var report = FilterReport(search);
            foreach (var item in report.Products)
            {
                var exportItem = Mapper.Map<WarehouseOversupplyExportModel>(item);
                exportItem.Division = item.HierarchyTags[0];
                exportItem.Department = item.HierarchyTags[1];
                exportItem.Class = item.HierarchyTags[2];
                export.Add(exportItem);
            }

            return export;
        }

        private WarehouseOversupplyViewModel FilterReport(WarehouseOversupplySearchModel search)
        {
            using (var scope = Context.Read())
            {                
                var report = scope.Context.WarehouseOversupplyReportView
                                .Where(w => w.WarehouseId == search.WarehouseId.Value || !search.WarehouseId.HasValue)
                                .Where(w => w.Fascia == search.Fascia || string.IsNullOrEmpty(search.Fascia));

                var productIds = report.Select(p => p.ProductId).ToList();
                var prodHier = scope.Context.ProductHierarchyView.Where(p => productIds.Contains(p.ProductId)).ToList();
                var levels = merchandisingHierarchyRepository.GetAllLevels().ToList();

                var tags = scope.Context.TagView.ToList();

                if (search.WarehouseId.HasValue)
                {
                    search.Warehouse = scope.Context.Location.First(l => l.Id == search.WarehouseId).Name;
                }

                // Limit by product hierarchy
                if (search.Hierarchy != null)
                {
                    foreach (var level in search.Hierarchy)
                    {
                        var remainingIds =
                            scope.Context.ProductHierarchyView.Where(
                                p =>
                                productIds.Contains(p.ProductId))
                                .AsEnumerable()
                                .Where(p => p.LevelId == int.Parse(level.Key) && p.TagId == int.Parse(level.Value))
                                .Select(p => p.ProductId);
                        report = report.Where(o => remainingIds.Contains(o.ProductId));
                    }

                    search.HierarchyString = string.Join(
                        ", ",
                        search.Hierarchy.Select(
                            h => levels.First(l => l.Id == int.Parse(h.Key)).Name + " - " + tags.First(t => t.LevelId == int.Parse(h.Key) && t.Id == int.Parse(h.Value)).TagName));
                }

                // Limit by tags 
                var result = ((search.Tags != null)
                                 ? report.Where(o => o.Tags != null)
                                       .AsEnumerable()
                                       .Where(
                                           o =>
                                           ((List<string>)
                                            new JsonSerializer().Deserialize(
                                                new StringReader(o.Tags),
                                                typeof(List<string>))).Any(t => search.Tags.Any(s => s == t)))
                                 : report).ToList();

                var products = MapToViewModel(result, prodHier, levels);
                return new WarehouseOversupplyViewModel 
                {
                    Levels = levels,
                    Products = products
                };
            }
        }

        private List<WarehouseOversupplyProductViewModel> MapToViewModel(List<WarehouseOversupplyReportView> view, List<ProductHierarchyView> hierarchy, List<Level> levels)
        {
            using (var scope = Context.Read())
            {
                var locations = scope.Context.Location.ToList();
               
                return view.Select(
                    p =>
                        {
                            var hierarchies = hierarchy.Where(h => h.ProductId == p.ProductId).ToList();

                            var missing = levels.Where(l => !hierarchies.Select(h => h.LevelId).Contains(l.Id));

                            hierarchies.AddRange(
                                missing.Select(m => new ProductHierarchyView { LevelId = m.Id, ProductId = p.ProductId, Tag = string.Empty }));

                            return new WarehouseOversupplyProductViewModel
                                       {
                                           Id = p.Id,
                                           ProductId = p.ProductId,
                                           Sku = p.Sku,
                                           LongDescription = p.LongDescription,
                                           WarehouseId = p.WarehouseId,
                                           WarehouseName = p.WarehouseName,
                                           StockOnHandInWarehouse = p.StockOnHandInWarehouse,
                                           LocationId = p.LocationId,
                                           LocationName = p.LocationName,
                                           DateLastReceived = p.DateLastReceived,
                                           StockRequisitionPending = p.StockRequisitionPending,
                                           LocationsAssigned =
                                               p.StoreTypes == null
                                                   ? locations.Count
                                                   : locations.Where(
                                                       l =>
                                                       JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(p.StoreTypes)
                                                           .Contains(l.StoreType)).ToList().Count,
                                           HierarchyTags =
                                               hierarchies
                                               .OrderBy(h => h.LevelId)
                                               .Select(h => h.Tag)
                                               .ToList()
                                       };
                        }).ToList();
            }
        } 
    }
}