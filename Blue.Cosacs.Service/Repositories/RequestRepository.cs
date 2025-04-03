using Blue.Cosacs.Messages.Service;
using Blue.Cosacs.Service.Models;
using Blue.Cosacs.Service.Utils;
using Blue.Events;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using Merch = Blue.Cosacs.Merchandising;

namespace Blue.Cosacs.Service.Repositories
{
    public class RequestRepository
    {
        public RequestRepository(IClock clock, Chub hub, Settings settings, IEventStore audit,
               TechnicianRepository technicianRepository, IContainer container,
               Merch.Settings merchandisingSettings,
               Merch.Repositories.TaxRepository merchandisingTaxRepo)
        {
            this.clock = clock;
            this.hub = hub;
            this.audit = audit;
            this.settings = settings;
            this.technicianRepository = technicianRepository;
            this.container = container;
            this.merchandisingSettings = merchandisingSettings;
            this.merchandisingTaxRepo = merchandisingTaxRepo;

            taxCharge = new Dictionary<string, string>();
            taxCharge.Add("Customer", "Customer Tax");
            taxCharge.Add("Deliverer", "Deliverer Tax");
        }

        private readonly Merch.Settings merchandisingSettings;
        private readonly Merch.Repositories.TaxRepository merchandisingTaxRepo;
        private readonly IClock clock;
        private readonly Chub hub;
        private readonly IEventStore audit;
        private readonly Settings settings;
        private readonly TechnicianRepository technicianRepository;
        private readonly IContainer container;

        private readonly Dictionary<string, string> taxCharge;

        public bool Save(RequestItem request, LastUpdated updated, IEventStore audit, bool isNew)
        {
            var id = request.Id;
            var techSave = false;
            var status = string.Empty;
            var summaryExists = false;
            var resolutionFail = true;
            if (!string.IsNullOrEmpty(request.InvoiceNumber))
            {
                request.InvoiceNumber = request.InvoiceNumber.Replace("-", "");
            }
            using (var readScope = Context.Read())
            {
                summaryExists = (from s in readScope.Context.SummaryView
                                 where s.ServiceRequestNo == request.Id
                                 && s.Acctno == request.Account
                                 select s).Any();

                var resolution = readScope.Context.Resolution
                    .Where(p => p.Description == request.Resolution)
                    .Select(p => new
                    {
                        Order = 1,
                        p.Fail
                    })
                    .Union(readScope.Context.Request
                        .Where(p => p.Id == request.Id)
                        .Select(p => new
                        {
                            Order = 2,
                            Fail = p.ResolutionFail
                        }))
                    .OrderBy(p => p.Order)
                    .FirstOrDefault();

                resolutionFail = (resolution ?? new
                {
                    Order = 1,
                    Fail = true
                }).Fail;

            }

            using (var scope = Context.Write())
            {
                if (!isNew)
                {
                    var serviceReq = (from s in scope.Context.Request
                                      where s.Id == id
                                      select new
                                      {
                                          s.IsClosed,
                                          s.State
                                      }).First();

                    if (serviceReq.IsClosed)
                    {
                        throw new ArgumentException(string.Format("Cannot save the already closed Service Request {0}", id));
                    }

                    var technicianAllocated = scope.Context.TechnicianBooking
                      .Where(t => t.RequestId == request.Id)
                      .OrderByDescending(t => t.AllocatedOn)
                      .Select(t => new
                      {
                          t.UserId
                      })
                      .FirstOrDefault();

                    if (technicianAllocated == null && request.AllocationTechnician.HasValue)
                    {
                        audit.LogAsync(new
                        {
                            ServiceRequestId = request.Id
                        }, EventType.Allocation, EventCategory.Service);
                    }
                    else if (!request.AllocationTechnician.HasValue || technicianAllocated.UserId != request.AllocationTechnician.Value)
                    {

                        audit.LogAsync(new
                        {
                            ServiceRequestId = request.Id
                        }, EventType.UpdateRequest, EventCategory.Service);
                    }

                    status = serviceReq.State;
                    var sr = scope.Context.Request.Find(id);

                    ConvertRequestItemToRequest(request, updated, sr);
                    sr.ResolutionFail = resolutionFail;
                    scope.Context.SaveChanges();

                }
                else
                {
                    var newSr = new Request();
                    ConvertRequestItemToRequest(request, updated, newSr);

                    newSr.ResolutionFail = resolutionFail;

                    audit.LogAsync(new
                    {
                        ServiceRequest = request
                    }, EventType.CreateRequest, EventCategory.Service);

                    scope.Context.Request.Add(newSr);
                    scope.Context.SaveChanges();
                }


                techSave = SaveRequestChildren(id, request, updated);
                scope.Context.SaveChanges();

                // Send messages (inside transaction)
                if (ServiceType.FromString(request.Type) == ServiceType.ServiceRequestInternal
                 && request.Account != null
                 && (request.IsClosed || ((status == "" || status == "New") && !summaryExists)))
                {
                    Summary(request);
                }

                SaveCharges(request);

                if (request.IsClosed)
                {
                    SendCharges(request, updated);
                    SendParts(request);

                    SendWarrantyServiceCompletionMessage(request);

                    SendServiceRequestDetails(request, updated);
                }
                scope.Complete();
            }

            Solr.SolrIndex.IndexRequest(new[] { id });

            return techSave;
        }

