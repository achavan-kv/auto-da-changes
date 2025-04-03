using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Service.Models;
using Blue.Events;
using System;

namespace Blue.Cosacs.Service.Repositories
{
    public class ServiceLabourException : Exception
    {
        public ServiceLabourException(string message) : base(message)
        {
        }
    }

    public class ChargesRepository
    {
        private readonly IEventStore audit;
        private const string AuditEventCategory = "LabourCostMatrix";

        public ChargesRepository(IEventStore audit)
        {
            this.audit = audit;
        }

        public int Save(ServiceLabour serviceLabour)
        {
            var id = 0;
            LabourCostMatrix original = null;

            if (!ValidateServiceLabour(serviceLabour))
            {
                throw new ServiceLabourException(string.Format("There is already a Labour Cost Matrix with the label '{0}'", serviceLabour.Label));
            }

            using (var scope = Context.Write())
            {
                if (serviceLabour.Id > 0)
                {
                    original = scope.Context.LabourCostMatrix.Find(serviceLabour.Id);
                    if (original != null)
                        scope.Context.LabourCostMatrix.Remove(original);
                }

                var newCost = new LabourCostMatrix
                {
                    ContractedTechnician = serviceLabour.ChargeContractedTech,
                    CustomerCharge = serviceLabour.ChargeCustomer,
                    EWClaim = serviceLabour.ChargeEWClaim,
                    InternalTechnician = serviceLabour.ChargeInternalTech,
                    ItemList = serviceLabour.ItemList,
                    Level_1 = serviceLabour.Level_1,
                    Level_2 = serviceLabour.Level_2,
                    Level_3 = serviceLabour.Level_3,
                    Supplier = serviceLabour.Supplier,
                    Type = serviceLabour.RepairType,
                    IsGroupFilter = serviceLabour.IsGroupFilter,
                    Label = serviceLabour.Label

                };
                scope.Context.LabourCostMatrix.Add(newCost);
                scope.Context.SaveChanges();
                id = newCost.Id;
                scope.Complete();
            };

            if (original != null)
            {
                audit.LogAsync(new
                {
                    Original = original,
                    Modified = serviceLabour
                }, EventType.EditMatrixEntry, AuditEventCategory);
            }
            else
            {
                audit.LogAsync(serviceLabour, EventType.CreateMatrixEntry, AuditEventCategory);
            }

            return id;
        }

        private bool ValidateServiceLabour(ServiceLabour value)
        {
            using (var scope = Context.Read())
            {
                var id = value.Id.HasValue ? value.Id.Value : 0;
                return scope.Context.LabourCostMatrix.Count(p => p.Label == value.Label && p.Id != id) == 0;
            }
        }

        public ServiceLabour[] GetAll()
        {
            using (var scope = Context.Read())
            {
                return (from lcm in scope.Context.LabourCostMatrix
                        orderby lcm.Id
                        select new ServiceLabour
                        {
                            ChargeContractedTech = lcm.ContractedTechnician,
                            ChargeCustomer = lcm.CustomerCharge,
                            ChargeEWClaim = lcm.EWClaim,
                            ChargeInternalTech = lcm.InternalTechnician,
                            ItemList = lcm.ItemList,
                            Level_1 = lcm.Level_1,
                            Level_2 = lcm.Level_2,
                            Level_3 = lcm.Level_3,
                            Supplier = lcm.Supplier,
                            RepairType = lcm.Type,
                            IsGroupFilter = lcm.IsGroupFilter,
                            Id = lcm.Id,
                            Label = lcm.Label
                        }).ToArray();
            }
        }

        public ServiceLabour Get(int id)
        {
            using (var scope = Context.Read())
            {
                return (from lcm in scope.Context.LabourCostMatrix
                        where lcm.Id == id
                        select new ServiceLabour
                        {
                            ChargeContractedTech = lcm.ContractedTechnician,
                            ChargeCustomer = lcm.CustomerCharge,
                            ChargeEWClaim = lcm.EWClaim,
                            ChargeInternalTech = lcm.InternalTechnician,
                            ItemList = lcm.ItemList,
                            Level_1 = lcm.Level_1,
                            Level_2 = lcm.Level_2,
                            Level_3 = lcm.Level_3,
                            Supplier = lcm.Supplier,
                            RepairType = lcm.Type,
                            IsGroupFilter = lcm.IsGroupFilter,
                            Id = lcm.Id,
                            Label = lcm.Label
                        }).First();
                ;
            }
        }

        public void Delete(int id)
        {
            LabourCostMatrix lcm = null;
            using (var scope = Context.Write())
            {
                lcm = scope.Context.LabourCostMatrix.Find(id);
                if (lcm != null)
                {
                    scope.Context.LabourCostMatrix.Remove(lcm);
                }
                scope.Context.SaveChanges();
                scope.Complete();
            }

            audit.LogAsync(lcm, EventType.DeleteMatrixEntry, AuditEventCategory);
        }

        public IEnumerable<ServiceLabour> GetCharges(CostMatrixQuery query)
        {
            using (var scope = Context.Read())
            {
                var charges = from lcm in scope.Context.LabourCostMatrix
                              select lcm;

                return (charges.Select(lcm => new ServiceLabour
                              {
                                  ChargeContractedTech = lcm.ContractedTechnician,
                                  ChargeCustomer = lcm.CustomerCharge,
                                  ChargeEWClaim = lcm.EWClaim,
                                  ChargeInternalTech = lcm.InternalTechnician,
                                  ItemList = lcm.ItemList,
                                  Level_1 = lcm.Level_1,
                                  Level_2 = lcm.Level_2,
                                  Level_3 = lcm.Level_3,
                                  Supplier = lcm.Supplier,
                                  RepairType = lcm.Type,
                                  IsGroupFilter = lcm.IsGroupFilter,
                                  Id = lcm.Id,
                                  Label = lcm.Label
                              })).ToArray();
            }
        }

        private class EventType
        {
            public const string CreateMatrixEntry = "CreateMatrixEntry";
            public const string EditMatrixEntry = "EditMatrixEntry";
            public const string DeleteMatrixEntry = "DeleteMatrixEntry";
        }
    }
}
