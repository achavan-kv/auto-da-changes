using Blue.Cosacs.Service.Models;
using Blue.Cosacs.Service.Solr;
using Blue.Events;
using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Service.Repositories
{
    public class SalesRepository
    {
        private readonly IEventStore audit;
        private readonly IClock clock;

        public SalesRepository(IEventStore audit, IClock clock)
        {
            this.audit = audit;
            this.clock = clock;
        }

        public void CloseInternalInstallationRequest(Service.Messages.Order order, LastUpdated userDetails)
        {
            var items = new List<Service.Messages.Item>(order.Items);
            var returnedInstallations = items.Where(x => x.ItemTypeId == (int)ItemTypeEnum.Installation && x.Returned).ToList();
            List<int> ids = new List<int>();

            using (var scope = Context.Write())
            {
                foreach (var installation in returnedInstallations)
                {
                    for (int j = 0; j < installation.Quantity; j++)
                    {
                        var existingData = scope.Context.Request.Where(i => "II".Equals(i.Type) && i.InvoiceNumber == order.OriginalOrderId.ToString() &&
                                                                        i.ItemNumber.Equals(installation.ParentItemNo) && !"Installaed".Equals(i.State) &&
                                                                        !"Cancelled".Equals(i.State))
                                                                        .FirstOrDefault();

                        var parentItem = items.Where(x => x.ItemTypeId == (int)ItemTypeEnum.Product && x.Id == installation.ParentId).SingleOrDefault();

                        if (existingData != null)
                        {
                            if (!"New".Equals(existingData.State))
                            {
                                var technicianBooking = scope.Context.TechnicianBooking.Where(x => x.RequestId == existingData.Id).SingleOrDefault();
                                if (technicianBooking != null)
                                {
                                    scope.Context.TechnicianBooking.Remove(technicianBooking);
                                    var id = scope.Context.TechnicianBookingDelete.Any() ? scope.Context.TechnicianBookingDelete.Max(i => i.Id) : 0;

                                    var technicianDeleteEntry = new TechnicianBookingDelete
                                    {
                                        Id = id + 1,
                                        RequestId = technicianBooking.RequestId,
                                        UserId = order.CreatedBy,
                                        Date = order.CreatedOn,
                                        TechincianId = technicianBooking.UserId,
                                        Reason = installation.ReturnReason == null ? parentItem.ReturnReason : installation.ReturnReason
                                    };
                                    scope.Context.TechnicianBookingDelete.Add(technicianDeleteEntry);
                                }
                            }

                            existingData.State = "Cancelled";
                            existingData.LastUpdatedOn = order.CreatedOn;
                            existingData.LastUpdatedUser = order.CreatedBy;
                            existingData.LastUpdatedUserName = userDetails.LastUpdatedUserName;

                            scope.Context.SaveChanges();

                            audit.LogAsync(
                                new
                                {
                                    ServiceRequestId = existingData.Id,
                                    Reason = "POS installation returned"
                                },
                                EventType.UpdateRequest,
                                EventCategory.Service);

                            ids.Add(existingData.Id);
                        }
                    }
                }
                scope.Complete();
            }
            SolrIndex.IndexRequest(ids.ToArray());
        }

        public void CreateInternalInstallationRequest(List<int> ids, Service.Messages.Order order, LastUpdated userDetails)
        {
            var items = new List<Service.Messages.Item>(order.Items);
            var newInstallations = items.Where(x => x.ItemTypeId == (int)ItemTypeEnum.Installation && !x.Returned).ToList();
            var merchandisingRepo = new Stock.Repositories.ProductRepository();

            using (var scope = Context.Write())
            {
                var i = 0;
                foreach (var installation in newInstallations)
                {
                    var parentItem = items.Single(x => x.ItemTypeId == (int)ItemTypeEnum.Product && x.Id == installation.ParentId);

                    for (int j = 0; j < installation.Quantity; j++)
                    {
                        var newRequest = new Request
                        {
                            Id = ids.ElementAt(i),
                            CreatedOn = order.CreatedOn,
                            CreatedBy = userDetails.LastUpdatedUserName,
                            Branch = order.BranchNo,
                            Type = "II",
                            State = "New",
                            Account = order.CashAndGoAccountNo,
                            InvoiceNumber = order.Id.ToString(),
                            CustomerId = order.Customer.CustomerId,
                            CustomerTitle = order.Customer.Title,
                            CustomerFirstName = order.Customer.FirstName,
                            CustomerLastName = order.Customer.LastName,
                            CustomerAddressLine1 = order.Customer.AddressLine1,
                            CustomerAddressLine2 = order.Customer.AddressLine2,
                            CustomerAddressLine3 = order.Customer.AddressLine3,
                            //CustomerNotes = not required,
                            CustomerPostcode = order.Customer.PostCode,
                            ItemId = parentItem.ProductItemId.ToString(),
                            ItemAmount = parentItem.Price,
                            ItemSoldBy = order.CreatedBy.ToString(),
                            ItemDeliveredOn = order.CreatedOn,
                            ItemStockLocation = order.BranchNo,
                            Item = parentItem.Description,
                            ItemSupplier = parentItem.ItemSupplier,
                            //ItemSerialNumber = not required
                            CreatedById = order.CreatedBy,
                            LastUpdatedUser = userDetails.LastUpdatedUser,
                            LastUpdatedUserName = userDetails.LastUpdatedUserName,
                            LastUpdatedOn = order.CreatedOn,
                            ProductLevel_1 = parentItem.Department,
                            ProductLevel_2 = parentItem.Category,
                            ProductLevel_3 = parentItem.Class,
                            ItemNumber = parentItem.ItemNo,
                        };

                        scope.Context.Request.Add(newRequest);

                        if (!string.IsNullOrEmpty(order.Customer.HomePhone))
                        {
                            scope.Context.RequestContact.Add(new RequestContact
                            {
                                RequestId = newRequest.Id,
                                Value = order.Customer.HomePhone,
                                Type = "HomePhone"
                            });
                        }

                        if (!string.IsNullOrEmpty(order.Customer.MobilePhone))
                        {
                            scope.Context.RequestContact.Add(new RequestContact
                            {
                                RequestId = newRequest.Id,
                                Value = order.Customer.MobilePhone,
                                Type = "MobilePhone"
                            });
                        }

                        if (!string.IsNullOrEmpty(order.Customer.Email))
                        {
                            scope.Context.RequestContact.Add(new RequestContact
                            {
                                RequestId = newRequest.Id,
                                Value = order.Customer.Email,
                                Type = "Email"
                            });
                        }

                        scope.Context.SaveChanges();

                        i = i + 1;

                        audit.LogAsync(
                            new
                            {
                                ServiceRequestId = newRequest.Id,
                                Reason = "POS installation sold"
                            },
                            EventType.CreateRequest,
                            EventCategory.Service);
                    }
                    scope.Complete();
                }
            }
            SolrIndex.IndexRequest(ids.ToArray());
        }

        public enum ItemTypeEnum
        {
            Product = 1, Warranty, Installation, NonStock, Discount
        }
    }
}