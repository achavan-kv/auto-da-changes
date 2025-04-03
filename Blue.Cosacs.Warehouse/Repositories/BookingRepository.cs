using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace Blue.Cosacs.Warehouse.Repositories
{
    using Blue.Cosacs.Messages.Merchandising.BookingMessage;
    using Blue.Cosacs.Messages.Warehouse;
    using Blue.Cosacs.Warehouse.Common;
    using Blue.Cosacs.Warehouse.Utils;
    using Blue.Hub.Client;

    public class BookingRepository
    {
        public IList<BookingPendingView> Pending(int? offset = null, int? count = null, BookingSearch search = null)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.BookingPendingView
                    .Where(b => b.TruckId == null && !b.Exception && b.CancellationId == null);

                if (search != null)
                {
                    query = query.Where(Expressions.IsInternal<BookingPendingView>(search.Internal));

                    if (search.DeliveryBranch.HasValue)
                    {
                        query = query.Where(b => b.DeliveryBranch == search.DeliveryBranch);
                    }
                    else // don't return anything if no branch is used to filter
                    {
                        return new List<BookingPendingView>();
                    }

                    if (!string.IsNullOrWhiteSpace(search.DeliveryZone))
                    {
                        query = query.Where(b => search.DeliveryZone.Contains(b.DeliveryZone));
                    }

                    if (!string.IsNullOrWhiteSpace(search.ProductCategory))
                    {
                        query = query.Where(b => search.ProductCategory.Contains(b.ProductCategory));
                    }

                    if (!string.IsNullOrWhiteSpace(search.Fascia))
                    {
                        query = query.Where(b => search.Fascia.Contains(b.Fascia));
                    }

                    if (search.ReceivingLocation.HasValue)
                    {
                        query = query.Where(b => b.ReceivingLocation == search.ReceivingLocation);
                    }

                    query = query.Where(b => b.PickUp == false);
                    query = query.OrderByDescending(b => b.DeliveryOrCollection == "T" ? 3 : (b.DeliveryOrCollection == "A" ? 2 : 1))
                        .ThenBy(b => b.Express)
                        .ThenBy(b => b.DeliveryOrCollectionDate);

                    if (count.HasValue && offset.HasValue)
                    {
                        query = query.Skip(offset.Value).Take(count.Value);
                    }
                }

                return query.ToList();
            }
        }

        //#10703 return number of bookings in search criteria
        public int AllBookings(BookingSearch search = null)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.BookingPendingView.AsQueryable();
                var total = 0;
                var queryparms = false;

                if (search != null)
                {
                    if (search.DeliveryBranch.HasValue)
                    {
                        query = query.Where(b => b.DeliveryBranch == search.DeliveryBranch);
                        queryparms = true;
                    }

                    if (!string.IsNullOrWhiteSpace(search.DeliveryZone))
                    {
                        query = query.Where(b => search.DeliveryZone.Contains(b.DeliveryZone));
                        queryparms = true;
                    }

                    if (!string.IsNullOrWhiteSpace(search.ProductCategory))
                    {
                        query = query.Where(b => search.ProductCategory.Contains(b.ProductCategory));
                        queryparms = true;
                    }

                    if (!string.IsNullOrWhiteSpace(search.Fascia))
                    {
                        query = query.Where(b => search.Fascia.Contains(b.Fascia));
                        queryparms = true;
                    }

                    query = query.Where(Expressions.IsInternal<BookingPendingView>(search.Internal))
                        .Where(b => b.PickUp == false &&
                            b.DeliverQuantity == null &&
                            b.PickingId == null &&
                            b.CancellationId == null &&
                            b.TruckId == null &&
                            b.Exception == false &&
                            (!search.ReceivingLocation.HasValue || b.ReceivingLocation == search.ReceivingLocation));
                }

                // only query if parameters entered
                if (queryparms)
                {
                    total = query.Count();
                }

                return total;
            }
        }

        //#10703 return number of bookings in search criteria
        public int AllCustomerPickUps(BookingSearch search = null)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.BookingPendingView.AsQueryable();
                var total = 0;
                var queryparms = false;

                if (search != null)
                {
                    if (search.DeliveryBranch.HasValue)
                    {
                        query = query.Where(b => b.DeliveryBranch == search.DeliveryBranch);
                        queryparms = true;
                    }

                    query = query
                        .Where(Expressions.IsInternal<BookingPendingView>(search.Internal))
                        .Where(b => b.PickUp);

                }

                // only query if parameters entered
                if (queryparms)
                    total = query.Count();

                return total;
            }
        }


        public void ResolveBookings(int id, DateTime DelColDate, string time)
        {
            using (var scope = Context.Write())
            {
                var booking = scope.Context.Booking.Find(id);
                booking.Exception = false;
                booking.OrderedOn = DateTime.UtcNow;        //13744
                booking.DeliveryOrCollectionDate = DelColDate;
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void CancelBookings(int id, int user, string notes, DateTime now)
        {
            using (var scope = Context.Write())
            {
                var booking = scope.Context.Booking.Find(id);
                booking.Exception = false;

                if (DeliveryOrCollection.MerchandisingTypes().Contains(booking.DeliveryOrCollection))
                {
                    scope.Context.Cancellation.Add(new Cancellation
                    {
                        Id = id,
                        Date = DateTime.UtcNow,
                        Reason = notes,
                        UserId = user
                    });

                    if (booking.PickUp)
                    {
                        booking.PickingRejected = true;
                        booking.PickQuantity = 0;
                    }
                    else
                    {
                        booking.ScheduleQuantity = 0;
                        booking.ScheduleComment = notes;
                        booking.ScheduleRejected = true;
                    }
                    scope.Context.SaveChanges();

                    var message = new BookingMessage
                    {
                        BookingId = Convert.ToInt32(booking.Path.Substring(0, booking.Path.IndexOf('.'))),
                        Quantity = booking.Quantity,
                        Type = booking.DeliveryOrCollection,
                        AverageWeightedCost = booking.UnitPrice
                    };
                    new Mhub().Cancel(message);
                }
                else
                {
                    var bookingCancel = new Message<WarehouseCancel>
                    {
                        CorrelationId = booking.AcctNo,
                        Payload = new WarehouseCancel
                        {
                            Id = id,
                            Comment = notes,
                            User = user,
                            DateTime = now,
                            Quantity = booking.CurrentQuantity.Value,
                            OrigBookingId = Convert.ToInt32(booking.Path.Substring(0, booking.Path.IndexOf('.')))
                        }
                    };
                    new Chub().Cancel(bookingCancel);

                    scope.Context.Cancellation.Add(new Cancellation
                    {
                        Id = id,
                        Date = DateTime.UtcNow,
                        Reason = notes,
                        UserId = user
                    });

                    if (booking.PickUp)
                    {
                        booking.PickingRejected = true;
                        booking.PickQuantity = 0;
                    }
                    else
                    {
                        booking.ScheduleQuantity = 0;
                        booking.ScheduleComment = notes;
                        booking.ScheduleRejected = true;
                    }
                    scope.Context.SaveChanges();
                }

                scope.Complete();
            }
        }

        public IEnumerable<Booking> BookingDetail(int id)
        {
            // this is utter madness...THIS IS SPARTAAA
            using (var scope = Context.Read())
            {
                var list = new List<Booking>();
                var ds = new DataSet();
                new BookingDetails() { id = id }.Fill(ds);

                var ids = new List<int>();

                foreach (DataRow row in ds.Tables[0].Rows)
                    ids.Add(Convert.ToInt32(row["Id"]));

                var status = (from s in scope.Context.StatusView
                              where ids.Contains(s.Id)
                              select s).ToDictionary(x => x.Id);

                foreach (DataRow b in ds.Tables[0].Rows)
                {
                    var booking = new Booking
                    {
                        Id = b.Field<int>("Id"),
                        CustomerName = b.Field<string>("CustomerName"),
                        AcctNo = b.Field<string>("AcctNo"),
                        AddressLine1 = b.Field<string>("AddressLine1"),
                        AddressLine2 = b.Field<string>("AddressLine2"),
                        AddressLine3 = b.Field<string>("AddressLine3"),
                        PostCode = b.Field<string>("PostCode"),
                        StockBranch = b.Field<short>("StockBranch"),
                        DeliveryBranch = b.Field<short>("DeliveryBranch"),
                        DeliveryOrCollection = b.Field<string>("DeliveryOrCollection"),
                        DeliveryOrCollectionDate = b.Field<DateTime>("DeliveryOrCollectionDate"),
                        DeliveryOrCollectionSlot = b.Field<string>("DeliveryOrCollectionSlot"),
                        ItemNo = b.Field<string>("ItemNo"),
                        ItemId = b.Field<int>("Id"),
                        ItemUPC = b.Field<string>("ItemUPC"),
                        NonStockServiceType = b.Field<string>("NonStockServiceType"),
                        NonStockServiceItemNo = b.Field<string>("NonStockServiceItemNo"),
                        NonStockServiceDescription = b.Field<string>("NonStockServiceDescription"),
                        ProductDescription = b.Field<string>("ProductDescription"),
                        ProductCategory = b.Field<string>("ProductCategory"),
                        Quantity = b.Field<short>("Quantity"),
                        OrderedOn = b.Field<DateTime>("OrderedOn").ToLocalTime(),
                        Damaged = b.Field<bool>("Damaged"),
                        PickingId = b.Field<int?>("PickingId"),
                        ScheduleId = b.Field<int?>("ScheduleId"),
                        TruckId = b.Field<int?>("TruckId"),
                        ContactInfo = b.Field<string>("ContactInfo"),
                        OriginalId = b.Field<int?>("OriginalId"),
                        Comment = b.Field<string>("Comment"),
                        PickingRejected = b.Field<bool?>("PickingRejected"),
                        PickingRejectedReason = b.Field<string>("PickingRejectedReason"),
                        PickQuantity = b.Field<int?>("PickQuantity"),
                        CurrentQuantity = b.Field<int?>("CurrentQuantity"),
                        Exception = b.Field<bool>("Exception"),
                        Status = BookingStatus.GetStatus(status[b.Field<int>("Id")]), // SHEER INSANITY
                        UnitPrice = b.Field<decimal>("UnitPrice"),
                        PickUp = b.Field<bool>("PickUp")
                    };
                    list.Add(booking);
                }
                return list;
            }
        }

        // Never make this constant smaller that 1000!!! Or indexing bookings by id will probably break
        private const int indexingReadSize = 50000; // more explanations bellow (inside the for cycle)

        public IEnumerable<Search.Booking> SolrIndex(int[] bookingIds = null)
        {
            var bookingCount = 0L;

            var status = new Dictionary<int, StatusView>();
            using (var scope = Context.Read())
            {
                if (bookingIds != null && bookingIds.Length > 0)
                {
                    // CAREFULL!!! ONLY TOUCH IF YOU'RE REWRITING THE SOLR INDEXING FROM SCRATCH
                    if (bookingIds.Length == 0) // if bookingIds is defined, but has no elements, abort solr indexing
                    {
                        // THIS premature return, fixes an exception when using an empty array (bookingIds) with linq
                        // The exception was: [ System.InvalidOperationException: The cast to value type System.Int32
                        // failed because the materialized value is null. Either the result type's generic parameter
                        // or the query must use a nullable type. ]
                        return null;
                    }

                    bookingCount = scope.Context.BookingForceView
                        .Where(e => bookingIds.Contains(e.Id))
                        .Select(p => p.Id)
                        .Max();

                    // Don't want to pass extra columns to search so load the StatusView separately.
                    status = scope.Context.StatusView
                        .Where(e => bookingIds.Contains(e.Id))
                        .ToDictionary(e => e.Id);
                }
                else
                {
                    bookingCount = scope.Context.BookingForceView
                        .Select(p => p.Id)
                        .Max();

                    // Don't want to pass extra columns to search so load the StatusView separately.
                    status = scope.Context.StatusView
                        .ToDictionary(e => e.Id);
                }
            }

            var rowsToSkip = 0;
            var correctNonSequential = 0;
            var tmpBookings = new List<BookingForceView>();
            for (int i = 0; i * indexingReadSize < bookingCount; i++)
            {
                rowsToSkip = correctNonSequential != 0 ? correctNonSequential : i * indexingReadSize;
                correctNonSequential = 0;

                IEnumerable<Search.Booking> bookings = null;
                using (var scope = Context.Read())
                {
                    if (bookingIds != null)
                    {
                        tmpBookings = (from b in scope.Context.BookingForceView
                                           .Where(e => bookingIds.Contains(e.Id) && e.Id > rowsToSkip)
                                           .Take(indexingReadSize)
                                       select b).ToList();

                        if (bookingIds.Length < indexingReadSize)
                        {
                            // I'm assuming here that when indexing bookings by id, the number of id's will be 
                            // in the order of tenths (maybe a couple of hundred max), and this should ALLWAYS
                            // be smaller than the indexingReadSize (which will be in the order of thousands).
                            // This is why all indexing results are being returned in this first for iteration.
                            bookings = Assign(tmpBookings, status);

                            // indexing bookings by id before returning them
                            new Blue.Solr.WebClient().Update(bookings);

                            // all done
                            return bookings;
                        }
                    }
                    else
                    {
                        tmpBookings = (from b in scope.Context.BookingForceView
                                           .Where(e => e.Id > rowsToSkip)
                                           .Take(indexingReadSize)
                                       select b).ToList();
                    }
                }

                if (tmpBookings.Count == 0)
                    break;

                // because the id is not sequential
                correctNonSequential = tmpBookings.Max(e => e.Id);

                bookings = Assign(tmpBookings, status);

                new Blue.Solr.WebClient().Update(bookings);
            }

            // Only returns indexed bookins when the bookings are getting indexed by Id
            return null;
        }

        private IEnumerable<Search.Booking> Assign(IEnumerable<BookingForceView> bookings, Dictionary<int, StatusView> status)
        {

            return (from b in bookings
                    select new Search.Booking
                    {
                        BookingNo = b.Id,
                        Customer = b.CustomerName.HtmlEncode(),
                        Account = b.Account,
                        Address = b.Address.HtmlEncode(),
                        StockBranch = b.StockBranch,
                        BookingBranch = b.DeliveryBranch,
                        StockBranchName = String.Format("{0} {1}", b.StockBranchName, b.StockBranch), //b.StockBranchName,
                        DeliveryBranchName = String.Format("{0} {1}", b.DeliveryBranchName, b.DeliveryBranch),
                        DeliveryZone = b.DeliveryZone,
                        DelCol = ScheduleOrPickup(b.DeliveryOrCollection, b.PickUp),
                        DueOn = b.DeliveryOrCollectionDate.ToSolrDate(),
                        ItemNo = b.ItemNo,
                        ItemId = b.Id,
                        ItemUPC = b.ItemUPC,
                        ItemDescription = b.ProductDescription.HtmlEncode(),
                        ProductCat = b.ProductCategory,
                        ItemsCount = b.Quantity.HasValue ? b.Quantity.Value : 0,
                        CreatedOn = b.OrderedOn,
                        Damaged = b.Damaged ? "Yes" : "No",
                        PickListNo = b.PickListNo,
                        ScheduleNo = b.ScheduleNo,
                        Truck = b.Truck,
                        OriginalId = b.OriginalId,
                        BookingStatus = BookingStatus.GetStatus(status[b.Id]),
                        Fascia = FasciaHelper.FromString(b.Fascia).Name,
                        PickUp = b.PickUp,
                        DeliveryOrCollectionSlot = b.DeliveryOrCollectionSlot   //#1406
                    }).ToList();
        }

        public BookingHistoryView History(int id)
        {
            using (var scope = Context.Read())
            {
                return (from h in scope.Context.BookingHistoryView
                        where h.Id == id
                        select h).FirstOrDefault();
            }
        }

        private string ScheduleOrPickup(string bookingType, bool isPickup)
        {
            if (bookingType == "D")
            {
                return isPickup
                    ? "Customer Pickup"
                    : "Scheduled Delivery";
            }
            if (bookingType == "C")
            {
                return isPickup
                    ? "Customer Return"
                    : "Scheduled Collection";
            }
            if (bookingType == "R")
            {
                return isPickup
                    ? "Customer RePickup"
                    : "Scheduled ReDelivery";
            }
            if (bookingType == "A")
            {
                return "Stock Allocation";
            }
            if (bookingType == "Q")
            {
                return "Stock Requisition";
            }
            if (bookingType == "T")
            {
                return "Stock Transfer";
            }

            throw new ArgumentException("Invalid booking type", "bookingType");
        }

        private bool IsDepositPaid(string acctNo)
        {
            var returnval = 0;
            var paid = 0;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

            SqlConnection sqlcon = new SqlConnection(connectionString);
            var sqlcmd = new SqlCommand();
            sqlcmd.Connection = sqlcon;
            sqlcon.Open();

            sqlcmd.CommandText = "SP_BS_GetDepositPaid";
            sqlcmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter sqlparam1 = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
            SqlParameter sqlparam2 = new SqlParameter("@is_paid", SqlDbType.Int);
            SqlParameter sqlparam3 = new SqlParameter("@return", SqlDbType.Int);
            sqlparam3.Direction = ParameterDirection.Output;
            sqlparam1.Value = acctNo;
            sqlparam2.Value = paid;
            sqlparam3.Value = returnval;
            sqlcmd.Parameters.Add(sqlparam1);
            sqlcmd.Parameters.Add(sqlparam2);
            sqlcmd.Parameters.Add(sqlparam3);
            int result = sqlcmd.ExecuteNonQuery();
            paid = Convert.ToInt32(sqlparam2.Value);
            returnval = Convert.ToInt32(sqlparam3.Value);
            sqlcon.Close();
            if (returnval != 1)
            {

                return false;
            }

            return true;
        }

        public void AddBooking(BookingSubmit message)
        {
            using (var scope = Context.Write())
            {

                CancelDuplicateRequisitions(message);

                var existing = scope.Context.Booking.Find(message.Id);
                if (existing != null)
                {
                    if (!(existing.AcctNo == message.Reference
                       && existing.ItemId == message.ItemId
                       && existing.ItemNo == message.SKU))
                        throw new Exception("Message has same shipment Id as another existing one but key fields differ.");

                    scope.Complete();
                    return;
                }
                if (message.Reference != null)
                {
                    if (!IsDepositPaid(message.Reference))
                    {
                        throw new Exception("Deposit Amount not paid for this Account number");
                        scope.Complete();
                        return;
                    }
                }


                // Ordered date should be local, so convert internal booking UTC dates
                if (DeliveryOrCollectionType.IsInternal(message.Type))
                {
                    message.RequestedDate = message.RequestedDate.ToLocalTime();
                }
                else
                {
                    message.OrderedOn = message.OrderedOn.ToUniversalTime();
                }

                scope.Context.Booking.Add(new Booking
                {
                    AcctNo = message.Reference,
                    AddressLine1 = message.AddressLine1,
                    AddressLine2 = message.AddressLine2,
                    AddressLine3 = message.AddressLine3,
                    AssemblyReq = message.AssemblyReq,
                    Comment = message.Comment,
                    ContactInfo = message.ContactInfo,
                    CustomerName = message.Recipient,
                    Damaged = message.Damaged,
                    DeliveryBranch = message.DeliveryBranch,
                    DeliveryZone = message.DeliveryZone,
                    DeliveryOrCollection = message.Type,
                    DeliveryOrCollectionDate = message.RequestedDate.Date,
                    Express = message.Express,
                    Id = message.Id,
                    ItemId = message.ItemId,
                    ItemUPC = message.ItemUPC,
                    ItemNo = message.SKU,
                    Quantity = message.Quantity,
                    OrderedOn = message.OrderedOn,
                    Path = String.Format("{0}.", message.Id),
                    PostCode = message.PostCode,
                    ProductArea = message.ProductArea,
                    ProductBrand = message.ProductBrand,
                    ProductCategory = message.ProductCategory,
                    ProductDescription = message.ProductDescription,
                    ProductModel = message.ProductModel,
                    RepoItemId = message.RepoItemId,
                    StockBranch = message.StockBranch,
                    UnitPrice = message.UnitPrice,
                    BookedBy = message.CreatedBy,
                    AddressNotes = message.AddressNotes,
                    Fascia = message.Fascia,
                    PickUp = message.PickUp,
                    DeliveryOrCollectionSlot = message.RequestedDate.ToString("HH") == "12" ? DeliveryTimeSlot.PM : DeliveryTimeSlot.AM,   //#14601
                    SalesBranch = message.SalesBranch,         //#19331
                    NonStockServiceType = message.NonStockServiceType,
                    NonStockServiceItemNo = message.NonStockServiceItemNo,
                    NonStockServiceDescription = message.NonStockServiceDescription,
                    ReceivingLocation = message.ReceivingLocation
                });

                scope.Context.SaveChanges();
                Search.SolrIndex.Booking(message.Id);
                scope.Complete();
            }
        }

        /// <returns>ID of the original requisition, defaults to null</returns>
        private int? CancelDuplicateRequisitions(BookingSubmit message)
        {
            if (message.Type != DeliveryOrCollectionType.Requisition)
            {
                return null;
            }

            using (var scope = Context.Read())
            {
                // when a requisition is booked for a SKU that is already
                // on an existing booking with a status of 'booked' for 
                // that location, the original booking is cancelled and
                // a new one is created with the same creation date as the
                // original

                var bookedRequisitionId = FindBookedRequisition(message);

                if (bookedRequisitionId != null)
                {
                    CancelBookings(bookedRequisitionId.Value, 99999, "Requisition superceded by new booking", DateTime.UtcNow);

                    var requisition = scope.Context.Booking.Single(b => b.Id == bookedRequisitionId);

                    new Mhub().Cancel(new BookingMessage
                    {
                        BookingId = requisition.BookingId,
                        Quantity = requisition.Quantity,
                        Type = requisition.DeliveryOrCollection,
                        AverageWeightedCost = requisition.UnitPrice
                    });

                    message.RequestedDate = requisition.DeliveryOrCollectionDate;
                }

                return bookedRequisitionId;
            }
        }

        private static int? FindBookedRequisition(BookingSubmit message)
        {
            using (var scope = Context.Read())
            {
                var requsitionIds = scope.Context.Booking.AsNoTracking()
                        .Where(b => b.DeliveryOrCollection == DeliveryOrCollectionType.Requisition
                                 && b.ItemNo == message.SKU
                                 && b.ReceivingLocation == message.ReceivingLocation
                                 && b.StockBranch == message.StockBranch)
                        .Select(b => b.Id);

                var requisitionStatusViews = scope.Context.StatusView.AsNoTracking()
                    .Where(x => requsitionIds.Contains(x.Id)).ToList();

                var requisitionStatuses = requisitionStatusViews
                    .Select(x => new { x.Id, Status = BookingStatus.GetStatus(x) });

                var bookedRequisitionIds = requisitionStatuses
                    .Where(x => x.Status == BookingStatus.Status.Booked)
                    .Select(x => x.Id)
                    .ToList();

                if (bookedRequisitionIds.Count > 1)
                {
                    throw new Exception(string.Format("Invalid data - multiple requisitions are booked for the SKU #{0} for the same location. Booking IDs: {1}", message.SKU, string.Join(", ", bookedRequisitionIds)));
                }

                var id = bookedRequisitionIds.SingleOrDefault();
                return id > 0 ? (int?)id : null;
            }
        }

        public void CancelBooking(BookingCancel message)
        {
            const string CancelComment = "This item has been cancelled from CoSACS.";

            var pathStart = String.Format("{0}.", message.Id);
            var pathMatch = String.Format(".{0}.", message.Id);

            using (var scope = Context.Write())
            {
                var booking = (from bookings in scope.Context.Booking
                               where (bookings.Path.StartsWith(pathStart) || bookings.Path.Contains(pathMatch))
                               && bookings.CurrentQuantity > 0          //#14440
                               select bookings).ToList();

                if (booking.Count > 0)        // #14494 booking found
                {
                    booking.ForEach(b =>
                    {
                        b.Comment = CancelComment;

                        if (!b.DeliverQuantity.HasValue) // Not delivered
                        {
                            b.Comment = CancelComment;

                            if (!b.ScheduleRejected.HasValue && !b.PickingRejected.HasValue)  // Not picked.
                            {
                                b.PickQuantity = 0;
                                b.PickingComment = CancelComment;
                                b.PickingRejected = true;
                            }
                            else // Picked
                            {
                                b.ScheduleQuantity = 0;
                                b.ScheduleComment = CancelComment;
                                b.ScheduleRejected = true;
                            }

                            //#10721
                            if (b.Exception == true)
                            {
                                b.Exception = false;
                            }

                            var cancel = (from cancellation in scope.Context.Cancellation       // #12379 do not add to Cancellation if already exists  
                                          where cancellation.Id == b.Id
                                          select cancellation).ToList();
                            if (cancel.Count == 0)
                            {
                                scope.Context.Cancellation.Add(new Cancellation
                                {

                                    Date = message.DateTime.ToUniversalTime(),     // #13469
                                    Id = b.Id,
                                    Reason = CancelComment,
                                    UserId = message.User
                                });
                            }

                        }
                    });
                }
                else
                {
                    throw new Exception("Booking not found");    // #14494 
                }

                scope.Context.SaveChanges();

                Search.SolrIndex.Booking(message.Id);

                scope.Complete();
            };
        }
    }
}
