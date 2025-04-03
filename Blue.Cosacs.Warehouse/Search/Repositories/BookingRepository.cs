using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Search = Blue.Cosacs.Warehouse.Search;

namespace Blue.Cosacs.Warehouse.Repositories
{
    public class BookingRepository
    {
        public IEnumerable<Search.Booking> Get(int[] ids)
        {
            using (var scope = Context.Read())
            {
                var bookingView = (ids == null ? scope.Context.BookingForceView :
                                              scope.Context.BookingForceView.Where(p => ids.Contains(p.Id))).ToList();

                var bookings = (from b in bookingView
                                select new Search.Booking
                                   {
                                       BookingNo = b.Id,
                                       Customer = b.CustomerName,
                                       Address = b.Address,
                                       StockBranch = b.StockBranch,
                                       DeliveryBranch = b.DeliveryBranch,
                                       DelCol = b.DeliveryOrCollection == "D" ? "Delivery" : "Collection",
                                       DueOn = b.DeliveryOrCollectionDate,
                                       ItemNo = b.ItemNo,
                                       ItemId = b.Id,
                                       ItemUPC = b.ItemUPC,
                                       ItemDescription = b.ProductDescription,
                                       ProductCat = b.ProductCategory,
                                       ItemsCount = b.Quantity,
                                       CreatedOn = b.OrderedOn,
                                       Damaged = b.Damaged,
                                       PickListNo = b.PickListNo,
                                       Truck = b.Truck
                                   }).ToList();

                return bookings;
            }
        }
    }
}