        public void SaveComment(int serviceRequest, string comment, string name)
        {
            using (var scope = Context.Write())
            {
                scope.Context.Comment.Add(new Comment
                {
                    AddedBy = name,
                    Date = clock.Now,
                    RequestId = serviceRequest,
                    Text = comment
                });
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void SaveCharges(RequestItem request)
        {
            if (request.Charges != null && request.Charges.Length > 0)
            {
                var charges = from c in request.Charges
                              group c by new
                              {
                                  c.ChargeType,
                                  c.Label
                              } into d
                              select new Charge
                              {
                                  Type = d.FirstOrDefault().ChargeType,
                                  Account = d.FirstOrDefault().Account,
                                  Tax = d.Sum(x => x.Tax),
                                  Value = d.Sum(x => x.Value),
                                  RequestId = request.Id,
                                  Label = d.FirstOrDefault().Label,
                                  Cost = d.FirstOrDefault().Cost,
                                  TaxRate = d.FirstOrDefault().TaxRate,
                                  IsExternal = d.FirstOrDefault().IsExternal
                              };

                using (var scope = Context.Write())
                {
                    scope.Context.Charge.RemoveRange(scope.Context.Charge.Where(p => p.RequestId == request.Id));
                    scope.Context.Charge.AddRange(charges);

                    scope.Context.SaveChanges();
                    scope.Complete();
                }
            }
        }

        private void SendCharges(RequestItem request, LastUpdated updated)
        {
            if (request.Charges == null || request.Charges.Length == 0)
            {
                return;
            }

            var serviceCharges = (from c in request.Charges
                                  select new ServiceCharge
                                  {
                                      Account = c.Account,
                                      Branch = request.Branch,
                                      ChargeType = c.ChargeType,
                                      CustomerId = c.CustomerId ?? request.CustomerId,
                                      ItemNumber = c.ItemNo,
                                      ServiceRequestNo = request.Id,
                                      Tax = c.Tax,
                                      Value = c.Value,
                                      Label = c.Label,
                                      Cost = c.Cost,
                                      IsExternal = c.IsExternal,
                                  }).ToList();

            if (taxCharge.ContainsKey(request.ResolutionPrimaryCharge))
            {
                var customerTaxCharge = (from c in request.Charges
                                         group c by c.ChargeType into grp
                                         select new ServiceCharge
                                         {
                                             Account = request.Account,
                                             Branch = request.Branch,
                                             ChargeType = taxCharge[request.ResolutionPrimaryCharge],
                                             CustomerId = request.CustomerId,
                                             ItemNumber = request.ItemNumber,
                                             ServiceRequestNo = request.Id,
                                             Tax = 0,
                                             Value = grp.Sum(x => x.Tax).HasValue ? grp.Sum(x => x.Tax).Value : 0,
                                             Label = "Tax",
                                             Cost = grp.Sum(x => x.Cost),
                                             IsExternal = null,
                                         }).FirstOrDefault();

                serviceCharges.Add(customerTaxCharge);
            }

            hub.ServiceCharges(
                new ServiceCharges
                {
                    EmpeeNo = Convert.ToInt32(updated.LastUpdatedUser),
                    ServiceType = request.Type,
                    ServiceCharge = serviceCharges.ToArray()
                }
            );


        }

        /// <summary>
        /// Send the parts used in the service request through the HUB to account for stock changes
        /// </summary>
        private void SendParts(RequestItem request)
        {
            if (request.Parts == null || request.Parts.Length == 0)
                return;

            var query = from p in request.Parts
                        where p.number != null // only parts with ItemNo (others are non-registered parts)
                        select new ServicePart
                        {
                            ItemNumber = p.number,
                            Price = p.price,
                            Quantity = p.quantity,
                            StockLocn = p.stockbranch
                        };

            if (query.Count() > 0)
            {
                hub.ServiceParts(new ServiceParts
                {
                    Account = request.Type != "II" && request.Type != "IE" ? settings.ServiceStockAccount : settings.InstallationStockAccount,
                    Branch = request.Branch,
                    ServiceRequestNo = request.Id,
                    ServicePart = query.ToArray()
                });
            }
        }

        private void Summary(RequestItem request)
        {
            hub.ServiceSummary(new ServiceSummary
            {
                Acctno = request.Account,
                ServiceRequestNo = request.Id,
                Branch = request.Branch,
                ItemId = Convert.ToInt32(request.ItemId),
                StockLocn = Convert.ToInt32(request.ItemStockLocation),
                ReplacementIssued = request.ReplacementIssued.HasValue && request.ReplacementIssued.Value,
                DateLogged = request.CreatedOn,
                DateClosed = Convert.ToDateTime(request.FinaliseReturnDate) == new DateTime(0001, 01, 01) ? new DateTime(1900, 01, 01) : Convert.ToDateTime(request.FinaliseReturnDate)
            });
        }

        private void SendServiceRequestDetails(RequestItem request, LastUpdated updated)
        {
            var countryCode = string.Empty;
            using (var readScope = Context.Read())
            {
                countryCode = (from b in readScope.Context.BranchLookup
                               where b.branchno == request.Branch
                               select b.countrycode).FirstOrDefault();
            }

            var serviceDetail = new ServiceDetail
            {
                AccountNumber = request.Account,
                ServiceRequestNo = request.Id,
                RequestType = request.Type,
                StockLocation = Convert.ToInt32(request.ItemStockLocation),
                Branch = request.Branch,
                CountryCode = countryCode,
                DateLogged = request.CreatedOn,
                DateClosed = Convert.ToDateTime(request.FinaliseReturnDate) == new DateTime(0001, 01, 01) ? new DateTime(1900, 01, 01) : Convert.ToDateTime(request.FinaliseReturnDate),
                ReplacementIssued = request.ReplacementIssued.HasValue && request.ReplacementIssued.Value,
                CreatedBy = request.CreatedById,
                LastUpdatedBy = updated.LastUpdatedUser.Value,
                Customer = GetCustomerDetails(request),
                Item = GetItemDetails(request),
                Charges = GetCharges(request),
                Parts = GetParts(request)
            };

            hub.ServiceDetail(serviceDetail);
        }

        private ServicePart[] GetParts(RequestItem request)
        {
            if (request.Parts == null || request.Parts.Length == 0)
            {
                return new ServicePart[] { };
            }

            var query = from p in request.Parts
                        where p.Source == "Internal"
                        select new ServicePart
                        {
                            ItemNumber = p.number,
                            Price = p.price,
                            Quantity = p.quantity,
                            StockLocn = p.stockbranch
                        };

            return query.ToArray();
        }

        private Cosacs.Messages.Service.Charge[] GetCharges(RequestItem request)
        {
            if (request.Charges == null || request.Charges.Length == 0)
            {
                return new Cosacs.Messages.Service.Charge[] { };
            }

            var charges = (from c in request.Charges
                           where (!c.Label.StartsWith("Labour") && (c.Value != 0 || c.Cost != 0)) || c.Label.StartsWith("Labour")
                           select new Cosacs.Messages.Service.Charge
                           {
                               Account = c.Account,
                               ChargeType = c.ChargeType,
                               ItemNumber = c.ItemNo,
                               Tax = c.Tax,
                               Value = c.Value,
                               Label = c.Label,
                               Cost = c.Cost,
                               IsExternal = c.IsExternal
                           }).ToList();

            if (taxCharge.ContainsKey(request.ResolutionPrimaryCharge))
            {
                var customerTaxCharge = (from c in request.Charges
                                         group c by c.ChargeType into grp
                                         select new Cosacs.Messages.Service.Charge
                                         {
                                             Account = request.Account,
                                             ChargeType = taxCharge[request.ResolutionPrimaryCharge],
                                             ItemNumber = request.ItemNumber,
                                             Tax = 0,
                                             Value = grp.Sum(x => x.Tax).HasValue ? grp.Sum(x => x.Tax).Value : 0,
                                             Label = "Tax",
                                             Cost = grp.Sum(x => x.Cost),
                                             IsExternal = null,
                                         }).FirstOrDefault();

                charges.Add(customerTaxCharge);
            }

            return charges.ToArray();
        }

        private Item GetItemDetails(RequestItem request)
        {
            var branch = request.ItemStockLocation ?? 0;

            return new Item
            {
                Id = request.ItemIdNumber,
                DeliveredOn = Convert.ToDateTime(request.ItemDeliveredOn) == new DateTime(0001, 01, 01) ? new DateTime(1900, 01, 01) : Convert.ToDateTime(request.ItemDeliveredOn),
                Model = request.ItemModelNumber,
                Number = request.ItemNumber,
                SerialNumber = request.ItemSerialNumber,
                StockLocn = branch,
                Supplier = request.ItemSupplier,
                LineItemIdentifier = string.IsNullOrWhiteSpace(request.WarrantyGroupId) ? "999999" : request.WarrantyGroupId,
                Hierarchy = new[] {
                    new HierarchyLevel { name = Stock.ProductRelations.Levels["Level_1"] , Value = request.ProductLevel_1 },
                    new HierarchyLevel { name = Stock.ProductRelations.Levels["Level_2"] ,
                        Value = request.ProductLevel_2.HasValue ? request.ProductLevel_2.ToString() : string.Empty },
                    new HierarchyLevel { name = Stock.ProductRelations.Levels["Level_3"] , Value = request.ProductLevel_3 },
                }
            };
        }

        private Customer GetCustomerDetails(RequestItem request)
        {
            if (request.Type == "S")
            {
                return null;
            }

            return new Customer
            {
                CustomerId = request.CustomerId,
                AddressLine1 = request.CustomerAddressLine1,
                AddressLine2 = request.CustomerAddressLine2,
                AddressLine3 = request.CustomerAddressLine3,
                PostCode = request.CustomerPostcode,
                FirstName = request.CustomerFirstName,
                LastName = request.CustomerLastName,
                Title = request.CustomerTitle,
                Notes = request.CustomerNotes
            };
        }

        private void ConvertRequestItemToRequest(RequestItem request, LastUpdated updated, Request destinationRequest)
        {
            destinationRequest.Id = request.Id;
            destinationRequest.Account = request.Account;
            destinationRequest.Branch = request.Branch;
            destinationRequest.CreatedBy = request.CreatedBy;
            destinationRequest.CreatedById = request.CreatedById;
            destinationRequest.CreatedOn = request.Id == 0 ? clock.UtcNow : request.CreatedOn;

            destinationRequest.addtype = request.addtype;
            destinationRequest.CustomerAddressLine1 = request.CustomerAddressLine1;
            destinationRequest.CustomerAddressLine2 = request.CustomerAddressLine2;
            destinationRequest.CustomerAddressLine3 = request.CustomerAddressLine3;
            destinationRequest.CustomerFirstName = request.CustomerFirstName;
            destinationRequest.CustomerId = request.CustomerId;
            destinationRequest.CustomerLastName = request.CustomerLastName;
            destinationRequest.CustomerNotes = request.CustomerNotes;
            destinationRequest.CustomerPostcode = request.CustomerPostcode;
            destinationRequest.CustomerTitle = request.CustomerTitle;
            destinationRequest.Evaluation = request.Evaluation;
            destinationRequest.EvaluationClaimFoodLoss = request.EvaluationClaimFoodLoss;
            destinationRequest.EvaluationAction = request.EvaluationAction;
            destinationRequest.EvaluationLocation = request.EvaluationLocation;
            destinationRequest.AllocationInstructions = request.AllocationInstructions;
            destinationRequest.AllocationItemReceivedOn = request.AllocationItemReceivedOn;
            destinationRequest.AllocationPartExpectOn = request.AllocationPartExpectOn;
            destinationRequest.FinalisedFailure = request.FinalisedFailure;
            destinationRequest.FinaliseReturnDate = request.FinaliseReturnDate;
            destinationRequest.ReplacementIssued = request.ReplacementIssued;
            destinationRequest.ReasonForExchange = request.ReasonForExchange;
            destinationRequest.Resolution = request.Resolution;
            destinationRequest.RepairType = request.RepairType;
            destinationRequest.ResolutionCategory = request.ResolutionCategory;
            destinationRequest.ResolutionDate = request.ResolutionDate;
            destinationRequest.ResolutionReport = request.ResolutionReport;
            destinationRequest.ResolutionSupplierToCharge = request.ResolutionSupplierToCharge;
            destinationRequest.ResolutionPrimaryCharge = request.ResolutionPrimaryCharge;
            destinationRequest.ResolutionAdditionalCost = request.ResolutionAdditionalCost;
            destinationRequest.ResolutionLabourCost = request.ResolutionLabourCost;
            destinationRequest.ResolutionTransportCost = request.ResolutionTransportCost;
            destinationRequest.ResolutionDelivererToCharge = request.ResolutionDelivererToCharge;
            destinationRequest.Type = request.Type;
            destinationRequest.InvoiceNumber = request.InvoiceNumber;
            destinationRequest.Item = request.Item;
            destinationRequest.ItemAmount = request.ItemAmount;
            destinationRequest.ItemDeliveredOn = request.ItemDeliveredOn;
            destinationRequest.ItemId = request.ItemId;
            destinationRequest.ItemNumber = request.ItemNumber;
            destinationRequest.ItemSerialNumber = request.ItemSerialNumber;
            destinationRequest.ItemSoldBy = request.ItemSoldBy;
            destinationRequest.ItemStockLocation = (short?)request.ItemStockLocation;
            destinationRequest.ItemSupplier = request.ItemSupplier;
            destinationRequest.ProductLevel_1 = request.ProductLevel_1;
            destinationRequest.ProductLevel_2 = request.ProductLevel_2;
            destinationRequest.ProductLevel_3 = request.ProductLevel_3;
            destinationRequest.Manufacturer = request.Manufacturer;
            destinationRequest.WarrantyGroupId = request.WarrantyGroupId;
            destinationRequest.State = request.State;
            destinationRequest.TransitNotes = request.TransitNotes;
            destinationRequest.WarrantyNumber = request.WarrantyNumber;
            destinationRequest.WarrantyLength = (byte?)request.WarrantyLength;
            destinationRequest.WarrantyContractNo = request.WarrantyContractNo;
            destinationRequest.WarrantyContractId = request.WarrantyContractId;
            destinationRequest.ManufacturerWarrantyNumber = request.ManufacturerWarrantyNumber;
            destinationRequest.ManufacturerWarrantyContractNo = request.ManufacturerWarrantyContractNo;
            destinationRequest.ManWarrantyLength = (byte?)request.ManufacturerWarrantyLength;
            destinationRequest.ItemModelNumber = request.ItemModelNumber;
            destinationRequest.LastUpdatedOn = updated.LastUpdatedOn;
            destinationRequest.LastUpdatedUser = updated.LastUpdatedUser;
            destinationRequest.LastUpdatedUserName = updated.LastUpdatedUserName;
            destinationRequest.IsClosed = request.IsClosed;
            destinationRequest.IsPaymentRequired = request.IsPaymentRequired;
            destinationRequest.Retailer = request.Retailer;
            destinationRequest.DepositAuthorised = request.DepositAuthorised;
            destinationRequest.DepositRequired = request.DepositRequired;
        }

        private bool SaveRequestChildren(int requestId, RequestItem request, LastUpdated updated)
        {
            var techError = false;
            using (var scope = Context.Write())
            {
                if (request.Contacts != null)
                {
                    scope.Context.RequestContact.RemoveRange(scope.Context.RequestContact.Where(p => p.RequestId == requestId));

                    scope.Context.RequestContact.AddRange(request.Contacts
                        .Select(p => new RequestContact
                        {
                            RequestId = requestId,
                            Value = p.Value,
                            Type = p.Type
                        }));
                }

                if (request.FoodLoss != null)
                {
                    scope.Context.RequestFoodLoss.RemoveRange(scope.Context.RequestFoodLoss.Where(p => p.RequestId == requestId));
                    scope.Context.RequestFoodLoss.AddRange(request.FoodLoss
                        .Select(p => new RequestFoodLoss
                        {
                            RequestId = requestId,
                            Item = p.item,
                            Value = p.value
                        }));
                }

                if (request.Parts != null)
                {
                    scope.Context.RequestPart.RemoveRange(scope.Context.RequestPart.Where(p => p.RequestId == requestId));
                    scope.Context.RequestPart.AddRange(request.Parts
                        .Select(p => new RequestPart
                        {
                            RequestId = requestId,
                            Description = p.description,
                            PartNumber = p.number == null ? "" : p.number,
                            PartType = p.type,
                            Price = p.price,
                            CostPrice = p.CostPrice,
                            TaxAmount = p.TaxAmount,
                            TaxRate = p.TaxRate,
                            Quantity = p.quantity,
                            StockBranch = p.stockbranch,
                            Source = p.Source
                        }));
                }
                else
                {
                    scope.Context.RequestPart.RemoveRange(scope.Context.RequestPart.Where(p => p.RequestId == requestId));
                }

                if (request.ScriptAnswer != null)
                {
                    scope.Context.RequestScriptAnswer.RemoveRange(scope.Context.RequestScriptAnswer.Where(p => p.RequestId == requestId));
                    scope.Context.RequestScriptAnswer.AddRange(request.ScriptAnswer
                        .Select(p => new RequestScriptAnswer
                        {
                            RequestId = requestId,
                            Question = p.Question,
                            Answer = p.Value
                        }));
                }

                if (request.FaultTags != null)
                {
                    scope.Context.FaultTag.RemoveRange(scope.Context.FaultTag.Where(p => p.RequestId == requestId));
                    scope.Context.FaultTag.AddRange(request.FaultTags
                        .Select(p => new FaultTag
                        {
                            RequestId = requestId,
                            Tag = p.Tag
                        }));
                }

                if (!string.IsNullOrWhiteSpace(request.Comment))
                {
                    var old = scope.Context.Comment.FirstOrDefault(p => p.RequestId == requestId);

                    if (old != null)
                    {
                        scope.Context.Comment.Remove(old);
                    }

                    scope.Context.Comment.Add(new Comment
                    {
                        RequestId = request.Id,
                        Date = Convert.ToDateTime(updated.LastUpdatedOn),
                        AddedBy = updated.LastUpdatedUserName,
                        Text = request.Comment

                    });
                }
                //scope.Context.SaveChanges();

                if (!string.IsNullOrWhiteSpace(request.Resolution) && request.ResolutionDate.HasValue && !string.IsNullOrWhiteSpace(request.ResolutionPrimaryCharge))
                {
                    request.AllocationServiceScheduledOn = null;
                    request.AllocationTechnician = null;
                    technicianRepository.CompleteAllocation(request.Id);
                }

                techError = technicianRepository.CheckSave(request, updated);
                //scope.Complete();
            }
            return techError;
        }

        public RequestItem Get(int id)
        {
            if (id == 0)
                return new RequestItem();

            using (var scope = Context.Read())
            {
                var cutid = from r in scope.Context.Request
                            where r.Id == id
                            select new RequestItem()
                            {

                                CustomerId = r.CustomerId,
                            };
                // CR2018-008 by tosif ali 18/10/2018*@
                string custid = "";
                string cusaddr1 = "";
                string cusaddr2 = "";
                foreach (var item in cutid)
                {
                    custid = item.CustomerId;
                    cusaddr1 = item.CustomerAddressLine1;
                    cusaddr2 = item.CustomerAddressLine2;
                }
                // CR2018-008 by tosif ali 19/10/2018*@
                var request = (from r in scope.Context.Request
                               join brn in scope.Context.BranchLookup on r.Branch equals brn.branchno
                               join add in scope.Context.custaddress on new { r.addtype, custid = r.CustomerId } equals new { add.addtype, custid = add.custid }
                               into gj
                               from subpet in gj.DefaultIfEmpty()
                               where r.Id == id

                               select new RequestItem()
                               {
                                   Id = r.Id,
                                   Account = r.Account,
                                   Branch = r.Branch,
                                   BranchName = brn.BranchNameLong,
                                   CreatedBy = r.CreatedBy,
                                   CreatedOn = r.CreatedOn,
                                   CustomerAddressLine1 = r.CustomerAddressLine1,
                                   CustomerAddressLine2 = r.CustomerAddressLine2,
                                   CustomerAddressLine3 = r.CustomerAddressLine3,
                                   CustomerFirstName = r.CustomerFirstName,
                                   CustomerId = r.CustomerId,
                                   CustomerLastName = r.CustomerLastName,
                                   CustomerNotes = r.CustomerNotes,
                                   DELTitleC = subpet.DELTitleC,
                                   DELFirstName = subpet.DELFirstName,
                                   DELLastName = subpet.DELLastName,
                                   addtype = subpet.addtype,
                                   CustomerPostcode = r.CustomerPostcode,
                                   CustomerTitle = r.CustomerTitle,
                                   Evaluation = r.Evaluation,
                                   EvaluationClaimFoodLoss = r.EvaluationClaimFoodLoss,
                                   EvaluationAction = r.EvaluationAction,
                                   EvaluationLocation = r.EvaluationLocation,
                                   AllocationInstructions = r.AllocationInstructions,
                                   AllocationItemReceivedOn = r.AllocationItemReceivedOn,
                                   AllocationPartExpectOn = r.AllocationPartExpectOn,
                                   CreatedById = r.CreatedById,
                                   FinalisedFailure = r.FinalisedFailure,
                                   FinaliseReturnDate = r.FinaliseReturnDate,
                                   ReplacementIssued = r.ReplacementIssued,
                                   ReasonForExchange = r.ReasonForExchange,
                                   Resolution = r.Resolution,
                                   ResolutionCategory = r.ResolutionCategory,
                                   ResolutionDate = r.ResolutionDate,
                                   RepairType = r.RepairType,
                                   ResolutionReport = r.ResolutionReport,
                                   ResolutionSupplierToCharge = r.ResolutionSupplierToCharge,
                                   ResolutionPrimaryCharge = r.ResolutionPrimaryCharge,
                                   ResolutionAdditionalCost = r.ResolutionAdditionalCost,
                                   ResolutionLabourCost = r.ResolutionLabourCost,
                                   ResolutionTransportCost = r.ResolutionTransportCost,
                                   ResolutionDelivererToCharge = r.ResolutionDelivererToCharge,
                                   Type = r.Type.Trim(),
                                   InvoiceNumber = r.InvoiceNumber,
                                   Item = r.Item,
                                   ItemAmount = r.ItemAmount,
                                   ItemDeliveredOn = r.ItemDeliveredOn,
                                   ItemId = r.ItemId,
                                   ItemNumber = r.ItemNumber,
                                   ItemSerialNumber = r.ItemSerialNumber,
                                   ItemSoldBy = r.ItemSoldBy,
                                   ItemStockLocation = r.ItemStockLocation,
                                   ItemSupplier = r.ItemSupplier,
                                   ProductLevel_1 = r.ProductLevel_1,
                                   ProductLevel_2 = r.ProductLevel_2,
                                   ProductLevel_3 = r.ProductLevel_3,
                                   Manufacturer = r.Manufacturer,
                                   WarrantyGroupId = r.WarrantyGroupId,
                                   State = r.State,
                                   TransitNotes = r.TransitNotes,
                                   WarrantyNumber = r.WarrantyNumber,
                                   WarrantyLength = r.WarrantyLength,
                                   WarrantyContractNo = r.WarrantyContractNo,
                                   WarrantyContractId = r.WarrantyContractId,
                                   IsPaymentRequired = r.IsPaymentRequired,
                                   IsClosed = r.IsClosed,
                                   ItemModelNumber = r.ItemModelNumber,
                                   ManufacturerWarrantyNumber = r.ManufacturerWarrantyNumber,
                                   ManufacturerWarrantyContractNo = r.ManufacturerWarrantyContractNo,
                                   ManufacturerWarrantyLength = r.ManWarrantyLength,
                                   Retailer = r.Retailer,
                                   DepositAuthorised = r.DepositAuthorised,
                                   DepositRequired = r.DepositRequired
                               }).FirstOrDefault();

                if (request == null)
                {
                    return null;
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.InvoiceNumber))
                    {
                        if (request.InvoiceNumber.Length > 12)
                        {
                            if (request.InvoiceNumber.Contains("-"))
                            {
                                request.InvoiceNumber = request.InvoiceNumber.Replace("-", "").Replace("--", "").Trim();
                            }
                            request.InvoiceNumber = request.InvoiceNumber.Insert(3, "-");
                        }
                    }
                }
                // CR2018-008 by tosif ali 16/10/2018*@
                var Address = (from t in scope.Context.AddressCode
                               join a in scope.Context.custaddress on t.code.Trim() equals a.addtype.Trim()
                               where t.category == "CA1" && a.custid == custid
                               select new RequestItem.Addresses()
                               {
                                   codedescript = t.codedescript,
                                   code = t.code,
                                   CustomerAddressLine1 = a.cusaddr1,
                                   CustomerAddressLine2 = a.cusaddr2,
                                   CustomerAddressLine3 = a.cusaddr3,
                                   CustomerPostcode = a.cuspocode,
                                   CustomerNotes = a.notes
                               }).ToList();

                request.Address = Address.ToArray();
                //End Hear

                request.Contacts = (from c in scope.Context.RequestContact
                                    where c.RequestId == id
                                    select new RequestItem.ContactItem()
                                    {
                                        Type = c.Type,
                                        Value = c.Value
                                    }).ToArray();
                request.FoodLoss = (from f in scope.Context.RequestFoodLoss
                                    where f.RequestId == id
                                    select new RequestItem.FoodLossItem()
                                    {
                                        item = f.Item,
                                        value = f.Value
                                    }).ToArray();

                request.Parts = (from p in scope.Context.RequestPartView
                                 where p.RequestId == id
                                 select new RequestItem.PartItem()
                                 {
                                     description = p.Description,
                                     number = p.PartNumber,
                                     price = p.Price,
                                     CostPrice = p.CostPrice.HasValue ? p.CostPrice.Value : 0,
                                     TaxAmount = p.TaxAmount.HasValue ? p.TaxAmount.Value : 0,
                                     quantity = p.Quantity,
                                     stockbranch = p.StockBranch,
                                     type = p.PartType,
                                     Source = p.Source,
                                     TaxRate = p.TaxRate.Value,
                                     CashPrice = p.CashPrice
                                 })
                                 .ToArray();

                request.ScriptAnswer = (from sa in scope.Context.RequestScriptAnswer
                                        where sa.RequestId == id
                                        select new RequestItem.ScriptAnswerItem()
                                        {
                                            Question = sa.Question,
                                            Value = sa.Answer.Trim()
                                        }).ToArray();
                request.FaultTags = (from ft in scope.Context.FaultTag
                                     where ft.RequestId == id
                                     select new RequestItem.FaultTag
                                     {
                                         Tag = ft.Tag
                                     }).ToArray();


                request.History = (from h in scope.Context.HistoryView
                                   where h.RequestId == id
                                   select new RequestItem.HistoryItem()
                                   {
                                       UpdatedOn = h.LastUpdatedOn,
                                       CreatedOn = h.CreatedOn,
                                       RequestId = h.Id,
                                       Status = h.State
                                   }).OrderByDescending(h => h.UpdatedOn).ToArray();

                var historyIds = request.History.Select(h => h.RequestId).ToArray();

                request.HistoryCharges = (from c in scope.Context.Charge
                                          where historyIds.Contains(c.RequestId)
                                          select new RequestItem.ChargeItem
                                          {
                                              Account = c.Account,
                                              ChargeType = c.Type,
                                              Tax = c.Tax,
                                              Value = c.Value,
                                              RequestId = c.RequestId
                                          }).ToArray();

                request.Charges = (from c in scope.Context.Charge
                                   where c.RequestId == id
                                   select new RequestItem.ChargeItem
                                   {
                                       Account = c.Account,
                                       ChargeType = c.Type,
                                       Tax = c.Tax,
                                       Label = c.Label,
                                       Value = c.Value,
                                       RequestId = c.RequestId
                                   }).ToArray();

                request.StockItem = (from si in scope.Context.StockView
                                     where si.ItemNumber == request.ItemNumber && si.Location == request.ItemStockLocation
                                     select new RequestItem.RequestStockItem
                                     {
                                         CostPrice = si.CostPrice
                                     }).FirstOrDefault();

                var techbooking = (from tb in scope.Context.TechnicianBooking
                                   join t in scope.Context.Technician on tb.UserId equals t.UserId
                                   join a in scope.Context.AdminUserView on tb.UserId equals a.Id
                                   join s in scope.Context.RequestSlotTimesView on tb.RequestId equals s.RequestId       //#11785
                                   where tb.RequestId == id && !tb.Reject
                                   select new
                                   {
                                       booking = tb,
                                       t,
                                       a,
                                       s
                                   }).OrderByDescending(b => b.booking.Id).FirstOrDefault();

                if (techbooking != null)
                {
                    request.AllocationServiceScheduledOn = techbooking.booking.Date;
                    request.AllocationSlots = techbooking.booking.Slot;
                    request.AllocationSlotExtend = techbooking.booking.SlotExtend;
                    request.AllocationTechnician = techbooking.booking.UserId;
                    request.AllocationTechnicianName = techbooking.a.FullName;
                    request.AllocationTechnicianIsInternal = techbooking.t.Internal;
                    request.SlotStartTime = techbooking.s.SlotStartTime;
                    request.SlotEndTime = techbooking.s.SlotEndTime;
                }

                request.RequestComments = (from c in scope.Context.Comment
                                           where c.RequestId == id
                                           select new RequestItem.RequestComment
                                           {
                                               Date = c.Date,
                                               AddedBy = c.AddedBy,
                                               Comment = c.Text
                                           }).OrderByDescending(d => d.Date).ToArray();

                Nullable<decimal> balance = scope.Context.Payment
                    .Where(p => p.RequestId == request.Id)
                    .Select(p => (decimal?)p.Amount)
                    .Sum();

                request.PaymentBalance = balance;

                return request;
            }
        }

