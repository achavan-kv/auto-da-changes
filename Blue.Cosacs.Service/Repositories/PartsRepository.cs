using System.Collections.Generic;
using System.Linq;
using Blue.Cosacs.Service.Models;
using Blue.Events;
using System;

namespace Blue.Cosacs.Service.Repositories
{
    public class PartsRepository
    {
        public class ServicePartsMatrixException : Exception
        {
            public ServicePartsMatrixException(string message)
                : base(message)
            {
            }
        }

        private readonly IEventStore audit;
        private const string AuditEventCategory = "PartsCostMatrix";

        public PartsRepository(IEventStore audit)
        {
            this.audit = audit;
        }

        public int Save(ServicePartsMatrix serviceParts)
        {
            var id = 0;
            PartsCostMatrix original = null;

            if (!ValidateServicePartsMatrix(serviceParts))
            {
                throw new ServiceLabourException(string.Format("There is already a Parts Cost Matrix with the label '{0}'", serviceParts.Label));
            }

            using (var scope = Context.Write())
            {
                if (serviceParts.Id > 0)
                {
                    original = scope.Context.PartsCostMatrix.Find(serviceParts.Id);
                    if (original != null)
                        scope.Context.PartsCostMatrix.Remove(original);
                }

                var newCost = new PartsCostMatrix
                {
                    Internal = serviceParts.ChargeInternal,
                    Customer = serviceParts.ChargeCustomer,
                    ExtendedWarranty = serviceParts.ChargeExtendedWarranty,
                    FirstYearWarranty = serviceParts.ChargeFirstYearWarranty,
                    ItemList = serviceParts.ItemList,
                    Level_1 = serviceParts.Level_1,
                    Level_2 = serviceParts.Level_2,
                    Level_3 = serviceParts.Level_3,
                    Supplier = serviceParts.Supplier,
                    Type = serviceParts.RepairType,
                    IsGroupFilter = serviceParts.IsGroupFilter,
                    Label = serviceParts.Label

                };
                scope.Context.PartsCostMatrix.Add(newCost);
                scope.Context.SaveChanges();
                id = newCost.Id;
                scope.Complete();
            };

            if (original != null)
            {
                audit.LogAsync(new { Original = original, Modified = serviceParts }, EventType.EditMatrixEntry, AuditEventCategory);
            }
            else
            {
                audit.LogAsync(serviceParts, EventType.CreateMatrixEntry, AuditEventCategory);
            }

            return id;
        }

        private bool ValidateServicePartsMatrix(ServicePartsMatrix value)
        {
            using (var scope = Context.Read())
            {
                var id = value.Id.HasValue ? value.Id.Value : 0;
                return scope.Context.PartsCostMatrix.Count(p => p.Label == value.Label && p.Id != id) == 0;
            }
        }

        public ServicePartsMatrix[] GetAll()
        {
            using (var scope = Context.Read())
            {
                return (from pcm in scope.Context.PartsCostMatrix
                        orderby pcm.Id
                        select new ServicePartsMatrix
                       {
                           ChargeInternal = pcm.Internal,
                           ChargeCustomer = pcm.Customer,
                           ChargeExtendedWarranty = pcm.ExtendedWarranty,
                           ChargeFirstYearWarranty = pcm.FirstYearWarranty,
                           ItemList = pcm.ItemList,
                           Level_1 = pcm.Level_1,
                           Level_2 = pcm.Level_2,
                           Level_3 = pcm.Level_3,
                           Supplier = pcm.Supplier,
                           RepairType = pcm.Type,
                           IsGroupFilter = pcm.IsGroupFilter,
                           Id = pcm.Id,
                           Label = pcm.Label
                       }).ToArray();
            }
        }

