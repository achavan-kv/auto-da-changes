using Blue.Cosacs.Service.Models;
using Blue.Cosacs.Service.Util;
using Blue.Cosacs.Web.Areas.Service.Models;
using Blue.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Linq;


namespace Blue.Cosacs.Service.Repositories
{
    public class TechnicianRepository
    {

        public TechnicianRepository(IEventStore audit, IClock clock)
        {
            this.clock = clock;
            this.audit = audit;
        }
        private readonly IClock clock;
        private readonly IEventStore audit;


        public TechnicianProfile Create(int id)
        {
            using (var scope = Context.Write())
            {
                var record = scope.Context.Technician.Find(id);
                if (scope.Context.Technician.Find(id) == null)
                {
                    record = new Technician()
                    {
                        Internal = false,
                        UserId = id,
                        StartTime = "0800",
                        EndTime = "1700",
                        Slots = 8,
                        MaxJobs = 10 //Code Added by Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.
                    };
                    scope.Context.Technician.Add(record);
                    scope.Context.SaveChanges();
                    scope.Complete();
                }
                return Get(id);
            }
        }

        public TechnicianProfile Get(int id)
        {
            using (var scope = Context.Read())
            {
                var tech = (from t in scope.Context.Technician
                            where t.UserId == id
                            select new TechnicianProfile()
                            {
                                Internal = t.Internal,
                                UserId = t.UserId,
                                startHour = t.StartTime.Substring(0, 2),
                                startMinute = t.StartTime.Substring(2, 2),
                                endHour = t.EndTime.Substring(0, 2),
                                endMinute = t.EndTime.Substring(2, 2),
                                slots = t.Slots,
                                maxJobs = t.MaxJobs, //Code Added by Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.
                            }).FirstOrDefault();

                if (tech != null)
                {
                    tech.Zones = GetZones(id);
                    return tech;
                }

                return null;
            }
        }

        public TechnicianBooking GetBooking(int requestId)
        {
            using (var scope = Context.Read())
            {
                return (from t in scope.Context.TechnicianBooking
                        where t.RequestId == requestId && !t.Reject
                        select t).OrderByDescending(b => b.Id).FirstOrDefault();
            }
        }

        private string[] GetZones(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ZoneUser.Where(z => z.UserId == id).Select(z => z.Zone).ToArray();
            }
        }

        public void Save(TechnicianProfile t)
        {
            using (var scope = Context.Write())
            {
                var tech = scope.Context.Technician.Find(t.UserId);
                tech.Internal = t.Internal;
                tech.StartTime = string.Format("{0}{1}", Pad(t.startHour), Pad(t.startMinute));
                tech.EndTime = string.Format("{0}{1}", Pad(t.endHour), Pad(t.endMinute));
                tech.Slots = t.slots;
                tech.MaxJobs = t.maxJobs; //Code Added by Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.

                scope.Context.ZoneUser.Where(z => z.UserId == t.UserId).ToList().ForEach(z =>
                {
                    scope.Context.ZoneUser.Remove(z);
                });

                if (t.Zones != null)
                {
                    foreach (var z in t.Zones)
                    {
                        scope.Context.ZoneUser.Add(new ZoneUser
                        {
                            UserId = t.UserId,
                            Zone = z
                        });
                    }
                }

                scope.Context.SaveChanges();
                scope.Complete();

            }
        }

        private string Pad(string val)
        {
            if (val.Length == 1)
            {
                return "0" + val;
            }
            else
            {
                return val;
            }
        }

