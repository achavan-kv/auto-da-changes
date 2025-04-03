namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public interface IStockMovementRepository
    {
        List<StockMovementTypeViewModel> GetStockMovements(int productId, int locationId, DateTime dateFrom, DateTime? dateTo);

        List<StockMovementReportModel> StockMovementReport(StockMovementQueryModel query);

        List<StockReceivedReportModel> StockReceivedReport(StockReceivedQueryModel query);
        List<StockMovementReportModel> GetStockMovementForNegativeStockReport(DateTime endDate, DataTable dtProductIds, DataTable dtProductLocationIds);
    }

    public class StockMovementRepository : IStockMovementRepository
    {
        public List<StockMovementTypeViewModel> GetStockMovements(int productId, int locationId, DateTime dateFrom, DateTime? dateTo)
        {
            return StockMovementReport(new StockMovementQueryModel
            {
                ProductId = productId,
                LocationId = locationId,
                CreatedFrom = dateFrom,
                CreatedTo = dateTo
            })
                .Select(x => new StockMovementTypeViewModel
                {
                    Date = x.DateProcessedUTC.Value.ToLocalDateTime(),
                    ID = x.TransactionId,
                    Movement = x.Quantity,
                    Type = x.Type,
                    IsDirect = x.IsDirect
                })
                .ToList();
        }

        private List<StockMovementTotalModel> CalculateEndStockLevels(List<StockMovementReportView> query, int? productId, int? locationId)
        {
            using (var scope = Context.Read())
            {
                var currentStockLevels = scope.Context.ProductStockLevel.AsQueryable();

                if (productId.HasValue)
                {
                    currentStockLevels = currentStockLevels.Where(c => c.ProductId == productId.Value);
                }

                if (locationId.HasValue)
                {
                    currentStockLevels = currentStockLevels.Where(c => c.LocationId == locationId.Value);
                }

                var stockLevels = currentStockLevels.Select(s => new { s.ProductId, s.StockOnHand, s.LocationId })
                                                    .ToDictionary(d => new { d.ProductId, d.LocationId }, d => d.StockOnHand);


                var aggregateQuantities = query.GroupBy(s => new { s.ProductId, s.LocationId })
                                               .Select(s => new { s.Key, total = s.Sum(t => t.Quantity) })
                                               .ToList();

                return (from a in aggregateQuantities
                        join s in stockLevels on a.Key equals s.Key into sj
                        from sl in sj.DefaultIfEmpty()
                        select new StockMovementTotalModel
                        {
                            LocationId = a.Key.LocationId,
                            ProductId = a.Key.ProductId,
                            Quantity = sl.Value - a.total.Value
                        }).ToList();
            }
        }

        private IQueryable<StockMovementReportView> FilterStockMovement(StockMovementQueryModel query)
        {
            using (var scope = Context.Read())
            {
                var smquery = scope.Context.StockMovementReportView.AsNoTracking().AsQueryable();

                if (query.CreatedFrom.HasValue)
                {
                    var createdFrom = query.CreatedFrom.Value.ToUniversalTime();
                    smquery = smquery.Where(x => x.DateProcessedUTC >= createdFrom);
                }

                if (query.CreatedTo.HasValue)
                {
                    var toDate = query.CreatedTo.Value.AddDays(1);
                    var toDateUTC = toDate.ToUniversalTime();
                    smquery = smquery.Where(x => x.DateProcessedUTC < toDateUTC);
                }

                if (query.Division != null)
                {
                    smquery = smquery.Where(m => query.Division == m.Division);
                };

                if (query.Department != null)
                {
                    smquery = smquery.Where(m => query.Department == m.Department);
                };

                if (query.Class != null)
                {
                    smquery = smquery.Where(m => query.Class == m.Class);
                };

                if (query.Fascia != null)
                {
                    smquery = smquery.Where(m => query.Fascia == m.Fascia);
                };

                if (query.LocationId != null)
                {
                    smquery = smquery.Where(m => query.LocationId == m.LocationId);
                };

                if (query.ProductId != null)
                {
                    smquery = smquery.Where(m => query.ProductId == m.ProductId);
                };

                if (query.Sku != null)
                {
                    smquery = smquery.Where(m => query.Sku == m.SKU);
                };

                return smquery;
            }
        }

        // NOTE:
        // The report combines UTC dates and local dates.
        // The front end behaves like DateUTC ?? Date, DateProcessedUTC ?? DateProcessed.
        // All UTC fields converted to local time. 
        // So if you fill UTC and local dates will be ignored.

        public List<StockMovementReportModel> StockMovementReport(StockMovementQueryModel query)
        {
            using (var scope = Context.Read())
            {
                var location = scope.Context.Location
                    .Select(p => new
                    {
                        p.Id,
                        p.Name
                    })
                    .ToDictionary(k => k.Id, v => v.Name);

                var product = scope.Context.Product
                                    .FirstOrDefault(p => p.SKU == query.Sku);
                if (product != null)
                {
                    query.ProductId = product.Id;
                }
                else
                {
                    query.Sku = null;
                }

                var smquery = FilterStockMovement(query).ToList();

                var levels = CalculateEndStockLevels(smquery, query.ProductId, query.LocationId)
                    .ToDictionary(d => new
                    {
                        d.LocationId,
                        d.ProductId
                    }, d => d.Quantity);

                var movements = smquery
                         .Select(p => new StockMovementReportModel()
                         {
                             Division = p.Division,
                             Department = p.Department,
                             Class = p.Class,
                             TransactionId = p.TransactionId,
                             ProductId = p.ProductId,
                             LongDescription = p.LongDescription,
                             BrandName = p.BrandName,
                             ProductTags = p.ProductTags,
                             SKU = p.SKU,
                             LocationId = p.LocationId,
                             Location = p.Location,
                             Type = p.Type,
                             Narration = string.Format(
                                 p.Narration,
                                 p.SendingLocationId.HasValue ? location[p.SendingLocationId.Value] : string.Empty,
                                 p.ReceivingLocationId.HasValue ? location[p.ReceivingLocationId.Value] : string.Empty),
                             Quantity = p.Quantity.HasValue ? p.Quantity.Value : 0,
                             Date = p.Date,
                             DateUTC = p.DateUTC,
                             DateProcessed = null,
                             DateProcessedUTC = p.DateProcessedUTC,
                             UserId = p.UserId,
                             User = p.User,
                             IsDirect = p.IsDirect,
                             StockLevel = levels.ContainsKey(new { p.LocationId, p.ProductId }) ? levels[new { p.LocationId, p.ProductId }] : 0,
                             IsHeader = false
                         })
                         .OrderBy(item => item.DateProcessedUTC)
                         .ToList();

                foreach (var productLocation in movements.GroupBy(r => new { r.LocationId, r.ProductId }))
                {
                    var previous = productLocation.FirstOrDefault();
                    if (previous == null)
                    {
                        continue;
                    }
                    previous.StockLevel += previous.Quantity;
                    previous.Tags = JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(previous.ProductTags);
                    foreach (var current in productLocation.Skip(1))
                    {
                        current.Tags = JsonConvertHelper.DeserializeObjectOrDefault<List<string>>(current.ProductTags);
                        current.StockLevel = previous.StockLevel + current.Quantity;
                        previous = current;
                    }
                }

                // Create opening balance rows
                var openingBalances =
                    movements
                        .GroupBy(x => new { x.LocationId, x.ProductId })
                          .Select(
                            g =>
                            g.OrderBy(x => x.DateProcessedUTC)
                            .Select(x => CreateOpeningBalance(x, query.CreatedFrom)).FirstOrDefault())
                        .ToList();

                // Create final data set
                movements =
                    movements
                        .Union(openingBalances)
                        .OrderBy(x => x.ProductId)
                        .ThenByDescending(x => x.IsHeader)
                        .ThenBy(x => x.DateProcessedUTC)
                        .ToList();


                return movements;
            }
        }

        private StockMovementReportModel CreateOpeningBalance(StockMovementReportModel template, DateTime? createdFromQuery)
        {
            var b = Mapper.Map<StockMovementReportModel>(template);
            var startDate = createdFromQuery.HasValue ? createdFromQuery.Value : template.DateProcessedUTC.Value.ToLocalTime();
            var startDateUTC = createdFromQuery.HasValue ? createdFromQuery.Value.ToUniversalTime() : template.DateProcessedUTC.Value;

            b.Type = "Opening Balance";
            b.Narration = string.Empty;
            b.Date = startDate;
            b.DateUTC = startDateUTC;
            b.DateProcessed = startDate;
            b.DateProcessedUTC = startDateUTC;
            b.User = string.Empty;
            b.Quantity = template.StockLevel - template.Quantity;
            b.StockLevel = template.StockLevel - template.Quantity;
            b.IsHeader = true;

            return b;
        }

        public List<StockReceivedReportModel> StockReceivedReport(StockReceivedQueryModel query)
        {
            using (var scope = Context.Read())
            {
                var locationquery = scope.Context.Location
                    .Select(p => new
                    {
                        p.Id,
                        p.Warehouse,
                        p.Fascia,
                        p.Name
                    });

                if (!string.IsNullOrEmpty(query.Fascia))
                {
                    locationquery = locationquery.Where(p => p.Fascia == query.Fascia || p.Warehouse);
                }

                var location = locationquery.ToDictionary(p => p.Id);

                var model = scope.Context.StockReceivedReportView.AsQueryable();

                if (!string.IsNullOrEmpty(query.Class))
                {
                    model = model.Where(p => p.Class == query.Class);
                }

                if (!string.IsNullOrEmpty(query.Department))
                {
                    model = model.Where(p => p.Department == query.Department);
                }

                if (!string.IsNullOrEmpty(query.Division))
                {
                    model = model.Where(p => p.Division == query.Division);
                }

                if (query.LocationId.HasValue)
                {
                    model = model.Where(p => p.LocationId == query.LocationId.Value);
                }

                if (query.VendorId.HasValue)
                {
                    model = model.Where(p => p.LocationId == query.LocationId.Value);
                }

                if (query.PurchaseOrderId.HasValue)
                {
                    model = model.Where(p => p.PurchaseOrderId == query.PurchaseOrderId.Value);
                }

                if (query.CreatedFrom.HasValue)
                {
                    var fromUTC = query.CreatedFrom.Value.ToUniversalTime();
                    model = model.Where(x => x.Date >= query.CreatedFrom.Value);
                }

                if (query.CreatedTo.HasValue)
                {
                    // var toUTC = query.CreatedTo.Value.ToUniversalTime().AddDays(1);
                    var d = query.CreatedTo.Value.AddDays(1);
                    model = model.Where(x => x.Date < d);
                }

                //apply the filter of facia that may be null but it has
                model = model.Where(p => location.Keys.Contains(p.LocationId));

                var results = model.ToList();

                //a list with only the product id's to use it as a filter criteria
                var allproducts = results
                    .Select(p => p.ProductId)
                    .Distinct()
                    .ToList();

                // Last date for that order.
                var lastReceivedValues = scope.Context.GoodsReceiptResume
                                         .GroupBy(g => g.PurchaseOrderId)
                                         .Select(p => new
                                         {
                                             key = p.Key,
                                             date = p.Max(x => x.DateReceived)
                                         }).ToDictionary(x => x.key, x => x.date);

                var pendingStockValues = scope.Context.StockReceivedReportStockCountView
                                            .ToDictionary(
                                                d => new
                                                {
                                                    d.ProductId,
                                                    d.LocationId,
                                                    d.PurchaseOrderId,
                                                    d.ReferenceNumberCsl
                                                },
                                                d => d.Pending);

                return results
                    .Select(p =>
                    {
                        var result = new StockReceivedReportModel();
                        var lastReceivedKey = new { p.ProductId, p.LocationId };
                        var pendingStockKey = new { p.ProductId, p.LocationId, p.PurchaseOrderId, p.ReferenceNumberCsl };

                        result.Class = p.Class;
                        result.DateLastReceived = lastReceivedValues.ContainsKey(p.PurchaseOrderId) ? lastReceivedValues[p.PurchaseOrderId] : (DateTime?)null;
                        result.Date = p.Date;
                        result.Department = p.Department;
                        result.Description = p.Description;
                        result.Division = p.Division;
                        result.ExtendedLandedCost = p.ExtendedLandedCost;
                        result.LastLandedCost = p.LastLandedCost;
                        result.Location = location[p.LocationId].Name;
                        result.LocationId = p.LocationId;
                        result.PendingStock = pendingStockValues.ContainsKey(pendingStockKey) ? pendingStockValues[pendingStockKey] : (int?)null;
                        result.ProductId = p.ProductId;
                        result.PurchaseOrderId = p.PurchaseOrderId;
                        result.Quantity = p.Quantity;
                        result.ReferenceNumberCsl = new ReferenceLink
                        {
                            Label = p.Quantity > 0 ? "GR" : "VR",
                            Id = p.ReferenceNumberCsl
                        };
                        result.ReferenceNumberExport = string.Format("{0}{1}", p.Quantity > 0 ? "GR" : "VR", p.ReferenceNumberCsl);
                        result.Sku = p.SKU;
                        result.StockOnHand = p.StockOnHand;
                        result.Vendor = p.Vendor;
                        result.VendorId = p.VendorId;

                        return result;
                    }).ToList();
            }
        }

        public List<StockMovementReportModel> GetStockMovementForNegativeStockReport(DateTime endDate, DataTable dtProductIds, DataTable dtProductLocationIds)
        {
            List<StockMovementReportModel> outputResult = new List<StockMovementReportModel>();
            using (var scope = Context.Read())
            {
                var result = scope.Context.GetStockMovementForNegativeStockReport(endDate, dtProductIds, dtProductLocationIds).Result;
                Mapper.CreateMap<StockMovementReportResult, StockMovementReportModel>();
                outputResult = Mapper.Map<IEnumerable<StockMovementReportModel>>(result).OrderBy(item => item.DateProcessedUTC).ToList();
            }
            return outputResult;
        }
    }
}