        public ServicePartsMatrix Get(int id)
        {
            using (var scope = Context.Read())
            {
                return (from pcm in scope.Context.PartsCostMatrix
                        where pcm.Id == id
                        select new ServicePartsMatrix
                        {
                            ChargeInternal = pcm.Internal,
                            ChargeCustomer = pcm.Customer,
                            ChargeExtendedWarranty = pcm.ExtendedWarranty,
                            ChargeFirstYearWarranty = pcm.FirstYearWarranty,
                            ItemList = pcm.ItemList,
                            Level_1 = pcm.Level_1,
                            Level_2 = pcm.Level_2,
                            Level_3 = pcm.Level_3,
                            Supplier = pcm.Supplier,
                            RepairType = pcm.Type,
                            IsGroupFilter = pcm.IsGroupFilter,
                            Id = pcm.Id,
                            Label = pcm.Label
                        }).First();
                ;
            }
        }

        public void Delete(int id)
        {
            PartsCostMatrix pcm = null;
            using (var scope = Context.Write())
            {
                pcm = scope.Context.PartsCostMatrix.Find(id);
                if (pcm != null)
                {
                    scope.Context.PartsCostMatrix.Remove(pcm);
                    scope.Context.SaveChanges();
                    scope.Complete();
                }
            }

            audit.LogAsync(pcm, EventType.DeleteMatrixEntry, AuditEventCategory);
        }

        public IEnumerable<ServicePartsMatrix> GetCharges(CostMatrixQuery query)
        {
            using (var scope = Context.Read())
            {
                var filtered = from pcm in scope.Context.PartsCostMatrix
                               select pcm;
                filtered = FilterCollection(query, filtered);
                return (from pcm in filtered
                        select new ServicePartsMatrix
                        {
                            ChargeInternal = pcm.Internal,
                            ChargeCustomer = pcm.Customer,
                            ChargeExtendedWarranty = pcm.ExtendedWarranty,
                            ChargeFirstYearWarranty = pcm.FirstYearWarranty,
                            ItemList = pcm.ItemList,
                            Level_1 = pcm.Level_1,
                            Level_2 = pcm.Level_2,
                            Level_3 = pcm.Level_3,
                            Supplier = pcm.Supplier,
                            RepairType = pcm.Type,
                            IsGroupFilter = pcm.IsGroupFilter,
                            Id = pcm.Id,
                            Label = pcm.Label
                        }).ToArray();
            }
        }

        private static IQueryable<PartsCostMatrix> FilterCollection(CostMatrixQuery query, IQueryable<PartsCostMatrix> filtered)
        {
            var groupQuery = false;
            if (!string.IsNullOrWhiteSpace(query.ProductLevel_1))
            {
                filtered = filtered.Where(c => c.IsGroupFilter && (c.Level_1 == null || c.Level_1 == query.ProductLevel_1));
                groupQuery = true;
            }
            if (!string.IsNullOrWhiteSpace(query.ProductLevel_2))
            {
                filtered = filtered.Where(c => c.IsGroupFilter && (c.Level_2 == null || c.Level_2 == query.ProductLevel_2));
                groupQuery = true;
            }
            if (!string.IsNullOrWhiteSpace(query.ProductLevel_3))
            {
                filtered = filtered.Where(c => c.IsGroupFilter && (c.Level_3 == null || c.Level_3 == query.ProductLevel_3));
                groupQuery = true;
            }
            if (!string.IsNullOrWhiteSpace(query.Manufacturer))
            {
                filtered = filtered.Where(c => c.IsGroupFilter && (c.Supplier == null || c.Supplier == query.Manufacturer));
                groupQuery = true;
            }

            if (!groupQuery && !string.IsNullOrWhiteSpace(query.ItemNumber))
            {
                filtered = filtered.Where(c => c.IsGroupFilter == false && c.ItemList.Contains(query.ItemNumber));
            }

            return filtered;
        }

        private class EventType
        {
            public const string CreateMatrixEntry = "CreateMatrixEntry";
            public const string EditMatrixEntry = "EditMatrixEntry";
            public const string DeleteMatrixEntry = "DeleteMatrixEntry";
        }
    }
}