        public IEnumerable<RequestItem> GetMany(IEnumerable<int> ids)
        {
            if (ids.Count() == 0)
                return new List<RequestItem>();

            using (var scope = Context.Read())
            {
                var requests = (from r in scope.Context.Request
                                join brn in scope.Context.BranchLookup on r.Branch equals brn.branchno
                                where ids.Contains(r.Id)
                                select new RequestItem()
                                {
                                    Id = r.Id,
                                    Account = r.Account,
                                    Branch = r.Branch,
                                    BranchName = brn.BranchNameLong,
                                    CreatedBy = r.CreatedBy,
                                    CreatedOn = r.CreatedOn,
                                    CustomerAddressLine1 = r.CustomerAddressLine1,
                                    CustomerAddressLine2 = r.CustomerAddressLine2,
                                    CustomerAddressLine3 = r.CustomerAddressLine3,
                                    CustomerFirstName = r.CustomerFirstName,
                                    CustomerId = r.CustomerId,
                                    CustomerLastName = r.CustomerLastName,
                                    CustomerNotes = r.CustomerNotes,
                                    CustomerPostcode = r.CustomerPostcode,
                                    CustomerTitle = r.CustomerTitle,
                                    Evaluation = r.Evaluation,
                                    EvaluationClaimFoodLoss = r.EvaluationClaimFoodLoss,
                                    EvaluationAction = r.EvaluationAction,
                                    EvaluationLocation = r.EvaluationLocation,
                                    AllocationInstructions = r.AllocationInstructions,
                                    AllocationItemReceivedOn = r.AllocationItemReceivedOn,
                                    AllocationPartExpectOn = r.AllocationPartExpectOn,
                                    CreatedById = r.CreatedById,
                                    FinalisedFailure = r.FinalisedFailure,
                                    FinaliseReturnDate = r.FinaliseReturnDate,
                                    Resolution = r.Resolution,
                                    ResolutionCategory = r.ResolutionCategory,
                                    ResolutionDate = r.ResolutionDate,
                                    ResolutionReport = r.ResolutionReport,
                                    ResolutionSupplierToCharge = r.ResolutionSupplierToCharge,
                                    ResolutionPrimaryCharge = r.ResolutionPrimaryCharge,
                                    ResolutionAdditionalCost = r.ResolutionAdditionalCost,
                                    ResolutionLabourCost = r.ResolutionLabourCost,
                                    ResolutionTransportCost = r.ResolutionTransportCost,
                                    ResolutionDelivererToCharge = r.ResolutionDelivererToCharge,
                                    Type = r.Type.Trim(),
                                    InvoiceNumber = r.InvoiceNumber,
                                    Item = r.Item,
                                    ItemAmount = r.ItemAmount,
                                    ItemDeliveredOn = r.ItemDeliveredOn,
                                    ItemId = r.ItemId,
                                    ItemNumber = r.ItemNumber,
                                    ItemSerialNumber = r.ItemSerialNumber,
                                    ItemSoldBy = r.ItemSoldBy,
                                    ItemStockLocation = r.ItemStockLocation,
                                    ItemSupplier = r.ItemSupplier,
                                    WarrantyGroupId = r.WarrantyGroupId,
                                    State = r.State,
                                    TransitNotes = r.TransitNotes,
                                    WarrantyNumber = r.WarrantyNumber,
                                    WarrantyLength = r.WarrantyLength,
                                    WarrantyContractNo = r.WarrantyContractNo,
                                    IsPaymentRequired = r.IsPaymentRequired,
                                    IsClosed = r.IsClosed,
                                    ItemModelNumber = r.ItemModelNumber,
                                    ManufacturerWarrantyNumber = r.ManufacturerWarrantyNumber,
                                    ManufacturerWarrantyContractNo = r.ManufacturerWarrantyContractNo,
                                    ManufacturerWarrantyLength = r.ManWarrantyLength
                                }).ToList();


                var contacts = (from c in scope.Context.RequestContact
                                where ids.Contains(c.RequestId)
                                select new
                                {
                                    Type = c.Type,
                                    Value = c.Value,
                                    RequestId = c.RequestId
                                }).ToLookup(k => k.RequestId);



                var foodLoss = (from f in scope.Context.RequestFoodLoss
                                where ids.Contains(f.RequestId)
                                select new
                                {
                                    item = f.Item,
                                    value = f.Value,
                                    RequestId = f.RequestId
                                }).ToLookup(k => k.RequestId);

                var parts = (from p in scope.Context.RequestPart
                             where ids.Contains(p.RequestId)
                             select new
                             {
                                 description = p.Description,
                                 number = p.PartNumber,
                                 price = p.Price,
                                 quantity = p.Quantity,
                                 stockbranch = p.StockBranch,
                                 type = p.PartType,
                                 RequestId = p.RequestId
                             }).ToLookup(k => k.RequestId);

                var scriptAnswer = (from sa in scope.Context.RequestScriptAnswer
                                    where ids.Contains(sa.RequestId)
                                    select new
                                    {
                                        Question = sa.Question,
                                        Value = sa.Answer.Trim(),
                                        RequestId = sa.RequestId
                                    }).ToLookup(k => k.RequestId);

                var faultTags = (from ft in scope.Context.FaultTag
                                 where ids.Contains(ft.RequestId)
                                 select new
                                 {
                                     Tag = ft.Tag,
                                     RequestId = ft.RequestId
                                 }).ToLookup(k => k.RequestId);

                var techbooking = (from tb in scope.Context.TechnicianBooking
                                   join t in scope.Context.Technician on tb.UserId equals t.UserId
                                   join a in scope.Context.AdminUserView on tb.UserId equals a.Id
                                   where ids.Contains(tb.RequestId) && !tb.Reject
                                   select new
                                   {
                                       tb = tb,
                                       t = t,
                                       a
                                   }).ToDictionary(k => k.tb.RequestId);

                var requestComments = (from c in scope.Context.Comment
                                       where ids.Contains(c.RequestId)
                                       select new
                                       {
                                           Date = c.Date,
                                           AddedBy = c.AddedBy,
                                           Comment = c.Text,
                                           RequestId = c.RequestId
                                       }).ToLookup(x => x.RequestId);

                var PaymentBalance = (from p in scope.Context.Payment
                                      where ids.Contains(p.RequestId)
                                      select new
                                      {
                                          p.Amount,
                                          p.RequestId
                                      }).ToLookup(p => p.RequestId);

                var historyView = (from h in scope.Context.HistoryView
                                   where ids.Contains(h.RequestId)
                                   select h).ToLookup(h => h.RequestId);

                requests.ForEach(r =>
                {
                    r.Contacts = (from c in contacts[r.Id]
                                  select new RequestItem.ContactItem()
                                  {
                                      Type = c.Type,
                                      Value = c.Value,
                                  }).ToArray();

                    r.FoodLoss = (from c in foodLoss[r.Id]
                                  select new RequestItem.FoodLossItem()
                                  {
                                      item = c.item,
                                      value = c.value,
                                  }).ToArray();
                    r.Parts = (from p in parts[r.Id]
                               select new RequestItem.PartItem()
                               {
                                   description = p.description,
                                   number = p.number,
                                   price = p.price,
                                   quantity = p.quantity,
                                   stockbranch = p.stockbranch,
                                   type = p.type,
                               }).ToArray();

                    r.ScriptAnswer = (from sa in scriptAnswer[r.Id]
                                      select new RequestItem.ScriptAnswerItem()
                                      {
                                          Question = sa.Question,
                                          Value = sa.Value
                                      }).ToArray();
                    r.FaultTags = (from ft in faultTags[r.Id]
                                   select new RequestItem.FaultTag
                                   {
                                       Tag = ft.Tag
                                   }).ToArray();

                    r.RequestComments = (from c in requestComments[r.Id]
                                         select new RequestItem.RequestComment
                                         {
                                             Date = c.Date,
                                             AddedBy = c.AddedBy,
                                             Comment = c.Comment
                                         }).OrderByDescending(c => c.Date).ToArray();

                    r.PaymentBalance = PaymentBalance[r.Id].Sum(p => p.Amount);


                    if (techbooking.Keys.Contains(r.Id))
                    {
                        var t = techbooking[r.Id];
                        r.AllocationServiceScheduledOn = t.tb.Date;
                        r.AllocationSlots = t.tb.Slot;
                        r.AllocationSlotExtend = t.tb.SlotExtend;
                        r.AllocationTechnician = t.tb.UserId;
                        r.AllocationTechnicianName = t.a.FullName;
                        r.AllocationTechnicianIsInternal = t.t.Internal;
                    }

                    r.History = (from h in historyView[r.Id]
                                 select new RequestItem.HistoryItem()
                                 {
                                     UpdatedOn = h.LastUpdatedOn,
                                     CreatedOn = h.CreatedOn,
                                     RequestId = h.Id,
                                     Status = h.State
                                 }).OrderByDescending(h => h.UpdatedOn).ToArray();
                });

                return requests;
            }
        }