        public TechnicianDiary GetDiary(int TechnicianId, DateTime Start, DateTime End)
        {
            using (var scope = Context.Read())
            {
                if (!scope.Context.AdminTechProfileUsersView.Any(x => x.UserId == TechnicianId))
                {
                    return new TechnicianDiary() { Found = false };
                }

                var tech = scope.Context.Technician.Find(TechnicianId);

                var techDiary = new TechnicianDiary()
                {
                    Holidays = (from h in scope.Context.Holiday
                                where h.UserId == TechnicianId && (h.EndDate >= Start && h.EndDate <= End || h.StartDate >= Start && h.StartDate <= End || h.StartDate <= Start && h.EndDate >= End)
                                select new HolidayList()
                                {
                                    StartDate = h.StartDate,
                                    EndDate = h.EndDate,
                                    Id = h.Id,
                                    Approved = h.Approved
                                }).ToList(),
                    PublicHolidays = (from p in scope.Context.PublicHolidayView
                                      where ((p.Date >= Start && p.Date <= End) || (Start <= p.Date && End >= p.Date))
                                      select new TechnicianDiary.PublicHoliday()
                                      {
                                          Date = p.Date,
                                      }).ToList(),
                };

                techDiary.Bookings = (from t in scope.Context.TechnicianAllocatedRequestView
                                      where t.UserId == TechnicianId && (t.Date >= Start && t.Date <= End || t.Reject)
                                      select new TechnicianDiary.TechnicianBooking()
                                      {
                                          Id = t.Id,
                                          RequestId = t.RequestId,
                                          CreatedOn = t.CreatedOn,
                                          CustomerAddressLine1 = t.CustomerAddressLine1,
                                          CustomerAddressLine2 = t.CustomerAddressLine2,
                                          CustomerAddressLine3 = t.CustomerAddressLine3,
                                          CustomerFirstName = t.CustomerFirstName,
                                          CustomerLastName = t.CustomerLastName,
                                          CustomerPostcode = t.CustomerPostcode,
                                          CustomerTitle = t.CustomerTitle,
                                          InvoiceNumber = t.InvoiceNumber,
                                          Item = t.Item,
                                          ItemSupplier = t.ItemSupplier,
                                          Type = t.Type.Trim(),
                                          Date = t.Date,
                                          Slot = t.Slot,
                                          SlotExtend = t.SlotExtend,
                                          Reject = t.Reject

                                      }).ToArray();

                techDiary.FreeBookings = new List<TechnicianDiary.TechnicianBooking>();

                //techDiary.FreeBookings = from t in scope.Context.TechnicianAvailableRequestView
                //                         select new TechnicianDiary.TechnicianBooking()
                //                         {
                //                             RequestId = t.RequestId,
                //                             CreatedOn = t.CreatedOn,
                //                             CustomerAddressLine1 = t.CustomerAddressLine1,
                //                             CustomerAddressLine2 = t.CustomerAddressLine2,
                //                             CustomerAddressLine3 = t.CustomerAddressLine3,
                //                             CustomerFirstName = t.CustomerFirstName,
                //                             CustomerLastName = t.CustomerLastName,
                //                             CustomerPostcode = t.CustomerPostcode,
                //                             CustomerTitle = t.CustomerTitle,
                //                             InvoiceNumber = t.InvoiceNumber,
                //                             Item = t.Item,
                //                             ItemSupplier = t.ItemSupplier,
                //                             Type = t.Type.Trim(),
                //                         };

                techDiary.PendingHolidays = (from h in scope.Context.Holiday
                                             where h.UserId == TechnicianId && h.Approved == false
                                             select new HolidayList()
                                             {
                                                 StartDate = h.StartDate,
                                                 EndDate = h.EndDate,
                                                 Id = h.Id,
                                                 Approved = h.Approved
                                             }).ToList();

                techDiary.Technician = new List<Technician>() { tech };
                techDiary.Found = true;
                return techDiary;
            }
        }

        public IEnumerable<TechnicianDiary.TechnicianBooking> GetFreeBookings(string query)
        {
            List<TechnicianDiary.TechnicianBooking> freeBookings;
            var isEmpty = string.IsNullOrEmpty(query);

            using (var scope = Context.Read())
            {
                var lst = (from t in scope.Context.TechnicianAvailableRequestView
                           where isEmpty || SqlFunctions.StringConvert((double)t.RequestId).TrimStart().StartsWith(query)
                           select new TechnicianDiary.TechnicianBooking()
                           {
                               RequestId = t.RequestId,
                               CreatedOn = t.CreatedOn,
                               CustomerAddressLine1 = t.CustomerAddressLine1,
                               CustomerAddressLine2 = t.CustomerAddressLine2,
                               CustomerAddressLine3 = t.CustomerAddressLine3,
                               CustomerFirstName = t.CustomerFirstName,
                               CustomerLastName = t.CustomerLastName,
                               CustomerPostcode = t.CustomerPostcode,
                               CustomerTitle = t.CustomerTitle,
                               InvoiceNumber = t.InvoiceNumber,
                               Item = t.Item,
                               ItemSupplier = t.ItemSupplier,
                               Type = t.Type.Trim(),
                           });


                freeBookings = lst.ToList();
            }

            return freeBookings;
        }

