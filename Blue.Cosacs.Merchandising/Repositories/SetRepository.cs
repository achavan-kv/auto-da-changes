namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Config.Repositories;
    using Blue.Cosacs.Merchandising.Enums;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISetRepository
    {
        SetModel Get(int id);

        SetModel Save(SetModel model, int userId);

        SetModel SaveComponent(int setId, int id, string sku, int quantity, int userId);

        SetModel RemoveComponent(int setId, string sku, int userId);

        SetModel SaveLocation(int setId, DateTime effectiveDate, string fascia, int? locationId, decimal? regularPrice, decimal? dutyFreePrice, decimal? cashPrice, int userId);

        SetModel RemoveLocation(int setId, DateTime effectiveDate, string fascia, int? locationId, int userId);

        Merchandising.Product LookupSetProduct(string sku);
    }

    public class SetRepository : ISetRepository
    {
        private readonly IEventStore audit;
        private readonly IProductRepository productRepository;

        private readonly ISettings settings;

        private readonly ILocationRepository locationRepository;

        private readonly IBrandRepository brandRepository;

        private readonly ISupplierRepository supplierRepository;

        private readonly IProductStatusProgresser productStatusProgresser;

        public SetRepository(IEventStore audit, IProductRepository productRepository, ISettings settings, ILocationRepository locationRepository, IBrandRepository brandRepository, ISupplierRepository supplierRepository, IProductStatusProgresser productStatusProgresser)
        {
            this.audit = audit;
            this.productRepository = productRepository;
            this.settings = settings;
            this.locationRepository = locationRepository;
            this.brandRepository = brandRepository;
            this.supplierRepository = supplierRepository;
            this.productStatusProgresser = productStatusProgresser;
        }

        public SetModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var setModel = Mapper.Map<SetModel>(scope.Context.Product.FirstOrDefault(s => s.Id == id));
                setModel.Locations = GetCurrentAndPendingLocations(id);
                setModel.Components = Enumerable.Distinct(scope.Context.SetProductView.Where(c => c.SetId == id)).ToList();

                setModel.Hierarchy =
                        scope.Context.ProductHierarchyView.Where(h => h.ProductId == id)
                            .ToDictionary(h => h.Level, h => h.Tag);

                return setModel;
            }
        }

        private List<SetLocationView> GetCurrentAndPendingLocations(int id)
        {
            using (var scope = Context.Read())
            {
                var prices = scope.Context.SetLocationView.Where(s => s.SetId == id).ToList();

                var currentPrices = Mapper.Map<List<CurrentRetailPriceView>, List<SetLocationView>>(scope.Context.CurrentRetailPriceView.Where(p => p.ProductId == id).ToList());

                var futurePrices = prices.Where(p => p.EffectiveDate > DateTime.Now.Date).ToList();

                return currentPrices.Union(futurePrices).ToList();
            }
        }

        public SetModel Save(SetModel model, int userId)
        {
            using (var scope = Context.Write())
            {
                var setProduct = scope.Context.Product.Find(model.Id);
                if (setProduct == null)
                {
                    setProduct = Mapper.Map<Merchandising.Product>(model);
                    setProduct.ProductType = ProductTypes.Set;
                    Mapper.Map(productRepository.Save(setProduct, userId), model);
                }
                else
                {
                    Mapper.Map(model, setProduct);
                }

                productStatusProgresser.AutoProgress(setProduct);

                scope.Context.SaveChanges();
                scope.Complete();
                return model;
            }
        }

        private object GetAuditComponent(Merchandising.Product component)
        {
            return
                new
                {
                    component.Id,
                    component.SKU,
                    component.LongDescription,
                    component.ProductType,
                    component.Tags,
                    component.StoreTypes,
                    component.POSDescription,
                    component.Attributes,
                    component.CreatedDate,
                    component.LastUpdatedDate,
                    Status = ((ProductStatuses)component.Status).ToString(),
                    component.PriceTicket,
                    component.SKUStatus,
                    component.CorporateUPC,
                    component.VendorUPC,
                    component.VendorStyleLong,
                    component.CountryOfOrigin,
                    component.VendorWarranty,
                    component.ReplacingTo,
                    component.Features,
                    BrandName = component.BrandId.HasValue ? brandRepository.Get(component.BrandId.Value).BrandName : string.Empty,
                    PrimaryVendorName = component.PrimaryVendorId.HasValue ? supplierRepository.Get(component.PrimaryVendorId.Value).Name : string.Empty,
                    component.LastStatusChangeDate,
                    component.OnlineDateAdded
                };
        }

        public SetModel SaveComponent(int setId, int id, string sku, int quantity, int userId)
        {
            using (var scope = Context.Write())
            {
                var set = setId == 0 ? Save(new SetModel(), userId) : Get(setId);
                if (set != null)
                {
                    var component = scope.Context.Product.FirstOrDefault(c => c.SKU == sku);
                    if (component != null)
                    {
                        var newSetProduct = scope.Context.SetProduct.FirstOrDefault(s => s.SetId == setId && s.Id == id);

                        if (newSetProduct == null)
                        {
                            newSetProduct = new SetProduct()
                            {
                                ProductId = component.Id,
                                Quantity = quantity,
                                SetId = set.Id
                            };
                            scope.Context.SetProduct.Add(newSetProduct);
                        }
                        else
                        {
                            newSetProduct.Quantity = quantity;
                            newSetProduct.ProductId = component.Id;
                        }

                        scope.Context.SaveChanges();
                        Save(set, userId);
                        audit.LogAsync(this.GetAuditComponent(component), SetEvents.AddComponent, EventCategories.Merchandising);
                    }
                }
                scope.Complete();
            }

            return Get(setId);
        }

        public SetModel RemoveComponent(int setId, string sku, int userId)
        {
            SetModel setModel;
            using (var scope = Context.Write())
            {
                var set = setId == 0 ? Save(new SetModel(), userId) : Get(setId);
                if (set != null)
                {
                    var component = scope.Context.Product.FirstOrDefault(c => c.SKU == sku);
                    if (component != null)
                    {
                        var setProd =
                            scope.Context.SetProduct.FirstOrDefault(
                                sp => sp.SetId == set.Id && sp.ProductId == component.Id);
                        if (setProd != null)
                        {
                            scope.Context.SetProduct.Remove(setProd);
                            scope.Context.SaveChanges();
                            Save(set, userId);
                            audit.LogAsync(GetAuditComponent(component), SetEvents.RemoveComponent, EventCategories.Merchandising);
                        }
                    }
                }
                setModel = Get(setId);
                scope.Complete();
            }
            return setModel;
        }

        public SetModel SaveLocation(int setId, DateTime effectiveDate, string fascia, int? locationId, decimal? regularPrice, decimal? dutyFreePrice, decimal? cashPrice, int userId)
        {
            SetModel set;
            using (var scope = Context.Write())
            {
                var loc = scope.Context.SetLocation.FirstOrDefault(p => p.SetId == setId && p.EffectiveDate == effectiveDate && (((string.IsNullOrEmpty(fascia) && string.IsNullOrEmpty(p.Fascia)) || p.Fascia == fascia) && ((locationId == null && p.LocationId == null) || p.LocationId == locationId)));
                if (loc == null)
                {
                    loc = new SetLocation() { SetId = setId, LocationId = locationId, Fascia = fascia, EffectiveDate = effectiveDate };
                    scope.Context.SetLocation.Add(loc);
                }
                loc.CashPrice = cashPrice;
                loc.RegularPrice = regularPrice;
                loc.DutyFreePrice = dutyFreePrice;

                scope.Context.SaveChanges();
                audit.LogAsync(
                    new
                    {
                        SetId = setId,
                        EffectiveDate = effectiveDate,
                        Fascia = string.IsNullOrWhiteSpace(fascia) ? "All" : string.Empty,
                        Location = locationId.HasValue ? locationRepository.Get(locationId.Value).Name : "All"
                    },
                    SetEvents.AddLocation,
                    EventCategories.Merchandising);
                set = Get(setId);
                Save(set, userId);

                scope.Complete();
            }

            return set;
        }

        public SetModel RemoveLocation(int setId, DateTime effectiveDate, string fascia, int? locationId, int userId)
        {
            SetModel setModel;
            using (var scope = Context.Write())
            {
                var set = Get(setId);

                var loc = scope.Context.SetLocation.FirstOrDefault(p => p.SetId == setId && p.EffectiveDate == effectiveDate && (((string.IsNullOrEmpty(fascia) && string.IsNullOrEmpty(p.Fascia)) || p.Fascia == fascia) && ((locationId == null && p.LocationId == null) || p.LocationId == locationId)));
                if (loc != null)
                {
                    scope.Context.SetLocation.Remove(loc);
                }
                else
                {
                    throw new Exception("Location Not Found");
                }

                scope.Context.SaveChanges();
                Save(set, userId);

                audit.LogAsync(
                    new
                    {
                        SetId = setId,
                        Fascia = string.IsNullOrWhiteSpace(fascia) ? "All" : string.Empty,
                        Location = locationId.HasValue ? locationRepository.Get(locationId.Value).Name : "All"
                    },
                    SetEvents.RemoveLocation,
                    EventCategories.Merchandising);
                setModel = Get(setId);
                scope.Complete();
            }
            return setModel;
        }

        public Merchandising.Product LookupSetProduct(string sku)
        {
            using (var scope = Context.Read())
            {
                return scope.Context
                    .Product
                    .Where(p => p.ProductType != ProductTypes.Combo)
                    .Where(p => p.ProductType != ProductTypes.Set)
                    .SingleOrDefault(p => p.SKU == sku);
            }
        }
    }
}