        public CustomerFoodLoss GetFoodLoss(int id)
        {
            using (var scope = Context.Read())
            {
                var cfl = (from r in scope.Context.Request
                           join tb in scope.Context.TechnicianBooking on r.Id equals tb.RequestId into j
                           from tb in j.DefaultIfEmpty()
                           where r.Id == id
                           select new CustomerFoodLoss()
                           {
                               Id = r.Id,
                               Branch = r.Branch,
                               CreatedBy = r.CreatedBy,
                               CreatedOn = r.CreatedOn,
                               CreatedById = r.CreatedById,
                               CustomerAddressLine1 = r.CustomerAddressLine1,
                               CustomerAddressLine2 = r.CustomerAddressLine2,
                               CustomerAddressLine3 = r.CustomerAddressLine3,
                               CustomerFirstName = r.CustomerFirstName,
                               CustomerId = r.CustomerId,
                               CustomerLastName = r.CustomerLastName,
                               CustomerPostcode = r.CustomerPostcode,
                               CustomerTitle = r.CustomerTitle,
                               AllocationTechnician = tb.UserId == null ? 0 : tb.UserId,
                               AllocationServiceScheduledOn = tb.Date == null ? new DateTime(1900, 01, 01) : tb.Date
                           }).First();

                cfl.FoodLoss = (from f in scope.Context.RequestFoodLoss
                                where f.RequestId == id
                                select new CustomerFoodLoss.FoodLossItem()
                                {
                                    item = f.Item,
                                    value = f.Value
                                }).ToArray();
                return cfl;
            }
        }

