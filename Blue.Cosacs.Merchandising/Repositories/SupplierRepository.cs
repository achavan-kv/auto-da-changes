namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Event;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public interface ISupplierRepository
    {
        List<SupplierModel> Get(bool includeInactive = false);

        Dictionary<int, string> GetList(bool includeInactive = false);

        SupplierModel Get(int id, bool includeInactive = false);

        SupplierModel Save(SupplierModel model);

        SupplierModel Save(SupplierImportModel model);

        bool Exists(string code);

        bool CanUseNameWithCode(string name, string code);

        Dictionary<int, List<int>> GetSuppliers(List<int> queryProductIds);

        Supplier LocateResource(string code);

        void SaveSupplierTags(int id, List<string> tags);
    }

    public class SupplierRepository : ISupplierRepository
    {
        private readonly IEventStore audit;

        public SupplierRepository(IEventStore audit)
        {
            this.audit = audit;
        }

        public List<SupplierModel> Get(bool includeInactive = false)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.Supplier.AsQueryable();
                if (!includeInactive)
                {
                    var stats = scope.Context.SupplierStatus.Select(s => s.Id);
                    query = query.Where(q => stats.Contains(q.Status));
                }
                return query.ToList().Select(Mapper.Map<SupplierModel>).ToList();
            }
        }

        public Dictionary<int, string> GetList(bool includeInactive = false)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.Supplier.AsQueryable();
                if (!includeInactive)
                {
                    query = query.Where(v => v.Status != (int)SupplierStatusEnum.Inactive);
                }
                var supplier = query.ToList();
                return supplier.ToDictionary(l => l.Id, l => l.Name);
            }
        }

        public SupplierModel Get(int id, bool includeInactive = false)
        {
            using (var scope = Context.Read())
            {
                var query = scope.Context.Supplier.AsQueryable();
                if (!includeInactive)
                {
                    var stats = scope.Context.SupplierStatus.Select(s => s.Id);
                    query = query.Where(q => stats.Contains(q.Status));
                }
                var supplier = query.Single(l => l.Id == id);
                return Mapper.Map<SupplierModel>(supplier);
            }
        }

        public SupplierModel Save(SupplierModel model)
        {
            using (var scope = Context.Write())
            {
                var supplier = scope.Context.Supplier.Find(model.Id);
                
                string eventType;

                if (supplier == null)
                {
                    supplier = new Supplier();
                    scope.Context.Supplier.Add(supplier);
                    eventType = SupplierEvents.CreateVendor;
                }
                else
                {
                    eventType = SupplierEvents.EditVendor;
                }

                Mapper.Map(model, supplier);
                scope.Context.SaveChanges();
                audit.LogAsync(supplier, eventType, EventCategories.Merchandising);
                scope.Complete();
                return Mapper.Map<SupplierModel>(supplier);
            }
        }

        public SupplierModel Save(SupplierImportModel model)
        {
            using (var scope = Context.Write())
            {
                var supplier = scope.Context.Supplier.SingleOrDefault(s => s.Code == model.Code);

                string eventType;

                if (supplier == null)
                {
                    supplier = new Supplier();
                    scope.Context.Supplier.Add(supplier);
                    eventType = SupplierEvents.CreateVendor;
                }
                else
                {
                    eventType = SupplierEvents.EditVendor;
                }

                Mapper.Map(model, supplier);
                scope.Context.SaveChanges();

                audit.LogAsync(supplier, eventType, EventCategories.Merchandising);
                scope.Complete();
                return Mapper.Map<SupplierModel>(supplier);
            }
        }

        public bool Exists(string code)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Supplier.Any(l => l.Code == code);
            }
        }

        public bool CanUseNameWithCode(string name, string code)
        {
            using (var scope = Context.Read())
            {
                return !scope.Context.Supplier.Any(l => l.Code != code && l.Name == name);
            }
        }

        public void SaveSupplierTags(int id, List<string> tags)
        {
            using (var scope = Context.Write())
            {
                var supplier = scope.Context.Supplier.Find(id);

                if (supplier == null)
                {
                    throw new ArgumentOutOfRangeException();
                }

                supplier.Tags = JsonConvertHelper.Serialize(tags);

                scope.Context.SaveChanges();
                
                this.audit.LogAsync(new { supplier.Name, supplier.Tags }, SupplierEvents.EditVendorTags, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public Dictionary<int, List<int>> GetSuppliers(List<int> queryProductIds)
        {
            using (var scope = Context.Read())
            {
                var suppliers = scope.Context.ProductSupplier
                    .Where(s => queryProductIds.Contains(s.ProductId))
                    .Select(p => new { p.ProductId, p.SupplierId })
                    .Union(scope.Context.Product.Where(p => 
                        queryProductIds.Contains(p.Id) && p.PrimaryVendorId.HasValue)
                            .Select(p => new { ProductId = p.Id, SupplierId = p.PrimaryVendorId.Value }))
                    .GroupBy(s => s.ProductId);

                return suppliers.ToDictionary(g => g.Key, g => g.Select(s => s.SupplierId).ToList());
            }
        }

        public Supplier LocateResource(string code)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Supplier.FirstOrDefault(s => s.Code == code);
            }
        }
    }
}