        public TechnicianDiary.TechnicianBooking GetFreeBooking(int requestId)
        {
            TechnicianDiary.TechnicianBooking freeBookings;

            using (var scope = Context.Read())
            {
                freeBookings = (from t in scope.Context.TechnicianAvailableRequestView
                                where t.RequestId == requestId
                                select new TechnicianDiary.TechnicianBooking()
                                {
                                    RequestId = t.RequestId,
                                    CreatedOn = t.CreatedOn,
                                    CustomerAddressLine1 = t.CustomerAddressLine1,
                                    CustomerAddressLine2 = t.CustomerAddressLine2,
                                    CustomerAddressLine3 = t.CustomerAddressLine3,
                                    CustomerFirstName = t.CustomerFirstName,
                                    CustomerLastName = t.CustomerLastName,
                                    CustomerPostcode = t.CustomerPostcode,
                                    CustomerTitle = t.CustomerTitle,
                                    InvoiceNumber = t.InvoiceNumber,
                                    Item = t.Item,
                                    ItemSupplier = t.ItemSupplier,
                                    Type = t.Type.Trim(),
                                }).SingleOrDefault();
            }

            return freeBookings;
        }

        public int AddHoliday(int id, DateTime start, DateTime end, bool portalUser)
        {
            int holId;

            using (var scope = Context.Write())
            {
                if (DateTime.Compare(start, end) > 0)
                {
                    throw new Exception("Error in Add holiday. End date before start.");
                }

                var holiday = new Holiday()
                {
                    UserId = id,
                    StartDate = start,
                    EndDate = end,
                    Approved = !portalUser
                };

                scope.Context.Holiday.Add(holiday);
                scope.Context.SaveChanges();
                holId = holiday.Id;
                audit.LogAsync(new { TechUnavail = holiday }, EventType.AddAvailability, EventCategory.TechnicianDiary);      // #12556
                scope.Complete();
            }
            return holId;
        }