        //Return Service Request details for Batch Print
        public IEnumerable<RequestItem> BatchPrint(IEnumerable<int> requestIds)
        {
            using (var scope = Context.Read())
            {
                UpdatePrinted(requestIds.ToArray());
                Solr.SolrIndex.IndexRequest(requestIds.ToArray());
                var requests = GetMany(requestIds);
                return requests;
            }
        }

        public IList<ChargeToView> GetChargeTo()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ChargeToView.ToList();
            }
        }

        public IList<PrimaryChargeToView> PrimaryGetChargeTo()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.PrimaryChargeToView.ToList();
            }
        }

        //Return Service Request details for Summary Print
        public IEnumerable<SummaryPrintView> SummaryPrint(IEnumerable<int> requestIds)
        {
            using (var scope = Context.Read())
            {
                var printDet = (from b in scope.Context.SummaryPrintView
                                where requestIds.Contains(b.RequestId)
                                select b).ToList();

                return printDet;
            }
        }


        //#11627 - Return Stockitems
        public IEnumerable<StockViewResult> StockSearch(string search, int branch, char type)
        {
            using (var scope = Context.Read())
            {
                var taxType = merchandisingSettings.TaxInclusive ? "I" : "E";
                var taxRate = GetCountryTaxRate();

                //var countryInfo = GetCountryTaxInfo();
                //var taxType = countryInfo.taxtype;
                var globalTaxRate = taxRate;

                var stockItems = (from s in scope.Context.StockView
                                  join brn in scope.Context.BranchLookup on s.Location equals brn.branchno
                                  where
                                    s.Location == branch &&
                                    (s.ItemNumber.Contains(search) || s.Description1.Contains(search) || s.Description2.Contains(search)) &&
                                    !string.Equals("Y", s.deleted)
                                    && ((type == 'P' && s.SparePart == true) || type == 'S')
                                  select new StockViewResult
                                  {
                                      ItemNumber = s.ItemNumber,
                                      Location = s.Location,
                                      Description1 = s.Description1,
                                      Description2 = s.Description2,
                                      StockOnHand = s.StockOnHand,
                                      CashPrice = s.CashPrice,
                                      CostPrice = s.CostPrice,
                                      TaxRate = s.taxrate,
                                      Supplier = s.Supplier,
                                      WarrantyLength = s.WarrantyLength,
                                      deleted = s.deleted,
                                      SparePart = s.SparePart,
                                      LocationName = brn.BranchNameLong
                                  }).Take(50).ToList();

                stockItems.ForEach(e =>
                {
                    var taxInclusivePrice = 0.00m;
                    var taxExclusivePrice = 0.00m;

                    if (e.TaxRate <= 0)
                    {
                        e.TaxRate = double.Parse(globalTaxRate.ToString());
                    }

                    var taxRatio = ((decimal)e.TaxRate) / 100.00m;
                    var cosacsCashPrice = e.CashPrice.HasValue ? e.CashPrice.Value : 0;
                    if (taxType == "I")
                    {
                        taxInclusivePrice = cosacsCashPrice;
                        if (taxRatio >= 0) // divide by zero protection
                        {
                            taxExclusivePrice = cosacsCashPrice / (1 + taxRatio);
                        }
                        else
                        {
                            taxExclusivePrice = 0;
                        }
                    }
                    else if (taxType == "E")
                    {
                        taxInclusivePrice = cosacsCashPrice + (cosacsCashPrice * taxRatio);
                        taxExclusivePrice = cosacsCashPrice;
                    }

                    e.CashPrice = Math.Round(taxExclusivePrice, 3, MidpointRounding.AwayFromZero);
                    var retTaxAmount = taxInclusivePrice - taxExclusivePrice;
                    e.TaxAmount = Math.Round(retTaxAmount, 3, MidpointRounding.AwayFromZero); // round calculated tax amount
                });

                return stockItems;
            }
        }

        ////private static string _TaxTypeCache = null;
        ////private static double? _TaxRateCache = null;

        ////private static CountryView GetCountryTaxInfo()
        ////{
        ////    var retVal = new CountryView();

        ////    if (string.IsNullOrWhiteSpace(_TaxTypeCache) || !_TaxRateCache.HasValue)
        ////    {
        ////        using (var readScope = Context.Read())
        ////        {
        ////            var countryInfo = (from c in readScope.Context.CountryView
        ////                               select c)
        ////                               .FirstOrDefault();

        ////            _TaxTypeCache = countryInfo.taxtype;
        ////            _TaxRateCache = countryInfo.taxrate;

        ////            #region Validate taxType and taxRate

        ////            if (!_TaxRateCache.HasValue || _TaxRateCache < 0)
        ////            {
        ////                throw new Exception("Error: The 'Service.CountryView' fail to read a valid tax rate. The global Tax Rate must be defined!");
        ////            }

        ////            _TaxTypeCache = _TaxTypeCache.ToUpper().Trim();
        ////            if (_TaxTypeCache != "E" && _TaxTypeCache != "I") // Tax type must be 'E' or 'I' (Exclusive or Inclusive)
        ////            {
        ////                throw new Exception(
        ////                  string.Format("Error: The 'Service.CountryView' fail to read a valid tax type. Cannot process unknown TaxType: {0}.", _TaxTypeCache));
        ////            }

        ////            #endregion
        ////        }
        ////    }

        ////    retVal.taxtype = _TaxTypeCache;
        ////    retVal.taxrate = _TaxRateCache;

        ////    return retVal;
        ////}

        public void UpdatePrinted(int[] ids)
        {
            using (var scope = Context.Write())
            {
                var request = (from s in scope.Context.Request
                               where ids.Contains(s.Id)
                               select s).ToList();

                if (request.Count() != 0)
                    request.ForEach(r => r.Printed = true);

                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        private void SendWarrantyServiceCompletionMessage(RequestItem request)
        {
            if (request.Type != "SI")
            {
                return;
            }

            if (request.ItemDeliveredOn.Value.AddMonths(request.WarrantyLength.HasValue ? request.WarrantyLength.Value : 12) < clock.UtcNow)
            {
                return;
            }

            var tmpManufacturerWarrantyLength = request.ManufacturerWarrantyLength.HasValue ? (int)request.ManufacturerWarrantyLength.Value : 0;
            var tmpWarrantyLength = request.WarrantyLength.HasValue ? (int)request.WarrantyLength.Value : 0;

            var isManufacturerWarranty = request.ItemDeliveredOn.Value.AddMonths(tmpManufacturerWarrantyLength) > clock.UtcNow;

            var warrantyNumber = isManufacturerWarranty ? request.ManufacturerWarrantyNumber : request.WarrantyNumber;

            if (string.IsNullOrEmpty(warrantyNumber))
            {
                return;
            }

            var warrantyServiceDetail = new WarrantyServiceDetail
            {
                AccountNumber = request.Account,
                ServiceRequestNo = request.Id,
                ServiceBranch = request.Branch,
                DateLogged = request.CreatedOn,
                DateClosed = request.FinaliseReturnDate.Value,
                Resolution = request.Resolution,
                ReplacementIssued = request.ReplacementIssued.HasValue && request.ReplacementIssued.Value,
                Charges = (from c in request.Charges
                           group c by c.ChargeType into d
                           select new WarrantyServiceCharge
                           {
                               Type = d.FirstOrDefault().ChargeType,
                               TotalCost = Math.Round(d.Sum(x => x.Value), 2),
                               LabourCost = Math.Round(d.Where(x => x.Label == "Labour").Sum(x => x.Value), 2),
                               AdditionalPartsCost = Math.Round(d.Where(x => x.Label == "Parts Other").Sum(x => x.Value), 2)
                           }).ToArray(),
                Item = new Item
                {
                    Id = Convert.ToInt32(request.ItemId),
                    SerialNumber = request.ItemSerialNumber,
                    LineItemIdentifier = string.IsNullOrWhiteSpace(request.WarrantyGroupId) ? "999999" : request.WarrantyGroupId,
                    Number = request.ItemNumber,
                    Model = request.ItemModelNumber,
                    Supplier = request.ItemSupplier,
                    StockLocn = request.ItemStockLocation.Value,
                    DeliveredOn = request.ItemDeliveredOn.Value
                },
                Warranty = new Blue.Cosacs.Messages.Service.Warranty
                {
                    Number = warrantyNumber,
                    Length = isManufacturerWarranty ? tmpManufacturerWarrantyLength : tmpWarrantyLength,
                    ContractNumber = isManufacturerWarranty ? request.ManufacturerWarrantyContractNo : request.WarrantyContractNo,
                    WarrantyContractId = isManufacturerWarranty ? null : (int?)request.WarrantyContractId
                }
            };

            hub.WarrantyServiceCompleted(warrantyServiceDetail);
        }

        public ReceiptBranchDetailsView GetBranchDetails(short branchNo)
        {
            using (var scope = Context.Read())
            {
                return (from b in scope.Context.ReceiptBranchDetailsView
                        where b.BranchNo == branchNo
                        select b).First();
            }
        }

        public int CheckForOpenServiceRequests(string acctno)
        {
            var serviceRequestCount = 0;

            using (var scope = Context.Read())
            {
                serviceRequestCount = (from r in scope.Context.Request
                                       where r.Account == acctno &&
                                       r.State != ServiceState.Closed
                                       select r.Id).Count();
            }

            return serviceRequestCount;
        }

        private decimal GetCountryTaxRate()
        {
            var currentTaxRateObj = merchandisingTaxRepo.GetCurrent();
            if (currentTaxRateObj != null)
            {
                return merchandisingTaxRepo.GetCurrent().Rate * 100;
            }
            return 0;
        }
        // Method Name      : GetForceReIndexableService
        // Method Details   : GetForceReIndexableService will return an integer array of service request which need to Force Reindexing
        // Author           : RD
        //Date              : 11/ May / 2019
        //CR                : #
        public void GetForceReIndexableService()
        {
            using (var scope = Context.Read())
            {
                var SRids = (from r in scope.Context.Request
                             where r.IsForceReIndexRequired == true
                             select r.Id).ToArray();
                if (SRids.Length > 0)
                {
                    Solr.SolrIndex.IndexRequest(SRids);
                }
            }
            using (var scope = Context.Write())
            {
                var requests = (from r in scope.Context.Request
                                where r.IsForceReIndexRequired == true
                                select r).ToList();
                if (requests.Count > 0)
                {
                    foreach (var r in requests)
                    {
                        r.IsForceReIndexRequired = false;
                    }
                    scope.Context.SaveChanges();
                    scope.Complete();
                }

            }
        }

    }
}
