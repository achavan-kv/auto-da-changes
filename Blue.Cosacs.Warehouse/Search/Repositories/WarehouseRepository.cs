using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.Warehouse;

namespace Blue.Cosacs.Warehouse
{
    public class WarehouseRepository
    {
        public void BookingsSave(Booking booking)
        {
            using (var scope = Context.Write())
            {
                scope.Context.Booking.Add(booking);
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public IEnumerable<BookingPendingView> BookingsPending(int offset, int count, BookingSearch search = null)
        {
            using (var scope = Context.Read())
            {
                var query = 
                    from b in scope.Context.BookingPendingView
                    select b;

                if (search.DeliveryBranch.HasValue)
                    query = query.Where(b => b.DeliveryBranch == search.DeliveryBranch);
                if (!string.IsNullOrWhiteSpace(search.DeliveryZone))
                    query = query.Where(b => search.DeliveryZone.Contains(b.DeliveryZone));
                if (!string.IsNullOrWhiteSpace(search.ProductCategory))
                    query = query.Where(b => search.ProductCategory.Contains(b.ProductCategory));
                        
                return query.OrderBy(b => b.Id).Skip(offset).Take(count).ToList();
            }
        }

        //public IEnumerable<PicklistView> PicklistGet(int id)
        //{
        //    using (var scope = Context.Read())
        //    {
        //        return null;// return scope.Context.PicklistView.Where(p => p.Id == id).ToList();
        //    }
        //}

        //public IEnumerable<PickingIndex> PickingIndex()
        //{
        //    using (var scope = Context.Read())
        //    {
        //        var headers = scope.Context.PickingIndex.ToDictionary(t => t.Id);
        //        var children = (from c in scope.Context.PickingItem
        //                        join t in scope.Context.Truck on c.TruckId equals t.Id
        //                        where headers.Keys.Contains(c.PickingId.Value)
        //                        select new { c.PickingId, Truck = t.Name, c.BookingId })
        //                       .ToLookup(t => t.PickingId);

        //        foreach (var header in headers)
        //        {
        //            var child = children[header.Key]; // select base on pickingId to get multiple children
        //            header.Value.Trucks = (from c in child select c.Truck).OrderBy(t => t).Distinct();
        //            header.Value.Bookings = (from c in child select c.BookingId).OrderBy(t => t).Distinct();
        //        }

        //        return headers.Values;
        //    }
        //}
    }
}