        public void ApproveHoliday(int id)
        {
            using (var scope = Context.Write())
            {
                var holiday = scope.Context.Holiday.Find(id).Approved = true;
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void DeleteHoliday(int id)
        {
            using (var scope = Context.Write())
            {
                audit.LogAsync(new { TechUnavail = scope.Context.Holiday.Find(id) }, EventType.DeleteAvailability, EventCategory.TechnicianDiary);      // #12555
                scope.Context.Holiday.Remove(scope.Context.Holiday.Find(id));
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public void DeleteBookingById(int id, int userId, string reason, bool log = true)
        {
            using (var scope = Context.Write())
            {
                var booking = scope.Context.TechnicianBooking.Find(id);
                scope.Context.TechnicianBooking.Remove(booking);
                scope.Context.SaveChanges();
                if (log)
                {
                    LogDeleteBooking(booking, userId, string.IsNullOrWhiteSpace(reason) ? "Removed" : reason);
                }

                scope.Complete();
            }
        }

        public void DeleteAllBookings(int requestId, int userId)
        {
            using (var scope = Context.Write())
            {
                var bookings = scope.Context.TechnicianBooking.Where(t => t.RequestId == requestId).ToList();
                bookings.ForEach(b =>
                {
                    scope.Context.TechnicianBooking.Remove(b);
                    LogDeleteBooking(b, userId, "Allocation Type Changed");
                });
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }


        public void DeleteBookingByRequest(int requestId, int userId, string reason, bool log = true)
        {
            using (var scope = Context.Write())
            {
                var lastBooking = scope.Context.TechnicianBooking.Where(b => b.RequestId == requestId).OrderByDescending(b => b.Id).FirstOrDefault();
                if (lastBooking != null)
                {
                    DeleteBookingById(lastBooking.Id, userId, reason, log);
                }
            }
        }

        private void LogDeleteBooking(TechnicianBooking booking, int userId, string reason)
        {
            using (var scope = Context.Write())
            {
                audit.LogAsync(new { TechBooking = booking }, EventType.DeleteBooking, EventCategory.TechnicianDiary, new { User = userId, RequestId = booking.RequestId });
                scope.Context.TechnicianBookingDelete.Add(new TechnicianBookingDelete
                {
                    Date = clock.Now,
                    Id = booking.Id,
                    Reason = reason,
                    RequestId = booking.RequestId,
                    TechincianId = booking.UserId,
                    UserId = userId
                });
                scope.Context.SaveChanges();
                Solr.SolrIndex.IndexRequest(new[] { booking.RequestId });
                scope.Complete();

            }
        }

        public int NewBooking(int userId, int requestId, DateTime date, int slot, int slotExtend)
        {
            int newId;

            using (var scope = Context.Write())
            {
                DeleteBookingByRequest(requestId, userId, null, log: false);

                var booking = new TechnicianBooking()
                {
                    Date = date,
                    RequestId = requestId,
                    Slot = slot,
                    UserId = userId,
                    SlotExtend = slotExtend,
                    AllocatedOn = clock.Now,
                    Reject = false
                };

                newId = SaveBooking(booking);
                scope.Context.SaveChanges();
                audit.LogAsync(new { TechBooking = booking }, EventType.AssignBooking, EventCategory.TechnicianDiary);
                newId = booking.Id;
                scope.Complete();
            }
            Solr.SolrIndex.IndexRequest(new[] { requestId });
            return newId;
        }

        public IEnumerable<TechnicianName> GetTechnician()
        {
            using (var scope = Context.Read())
            {
                return (from t in scope.Context.TechnicianNameView
                        join a in scope.Context.AdminTechProfileUsersView on t.UserId equals a.UserId
                        select new TechnicianName
                        {
                            Name = t.FullName,
                            Id = t.UserId
                        }).ToList();
            }
        }

        public TechnicianFree FreeAllocation(FreeTechnicianSearch search)
        {
            var now = new DateTime(clock.Now.Year, clock.Now.Month, clock.Now.Day);
            var EndPeriod = now.AddDays(7);


            using (var scope = Context.Read())
            {
                var holidays = scope.Context.Holiday.Select(p => p);
                var bookings = scope.Context.TechnicianBooking.Select(t => t);
                var technician = scope.Context.TechnicianUser.Select(p => p);
                var publicHolidays = scope.Context.PublicHolidayView.Select(p => p);


                if (search.bookingDate.HasValue && !search.technicianId.HasValue)
                {
                    var bDate = search.bookingDate.Value;
                    technician = from t in technician
                                 where !holidays.Any(h => h.UserId == t.UserId && h.StartDate <= bDate && h.EndDate >= bDate) &&
                                       !publicHolidays.Any(p => p.Date == bDate)
                                 select t;
                    bookings = bookings.Where(b => b.Date == bDate);
                }

                if (search.technicianId.HasValue && !search.bookingDate.HasValue) // Get whole week for tech for all lists.
                {
                    holidays = holidays.Where(h => ((h.StartDate >= now && h.StartDate <= EndPeriod ||
                                          h.EndDate <= EndPeriod && h.EndDate >= now) ||
                                          h.StartDate <= now && h.EndDate >= EndPeriod) &&
                                         h.UserId == search.technicianId.Value);
                    bookings = bookings.Where(b => b.Date >= now && b.Date <= EndPeriod)
                                        .Where(b => b.UserId == search.technicianId.Value);
                    publicHolidays = publicHolidays.Where(b => b.Date >= now && b.Date <= EndPeriod);
                }

                if (search.bookingDate.HasValue && search.technicianId.HasValue)
                {
                    var bDate = search.bookingDate.Value;
                    technician = from t in technician
                                 where !holidays.Any(h => h.UserId == t.UserId && h.StartDate <= bDate && h.EndDate >= bDate) &&
                                       !publicHolidays.Any(p => p.Date == bDate)
                                 select t;
                    bookings = bookings.Where(b => b.Date == bDate).Where(b => b.UserId == search.technicianId.Value);
                    holidays = holidays.Where(h => h.UserId == search.technicianId.Value && h.StartDate <= bDate && h.EndDate >= bDate);
                    publicHolidays = publicHolidays.Where(b => b.Date >= now && b.Date <= EndPeriod);
                }

                if (!string.IsNullOrWhiteSpace(search.category))
                {
                    var zoneUsers = scope.Context.ZoneUser.Where(z => z.Zone == search.category).Select(z => z.UserId).ToList();
                    technician = from t in technician
                                 where zoneUsers.Contains(t.UserId)
                                 select t;
                    bookings = from b in bookings
                               where technician.Select(t => t.UserId).Contains(b.UserId)
                               select b;
                }

                var freeBookings = (from b in bookings
                                    join r in scope.Context.Request on b.RequestId equals r.Id
                                    select new TechnicianFree.TechnicianBook()
                                    {
                                        Date = b.Date,
                                        Id = b.Id,
                                        UserId = b.UserId,
                                        RequestId = b.RequestId,
                                        Slot = b.Slot,
                                        SlotExtend = b.SlotExtend,
                                        Type = r.Type.Trim(),
                                        Reject = b.Reject
                                    });

                var freeHolidays = (from h in holidays
                                    select new TechnicianFree.Hol()
                                    {
                                        Id = h.Id,
                                        UserId = h.UserId,
                                        EndDate = h.EndDate,
                                        StartDate = h.StartDate
                                    });

                var free = new TechnicianFree()
                {
                    Bookings = (search.technicianId.HasValue || search.bookingDate.HasValue) ? freeBookings.ToList() : new List<TechnicianFree.TechnicianBook>(), // Get bookings if either date or tech provided.
                    Holidays = search.technicianId.HasValue ? freeHolidays.ToList() : new List<TechnicianFree.Hol>(),             // Get hols if techid provided. (date is filtered above)
                    PublicHolidays = (from p in publicHolidays
                                      select new TechnicianFree.PHolidays()
                                      {
                                          Date = p.Date
                                      }).ToList(),
                    Technicians = (from t in technician
                                   select new TechnicianFree.Tech()
                                   {
                                       UserId = t.UserId,
                                       EndTime = t.EndTime,
                                       Internal = t.Internal,
                                       Slots = t.Slots,
                                       StartTime = t.StartTime,
                                       Name = t.FullName
                                   }).ToList()
                };
                return free;
            };
        }

        public void CompleteAllocation(int requestId)
        {
            using (var scope = Context.Write())
            {
                var booking = scope.Context.TechnicianBooking.Where(t => t.RequestId == requestId && t.CompletedDate == null).FirstOrDefault();
                if (booking != null)
                {
                    booking.CompletedDate = clock.Now;
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public bool CheckSave(RequestItem request, LastUpdated updated)
        {
            using (var scope = Context.Write())
            {
                var lastBooking = scope.Context.TechnicianBooking.Where(b => b.RequestId == request.Id && b.CompletedDate == null).ToList();

                if (lastBooking.Any())
                {
                    lastBooking.ForEach(b =>
                    {
                        DeleteBookingById(b.Id, updated.LastUpdatedUser.Value, request.TechnicianBookingDeleteReason, log: true);
                    });
                }
                return SaveBooking(request);
            }
        }

        private bool SaveBooking(RequestItem r)
        {
            if (r.AllocationServiceScheduledOn.HasValue && r.AllocationTechnician.HasValue)
            {
                var tech = new TechnicianBooking()
                {
                    Date = r.AllocationServiceScheduledOn.Value,
                    UserId = r.AllocationTechnician.Value,
                    RequestId = r.Id,
                    Slot = r.AllocationSlots,
                    SlotExtend = r.AllocationSlotExtend,
                    AllocatedOn = r.AllocationOn.HasValue ? r.AllocationOn.Value : clock.Now,
                    Reject = false
                };
                return SaveBooking(tech) == 0;
            }
            else
            {
                return false;
            }
        }


        public int SaveBooking(TechnicianBooking booking)
        {
            var id = 0;
            using (var scope = Context.Write())
            {
                var bookings = from t in scope.Context.TechnicianBooking
                               where (t.UserId == booking.UserId &&
                                        booking.Date == t.Date &&
                                        ((booking.Slot >= t.Slot &&
                                        booking.Slot <= t.Slot + t.SlotExtend &&
                                        booking.Slot + booking.SlotExtend >= t.Slot &&
                                        booking.Slot + booking.SlotExtend <= t.Slot + t.SlotExtend) ||
                                        (booking.Slot <= t.Slot && booking.Slot + booking.SlotExtend >= t.Slot + t.SlotExtend) ||
                                        (t.Slot <= booking.Slot && t.Slot + t.SlotExtend >= booking.Slot + booking.SlotExtend)))
                               select t;

                var holidays = from h in scope.Context.Holiday
                               where ((booking.Date >= h.StartDate &&
                                      booking.Date <= h.EndDate) ||
                                      (h.StartDate <= booking.Date && h.EndDate >= booking.Date)) && h.UserId == booking.UserId
                               select h;

                if (holidays.Any() || bookings.Any())
                {
                    return 0;
                }

                scope.Context.TechnicianBooking.Add(booking);
                scope.Context.SaveChanges();
                id = booking.Id;
                scope.Complete();
            }
            return id;
        }

        public DiaryExceptions GetExceptions()
        {
            using (var scope = Context.Read())
            {
                return new DiaryExceptions()
                {
                    PendingHolidays = (from h in scope.Context.PendingHolidaysNameView
                                       where h.Approved == false
                                       select new HolidayList()
                                       {
                                           StartDate = h.StartDate,
                                           EndDate = h.EndDate,
                                           Id = h.Id,
                                           Approved = h.Approved,
                                           UserId = h.UserId,
                                           FullName = h.FullName
                                       }).ToList(),
                    RejectBookings = (from b in scope.Context.GetRejectedJobsView
                                      where b.Reject == true
                                      select new DiaryExceptions.Rejections
                                      {
                                          Id = b.Id,
                                          DateAllocated = b.Date,
                                          RequestId = b.RequestId,
                                          Date = b.AllocatedOn,
                                          CreatedOn = b.CreatedOn,
                                          FullName = b.FullName,
                                          LastUpdatedOn = b.LastUpdatedOn,
                                          Type = b.Type.Trim(),
                                          UserId = b.UserId
                                      }).ToList()
                };
            }
        }

        public List<TechnicianPaymentsView> GetTechnicianPayments(TechnicianPaymentSearch search)
        {
            if (search.DateTo.HasValue)
            {
                search.DateTo = search.DateTo.Value.AddDays(1);
            }

            using (var scope = Context.Read())
            {
                var q = from t in scope.Context.TechnicianPaymentsView
                        where
                            t.TechnicianId == search.TechnicianId
                            && t.Total > 0
                        select t;

                //null dates filter
                if (!search.DateFrom.HasValue && !search.DateTo.HasValue && string.IsNullOrWhiteSpace(search.ServiceRequest))
                {
                    var date = clock.Now.AddDays(-30); // Default 1 month
                    q = q.Where(p => p.AllocatedOn > date);
                }
                //only date from dates filter
                else if (search.DateFrom.HasValue && !search.DateTo.HasValue)
                {
                    q = q.Where(p => p.AllocatedOn >= search.DateFrom.Value);
                }
                //only date to dates filter
                else if (!search.DateFrom.HasValue && search.DateTo.HasValue)
                {
                    q = q.Where(p => p.AllocatedOn < search.DateTo.Value);
                }
                //dates between
                else if (search.DateFrom.HasValue && search.DateTo.HasValue)
                {
                    q = q.Where(p => p.AllocatedOn >= search.DateFrom.Value && p.AllocatedOn < search.DateTo.Value);
                }

                if (search.TypeFilter != "All")
                {
                    if (string.IsNullOrWhiteSpace(search.TypeFilter))
                    {
                        q = q.Where(f => f.State == null);
                    }
                    else
                    {
                        q = q.Where(f => f.State == search.TypeFilter);
                    }
                }

                int tmpServiceRequestId = -1;
                if (!string.IsNullOrWhiteSpace(search.ServiceRequest) &&
                    int.TryParse(search.ServiceRequest, out tmpServiceRequestId))
                {
                    q = q.Where(p => p.RequestId == tmpServiceRequestId);
                }

                return q.ToList();
            }
        }

        public List<TechnicianPaymentsView> GetTechnicianPayments(IEnumerable<int> ids)
        {
            using (var scope = Context.Read())
            {
                return (from t in scope.Context.TechnicianPaymentsView
                        where ids.Contains(t.RequestId)
                        select t).ToList();
            }
        }

        public List<TechnicianPaymentsView> SearchPaymentsById(int id, string search)
        {

            using (var scope = Context.Read())
            {
                return (from t in scope.Context.TechnicianPaymentsView
                        where t.TechnicianId == id && t.Total > 0 && SqlFunctions.StringConvert((double)t.RequestId).Contains(search)
                        select t).ToList();
            }
        }

        public List<TechnicianPaymentsTotCostView> GetTechniciansWithPayments()
        {
            using (var scope = Context.Read())
            {
                return (from t in scope.Context.TechnicianPaymentsTotCostView
                        where t.Total > 0
                        select t).OrderBy(t => t.FullName).ToList();
            }
        }

        public void RejectBooking(int id, string reason, int userId)
        {
            using (var scope = Context.Write())
            {
                var booking = scope.Context.TechnicianBooking.Find(id);
                if (booking == null)
                {
                    throw new Exception("Booking Not Found");
                }

                scope.Context.TechnicianBookingReject.Add(new TechnicianBookingReject
                {
                    Date = clock.Now,
                    Id = booking.Id,
                    Reason = reason,
                    RequestId = booking.RequestId,
                    TechincianId = booking.UserId,
                    UserId = userId
                });
                audit.LogAsync(new { TechBooking = booking }, EventType.RejectBooking, EventCategory.TechnicianDiary, new { User = userId, RequestId = booking.RequestId });
                booking.Reject = true;
                scope.Context.SaveChanges();
                Solr.SolrIndex.IndexRequest(new[] { booking.RequestId });
                scope.Complete();
            }

        }

        public bool RemoveTechnicianPayment(int id)
        {
            using (var scope = Context.Write())
            {
                var request = scope.Context.Request.Find(id);
                if (request != null && request.TechnicianPayState == null)
                {
                    request.TechnicianPayState = TechnicianPayStates.Deleted;
                    scope.Context.SaveChanges();
                    scope.Complete();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool HoldTechnicianPayment(int id, bool hold)
        {
            using (var scope = Context.Write())
            {
                var request = scope.Context.Request.Find(id);
                if (request != null && (request.TechnicianPayState == null && hold ||
                                        request.TechnicianPayState == TechnicianPayStates.OnHold && !hold))
                {
                    request.TechnicianPayState = hold ? TechnicianPayStates.OnHold : null;
                    scope.Context.SaveChanges();
                    scope.Complete();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool HoldAllTechnicianPayment(int[] ids, bool hold)
        {
            var saved = true;
            using (var scope = Context.Write())
            {
                var requests = scope.Context.Request.Where(r => ids.Contains(r.Id)).ToList();
                requests.ForEach(r =>
                {
                    if (r.TechnicianPayState == null || r.TechnicianPayState == TechnicianPayStates.OnHold)
                    {
                        r.TechnicianPayState = hold ? TechnicianPayStates.OnHold : null;
                    }
                    else
                    {
                        saved = false;
                    }
                });
                if (saved)
                {
                    scope.Context.SaveChanges();
                    scope.Complete();
                }
            }
            return saved;
        }

        public bool PayTechnicianPayment(int[] ids)
        {
            var saved = true;
            using (var scope = Context.Write())
            {
                var requests = scope.Context.Request.Where(r => ids.Contains(r.Id)).ToList();
                requests.ForEach(r =>
                {
                    if (r.TechnicianPayState == null)
                    {
                        r.TechnicianPayState = TechnicianPayStates.Paid;
                    }
                    else
                    {
                        saved = false;
                    }
                });
                if (saved)
                {
                    scope.Context.SaveChanges();
                    scope.Complete();
                }
                return saved;
            };
        }

        //Code Added by Gurpreet - CR2018-010 - 31/10/18 - Setting of max no. of Jobs & Validation with allocated jobs for a technician.

        /// <summary>
        /// Gets the Maximum and Current number of jobs allocated to a technician.
        /// </summary>
        /// <param name="techid"></param>
        /// <returns></returns>
        public TechnicianJobs GetTechnicianMaxAndCurrentJobs(int techid)
        {
            using (var scope = Context.Read())
            {
                var tempcount = 0;
                var currJobs_count = GetTechnicianJobAllocation(techid);

                foreach (var item in currJobs_count)
                {
                    tempcount = tempcount + 1;
                }

                var result = (from t in scope.Context.Technician
                              where t.UserId == techid
                              select new TechnicianJobs()
                              {
                                  maxJobs = t.MaxJobs,
                                  currJobs = tempcount
                              }).FirstOrDefault();

                return result;
            }
        }


        /// <summary>
        /// Gets the number of jobs allocated to the technician for the user to override a jobs
        /// </summary>
        /// <param name="techId"></param>
        /// <returns></returns>
        public List<TechnicianJobAllocation> GetTechnicianJobAllocation(int techId)
        {
            var dt = new DataTable();
            Unicomer.CosacsService.ServiceTechnician st = new Unicomer.CosacsService.ServiceTechnician(audit);
            dt = st.GetAllocatedTechnicianJobs(techId);
            List<TechnicianJobAllocation> techList = new List<TechnicianJobAllocation>();

            if (dt != null)
            {
                techList = dt.Rows.OfType<DataRow>()
                     .Select(ta => new TechnicianJobAllocation()
                     {
                         ServiceType = Convert.ToString(ta["ServiceType"]),
                         AccountNumber = Convert.ToString(ta["AccountNumber"]),
                         RequestId = Convert.ToInt32(ta["ServiceRequestNo"]),
                         DateLogged = Convert.ToDateTime(ta["LoggedOn"]),
                         DeliveredOn = Convert.ToDateTime(ta["DeliveredOn"]),
                         WarrantyType = Convert.ToString(ta["WarrantyType"]),
                         ItemCodeDescription = Convert.ToString(ta["ItemCodeDescription"]),
                         SlotDate = Convert.ToDateTime(ta["ServiceScheduledDate"]),
                         CurrentJobState = Convert.ToString(ta["CurrentJobState"])
                     }).ToList();
            }

            return techList;
        }

        /// <summary>
        /// Overrides a selected job by the user and adds the current new job for the technician
        /// </summary>
        /// <param name="techId"></param>
        /// <param name="requestId"></param>
        public void OverrideBookingByRequestId(int techId, int requestId)
        {
            Unicomer.CosacsService.ServiceTechnician st = new Unicomer.CosacsService.ServiceTechnician(audit);
            st.OverrideBookingByRequestId(techId, requestId);
        }

        /// <summary>
        /// Audits the job override process carried by the user
        /// </summary>
        /// <param name="oldRequestId"></param>
        /// <param name="overrideByUserId"></param>
        /// <param name="overideDate"></param>
        /// <param name="newRequestId"></param>
        public void JobOverrideAudit(int oldRequestId, int overrideByUserId, DateTime overideDate, int newRequestId)
        {
            Unicomer.CosacsService.ServiceTechnician st = new Unicomer.CosacsService.ServiceTechnician(audit);
            st.JobOverrideAudit(oldRequestId, overrideByUserId, overideDate, newRequestId);
        }

        public List<TechAuthentiction> GetUserAuthForOverride(int id, string pwd)
        {
            var dt = new DataTable();
            string message = string.Empty;
            Unicomer.CosacsService.ServiceTechnician st = new Unicomer.CosacsService.ServiceTechnician(audit);
            dt = st.GetUserAuthForOverride(id, pwd);
            List<TechAuthentiction> techAuthList = new List<TechAuthentiction>();
            if (dt != null)
            {
                techAuthList = dt.Rows.OfType<DataRow>()
                     .Select(ta => new TechAuthentiction()
                     {
                         UserId = Convert.ToInt32(ta["UserId"]),
                         Password = Convert.ToString(ta["UserPassword"]),
                         IsLocked = Convert.ToBoolean(ta["Locked"]),
                         PwdChange = Convert.ToBoolean(ta["ChangePassword"]),
                         IsAuthorised = Convert.ToBoolean(ta["UserDeny"]),
                     }).ToList();
            }
            else
            { techAuthList = null; }
            return techAuthList;
        }

        //CR2018-010 Changes End
    }
}