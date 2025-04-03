using System;
using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Warehouse.Repositories;

namespace Blue.Cosacs.Warehouse.Search
{
    public static class SolrIndex
    {
        public static IEnumerable<Booking> Booking(params int[] ids)
        {
            return new BookingRepository().SolrIndex(ids);
        }

        public static IEnumerable<PickList> Picking(int[] ids = null)
        {
            using (var scope = Context.Read())
            {
                var headers = (ids == null ?
                    scope.Context.PickingView :
                    scope.Context.PickingView.Where(p => ids.Contains(p.Id)))
                    .ToDictionary(t => t.Id);

                var items = (from b in scope.Context.Booking
                             join t in scope.Context.Truck on b.TruckId equals t.Id
                             where b.PickingId != null
                             select new { b.PickingId, Truck = t.Name, b.Id });

                var children = (ids == null ? items : items.Where(i => ids.Contains(i.PickingId.Value)))
                               .ToLookup(t => t.PickingId);

                var trucks = new Dictionary<int, string[]>();
                var bookings = new Dictionary<int, int[]>();
                foreach (var header in headers)
                {
                    var child = children[header.Key]; // select base on pickingId to get multiple children
                    trucks.Add(header.Value.Id, (from c in child select c.Truck).OrderBy(t => t).Distinct().ToArray());
                    bookings.Add(header.Value.Id, (from c in child select c.Id).OrderBy(t => t).Distinct().ToArray());
                }

                var query = headers.Values.AsQueryable();

                var indexItems =
                    (from p in query
                     select new Blue.Cosacs.Warehouse.Search.PickList
                     {
                         PickListNo = p.Id,

                         CreatedOn = p.CreatedOn.ToSolrDate(),
                         ConfirmedOn = p.ConfirmedOn.HasValue ? p.ConfirmedOn.Value.ToSolrDate() : null,
                         PickedOn = p.PickedOn.HasValue ? p.PickedOn.Value.ToSolrDate() : null,

                         PickingEmployees = new[] { p.CheckedByName, p.CreatedByName, p.PickedByName, p.ConfirmedByName }.Distinct().ToArray(),
                         ConfirmedBy = p.ConfirmedByName,
                         CreatedBy = p.CreatedByName,
                         PickedBy = p.PickedByName,
                         CheckedBy = p.CheckedByName,
                         PickingStatus = String.IsNullOrWhiteSpace(p.PickedByName) ? "Created" : "Confirmed",

                         Trucks = trucks[p.Id],
                         BookingIds = bookings[p.Id],
                         StockBranchName = String.Format("{0} {1}", p.BranchName, p.BranchNumber)
                     }).ToList();
                new Blue.Solr.WebClient().Update(indexItems);
                return indexItems;
            }
        }

        // Never make this constant smaller that 1000!!! Or indexing deliveries by id will probably break
        private const int indexingReadSize = 10000; // more explanations bellow

        public static IEnumerable<Load> Index(int[] deliveryIds = null)
        {
            var deliveriesCount = 0L;

            using (var scope = Context.Read())
            {
                if (deliveryIds != null)
                {
                    deliveriesCount = scope.Context.LoadView
                        .Where(e => deliveryIds.Contains(e.Id))
                        .Select(p => p.Id)
                        .Max();
                }
                else
                {
                    deliveriesCount = scope.Context.LoadView
                        .Select(p => p.Id)
                        .Max();
                }
            }

            var rowsToSkip = 0;
            var correctNonSequential = 0;
            for (int i = 0; i * indexingReadSize < deliveriesCount; i++)
            {
                rowsToSkip = correctNonSequential != 0 ? correctNonSequential : i * indexingReadSize;
                correctNonSequential = 0;

                List<LoadView> loadHeaders = null;

                List<Search.Load> loads = new List<Load>();
                using (var scope = Context.Read())
                {
                    var orderedLoadView = scope.Context.LoadView.OrderBy(e => e.Id);
                    if (deliveryIds != null)
                    {
                        loadHeaders = (from l in orderedLoadView
                                           .Where(e => deliveryIds.Contains(e.Id) && e.Id > rowsToSkip)
                                           .Take(indexingReadSize)
                                       select l).ToList();
                    }
                    else
                    {
                        loadHeaders = (from l in orderedLoadView
                                           .Where(e => e.Id > rowsToSkip)
                                           .Take(indexingReadSize)
                                       select l).ToList();
                    }

                    var hIds = loadHeaders.Select(h => h.Id).ToList();
                    var loadedChildren = scope.Context.ScheduleView.Where(s => s.ScheduleID.HasValue && hIds.Contains(s.ScheduleID.Value)).ToLookup(s => s.ScheduleID);

                    if (loadedChildren.Count > 0)
                    {
                        foreach (var head in loadHeaders)
                        {
                            var loaditems = loadedChildren[head.Id];

                            loads.Add(new Load
                            {
                                LoadId = head.Id,
                                CreatedOn = head.CreatedOn.ToSolrDate(),
                                CreatedBy = head.CreatedByName,
                                DeliveryEmployees = new[] { head.CreatedByName },
                                Truck = loaditems.FirstOrDefault() == null ? string.Empty : loaditems.FirstOrDefault().TruckName,
                                Driver = head.DriverName,
                                //ItemsCount = loaditems.Sum(x => x.CurrentQuantity.Value),
                                ItemsCount = loaditems.Sum(x => x.ScheduleQuantity.Value),                  //#11484
                                DeliveryBranchName = loaditems.FirstOrDefault() == null ? string.Empty : String.Format("{0} {1}", loaditems.FirstOrDefault().DeliveryBranchName, loaditems.FirstOrDefault().DeliveryBranch),
                                DeliveryZones = loaditems.Select(s => s.DeliveryZone).Where(z => !string.IsNullOrWhiteSpace(z)).ToArray(),
                                ConfirmedOn = head.ConfirmedOn.HasValue ? head.ConfirmedOn.Value.ToSolrDate() : null,
                                ConfirmedBy = head.ConfirmedByName,
                                DeliveryStatus = head.ConfirmedOn.HasValue ? "Delivered" : "Scheduled",
                            });
                        }
                    }
                }

                // because the LoadId is not sequential
                correctNonSequential = loads.Any() ? loads.Max(e => e.LoadId) : 0;

                new Blue.Solr.WebClient().Update(loads);

                if (deliveryIds != null)
                {
                    // I'm assuming here that when indexing deliveries by id, the number of id's will be 
                    // in the order of tenths (maybe a couple of hundred max), and this should ALLWAYS
                    // be smaller than the indexingReadSize (which will be in the order of thousands).
                    // This is why all indexing results are being returned in this first for iteration.
                    return loads;
                }
            }

            return null;
        }
    }
}


