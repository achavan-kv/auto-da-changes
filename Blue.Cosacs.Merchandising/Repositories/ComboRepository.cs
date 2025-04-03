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

    public interface IComboRepository
    {
        ComboModel Get(int id);

        ComboModel Save(ComboModel model, int userId);

        ComboModel SaveComponent(int comboId, int comboProductId, string sku, int quantity, int userId);

        ComboModel RemoveComponent(int comboId, string sku, int userId);

        ComboModel SaveLocation(int comboId, string fascia, int? locationId, ComboProductPriceModel locationPrices, int userId);

        ComboModel RemoveLocation(int comboId, string fascia, int? locationId, int userId);
    }

    public class ComboRepository : IComboRepository
    {
        private readonly IEventStore audit;
        private readonly IProductRepository productRepository;
        private readonly ITaxRepository taxRepository;
        private readonly ISettings settings;

        private readonly IBrandRepository brandRepository;

        private readonly ISupplierRepository supplierRepository;

        private readonly ILocationRepository locationRepository;

        public ComboRepository(IEventStore audit, IProductRepository productRepository, ITaxRepository taxRepository, ISettings settings, IBrandRepository brandRepository, ISupplierRepository supplierRepository, ILocationRepository locationRepository)
        {
            this.audit = audit;
            this.productRepository = productRepository;
            this.taxRepository = taxRepository;
            this.settings = settings;
            this.brandRepository = brandRepository;
            this.supplierRepository = supplierRepository;
            this.locationRepository = locationRepository;
        }

        public ComboModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var combo = scope.Context
                    .ComboView
                    .ToList()
                    .Select(Mapper.Map<ComboModel>)
                    .Single(s => s.Id == id);

                var today = DateTime.Now.Date;
                var startDay = combo.StartDate.HasValue ? combo.StartDate.Value.Date : (DateTime?)null;

                var prodGroups = scope.Context.ComboProductView
                    .Where(c => c.ComboId == id && (c.PriceEffectiveDate <= ((combo.StartDate.HasValue && startDay > today) ? startDay : today) || c.PriceEffectiveDate == null))
                    .GroupBy(p => new { p.ProductId, p.LocationId, p.Fascia }).ToList();

                combo.Components = (from @group in prodGroups
                                    let maxDate = @group.Max(g => g.PriceEffectiveDate)
                                    select @group.FirstOrDefault(g => g.PriceEffectiveDate == maxDate))
                                    .ToList();

                var priceGroups =
                    scope.Context.ComboPriceView.Where(c => c.ComboId == id)
                        .ToList()
                        .GroupBy(c => new { c.Fascia, c.LocationId, c.LocationName });

                combo.ComboPrices =
                    priceGroups.Select(
                        pg =>
                        new ComboPriceLocationModel()
                            {
                                Fascia = pg.Key.Fascia,
                                LocationId = pg.Key.LocationId,
                                LocationName = pg.Key.LocationName,
                                Prices = pg.ToList()
                            }).OrderBy(cp => cp.LocationName);

                combo.Hierarchy = scope.Context.ProductHierarchyView.Where(h => h.ProductId == id)
                        .ToDictionary(h => h.Level, h => h.Tag);

                return combo;
            }
        }

        public ComboModel Save(ComboModel model, int userId)
        {
            using (var scope = Context.Write())
            {
                var prod = scope.Context.Product.Find(model.Id);
                if (prod == null)
                {
                    prod = Mapper.Map<Merchandising.Product>(model);
                    prod.ProductType = ProductTypes.Combo;
                    Mapper.Map(productRepository.Save(prod, userId), model);
                }
                else
                {
                    Mapper.Map(model, prod);
                    productRepository.Save(prod, userId);
                }

                // Save combo specifics
                var combo = scope.Context.Combo.Find(model.Id);
                if (combo == null)
                {
                    combo = new Combo();
                    Mapper.Map(model, combo);
                    scope.Context.Combo.Add(combo);
                }
                else
                {
                    Mapper.Map(model, combo);
                }
                
                scope.Context.SaveChanges();
                scope.Complete();
            }

            return model;
        }

        public ComboModel SaveComponent(int comboId, int comboProductId, string sku, int quantity, int userId)
        {
            var components = new List<Merchandising.Product>();
            using (var scope = Context.Write())
            {
                var combo = comboId == 0 ? Save(new ComboModel(), userId) : Get(comboId);
                if (combo != null)
                {
                    var component = scope.Context.Product.FirstOrDefault(c => c.SKU == sku);
                    if (component != null)
                    {
                        var newComboProduct = scope.Context.ComboProduct.FirstOrDefault(s => s.Id == comboProductId);
                        if (newComboProduct == null)
                        {
                            newComboProduct = new ComboProduct() { ProductId = component.Id, Quantity = quantity, ComboId = combo.Id };
                            scope.Context.ComboProduct.Add(newComboProduct);
                        }
                        else
                        {
                            newComboProduct.Quantity = quantity;
                            newComboProduct.ProductId = component.Id;
                        }

                        scope.Context.SaveChanges();
                        Save(combo, userId);
                        components.Add(component);
                    }
                }

                audit.LogAsync(components.Select(c => this.GetAuditComponent(c)), ComboEvents.AddComponent, EventCategories.Merchandising);
                scope.Complete();
            }
            return Get(comboId);
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

        public ComboModel RemoveComponent(int comboId, string sku, int userId)
        {
            var components = new List<Merchandising.Product>();
            using (var scope = Context.Write())
            {
                var combo = comboId == 0 ? Save(new ComboModel(), userId) : Get(comboId);
                if (combo != null)
                {
                    var component = scope.Context.Product.FirstOrDefault(c => c.SKU == sku);
                    if (component != null)
                    {
                        var prod =
                            scope.Context.ComboProduct.FirstOrDefault(
                                c => c.ComboId == combo.Id && c.ProductId == component.Id);

                        if (prod != null)
                        {
                            // Delete prices for this combo product
                            var prices = scope.Context.ComboProductPrice.Where(
                                c => c.ComboProductId == prod.Id);

                            scope.Context.ComboProductPrice.RemoveRange(prices);
                            scope.Context.SaveChanges();

                            scope.Context.ComboProduct.Remove(prod);
                            scope.Context.SaveChanges();

                            Save(combo,userId);
                            components.Add(component);
                        }
                    }
                }

                audit.LogAsync(components.Select(s => this.GetAuditComponent(s)), ComboEvents.RemoveComponent, EventCategories.Merchandising);
                scope.Complete();
            }
            return Get(comboId);
        }

        public ComboModel SaveLocation(int comboId, string fascia, int? locationId, ComboProductPriceModel locationPrices, int userId)
        {
            ComboModel combo;

            using (var scope = Context.Write())
            {
                var comboProducts = scope.Context.ComboProduct.Where(p => p.ComboId == comboId).ToList();
                var comboProductIds = comboProducts.Select(c => c.Id).ToList();

                // Remove existing prices in this location
                var existingPrices =
                    scope.Context.ComboProductPrice.Where(
                        p =>
                        comboProductIds.Contains(p.ComboProductId)
                        && (((string.IsNullOrEmpty(fascia) && string.IsNullOrEmpty(p.Fascia)) || p.Fascia == fascia)
                            && ((locationId == null && p.LocationId == null) || p.LocationId == locationId)));
                scope.Context.ComboProductPrice.RemoveRange(existingPrices);

                comboProducts.ForEach(
                    c =>
                        {
                        var regPrice = locationPrices.RegularPrice.FirstOrDefault(lp => lp.ProductId == c.ProductId) ?? new ComboProductPriceItemModel();

                        var cashPrice = locationPrices.CashPrice.FirstOrDefault(lp => lp.ProductId == c.ProductId) ?? new ComboProductPriceItemModel();

                        var dutyPrice = locationPrices.DutyFreePrice.FirstOrDefault(lp => lp.ProductId == c.ProductId) ?? new ComboProductPriceItemModel();
                                           
                            scope.Context.ComboProductPrice.Add(
                                new ComboProductPrice()
                                    {
                                        Fascia = fascia,
                                        LocationId = locationId,
                                        ComboProductId = c.Id,
                                        RegularPrice = regPrice.Price,
                                        CashPrice = cashPrice.Price,
                                        DutyFreePrice = dutyPrice.Price,
                                    });
                        });

                scope.Context.SaveChanges();

                audit.LogAsync(
                    new 
                    { 
                        ComboId = comboId,
                        Fascia = string.IsNullOrWhiteSpace(fascia) ? "All" : string.Empty,
                        Location = locationId.HasValue ? locationRepository.Get(locationId.Value).Name : "All"
                    }, 
                    ComboEvents.AddLocation, 
                    EventCategories.Merchandising);

                combo = Get(comboId);
            Save(combo, userId);
                scope.Complete();
            }
            return combo;
        }

        public ComboModel RemoveLocation(int comboId, string fascia, int? locationId, int userId)
        {
            ComboModel combo;
            using (var scope = Context.Write())
            {
                var comboPrices = scope.Context.ComboPriceView.Where(p => p.ComboId == comboId && (((string.IsNullOrEmpty(fascia) && string.IsNullOrEmpty(p.Fascia)) || p.Fascia == fascia) && ((locationId == null && p.LocationId == null) || p.LocationId == locationId))).ToList();
                var comboPriceIds = comboPrices.Select(c => c.Id).ToList();

                // Remove existing prices in this location
                var existingPrices = scope.Context.ComboProductPrice.Where(p => comboPriceIds.Contains(p.Id)).ToList();
                scope.Context.ComboProductPrice.RemoveRange(existingPrices);

                scope.Context.SaveChanges();

                audit.LogAsync(
                    new 
                    { 
                        ComboId = comboId,
                        Fascia = string.IsNullOrWhiteSpace(fascia) ? "All" : string.Empty,
                        Location = locationId.HasValue ? locationRepository.Get(locationId.Value).Name : "All"
                    }, 
                    SetEvents.RemoveLocation, 
                    EventCategories.Merchandising);

                combo = Get(comboId);
            Save(combo, userId);
                scope.Complete();
            }
            return combo;
        }
    }
}
