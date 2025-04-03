namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using EntityFramework.BulkInsert.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRetailPriceRepository
    {
        List<RetailPriceViewModel> GetCurrentAndPendingForProduct(int id);

        List<RetailPriceViewModel> GetCurrentForProducts(List<int> ids);

        List<RetailPriceViewModel> GetForRepossessedProduct(int id);

        Dictionary<int, bool> ExistsForLocationEffectiveDate(int? locationId, string fascia, List<int> productIds, DateTime effectiveDate);

        List<Incoterm> GetIncoterms(int productId);

        List<Incoterm> SaveIncoterms(int productId, List<Incoterm> incoterms);

        void SaveIncoterms(List<IncotermStaging> incoterms);

        RetailPriceView GetForProductLocationEffectiveDate(
            int? locationId,
            string fascia,
            int productId,
            DateTime effectiveDate,
            List<RetailPriceView> prices = null,
            List<Location> locations = null);

        RetailPriceView Get(int id);

        RetailPriceView Get(int? locationId, string fascia, int productId, DateTime effectiveDate);

        bool IsDuplicate(RetailPriceViewModel retailPrice);

        void Delete(int id);

        void Delete(int? locationId, string fascia, int productId, DateTime effectiveDate);

        RetailPriceViewModel Save(RetailPriceViewModel retailPrice);

        Dictionary<int, List<RetailPriceViewModel>> GetForRepossessedProduct(List<int> ids);
    }

    public class RetailPriceRepository : IRetailPriceRepository
    {
        private readonly IEventStore audit;

        private readonly IProductStatusProgresser productStatusProgresser;

        public RetailPriceRepository(IEventStore audit, IProductStatusProgresser productStatusProgresser)
        {
            this.audit = audit;
            this.productStatusProgresser = productStatusProgresser;
        }

        public List<RetailPriceViewModel> GetCurrentAndPendingForProduct(int id)
        {
            using (var scope = Context.Read())
            {
                var prices = scope.Context.RetailPriceView.Where(p => p.ProductId == id).ToList();

                var currentPrices = Mapper.Map<List<CurrentRetailPriceView>, List<RetailPriceView>>(scope.Context.CurrentRetailPriceView.Where(p => p.ProductId == id).ToList());

                var futurePrices = prices.Where(p => p.EffectiveDate > DateTime.Now.Date).ToList();

                return Mapper.Map<List<RetailPriceViewModel>>(currentPrices.Union(futurePrices));
            }
        }

        public List<RetailPriceViewModel> GetCurrentForProducts(List<int> ids)
        {
            using (var scope = Context.Read())
            {
                var currentPrices = Mapper.Map<List<CurrentRetailPriceView>, List<RetailPriceView>>(scope.Context.CurrentRetailPriceView.Where(p => ids.Contains(p.ProductId)).ToList());

                return Mapper.Map<List<RetailPriceViewModel>>(currentPrices);
            }
        }

        public List<RetailPriceViewModel> GetForRepossessedProduct(int id)
        {
            using (var scope = Context.Read())
            {
                var prices = scope.Context.RepossessedPriceView.Where(p => p.ProductId == id).ToList();

                var currentPrices = Mapper.Map<List<CurrentRetailPriceView>, List<RepossessedPriceView>>(scope.Context.CurrentRetailPriceView.Where(p => p.ProductId == id).ToList());

                var futurePrices = prices.Where(p => p.EffectiveDate > DateTime.Now.Date).ToList();

                return Mapper.Map<List<RetailPriceViewModel>>(currentPrices.Union(futurePrices));
            }
        }

        public Dictionary<int, List<RetailPriceViewModel>> GetForRepossessedProduct(List<int> ids)
        {
            using (var scope = Context.Read())
            {
                var allPrices = scope.Context.RepossessedPriceView.Where(p => ids.Contains(p.ProductId)).ToList();
                var allCurrentPrices = Mapper.Map<List<CurrentRetailPriceView>, List<RepossessedPriceView>>(scope.Context.CurrentRetailPriceView.Where(p => ids.Contains(p.ProductId)).ToList());

                var result = new Dictionary<int, List<RetailPriceViewModel>>();

                foreach (var id in ids)
                {
                    var prices = allPrices.Where(p => p.Id == id).ToList();
                    var currentPrices = allCurrentPrices.Where(p => p.Id == id).ToList();
                    var futurePrices = prices.Where(p => p.EffectiveDate > DateTime.Now.Date).ToList();

                    result.Add(
                        id,
                        Mapper.Map<List<RetailPriceViewModel>>(currentPrices.Union(futurePrices)));
                }

                return result;
            }
        }

        public Dictionary<int, bool> ExistsForLocationEffectiveDate(int? locationId, string fascia, List<int> productIds, DateTime effectiveDate)
        {
            var result = new Dictionary<int, bool>();

            if (string.IsNullOrWhiteSpace(fascia))
            {
                fascia = null;
            }

            using (var scope = Context.Read())
            {
                var allProductPricesPrices = scope.Context.RetailPriceView.Where(rp => productIds.Contains(rp.ProductId)).ToList();
                foreach (var productId in productIds)
                {
                    if (string.IsNullOrWhiteSpace(fascia))
                    {
                        fascia = null;
                    }

                    var allPrices = allProductPricesPrices.Where(rp => rp.ProductId == productId);

                    if (locationId.HasValue)
                    {
                        allPrices = allPrices.Where(p => !p.LocationId.HasValue || p.LocationId.Value == locationId.Value);
                    }

                    if (fascia != null)
                    {
                        allPrices = allPrices.Where(p => p.Fascia == null || p.Fascia == fascia);
                    }

                    if (!result.ContainsKey(productId))
                    {
                        result.Add(productId, allPrices.Any(p => p.EffectiveDate <= effectiveDate));
                    }
                }
            }
            return result;
        }

        public List<Incoterm> GetIncoterms(int productId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Incoterm.Where(i => i.ProductId == productId).ToList();
            }
        }

        public List<Incoterm> SaveIncoterms(int productId, List<Incoterm> incoterms)
        {
            using (var scope = Context.Write())
            {
                var existing = scope.Context.Incoterm.Where(i => i.ProductId == productId);
                scope.Context.Incoterm.RemoveRange(existing);

                scope.Context.Incoterm.AddRange(incoterms);
                scope.Context.SaveChanges();
                scope.Complete();
                return incoterms;
            }
        }

        public void SaveIncoterms(List<IncotermStaging> incoterms)
        {
            using (var scope = Context.Write())
            {
                scope.Context.BulkInsert(incoterms);
                scope.Context.SaveChanges();
                scope.Context.BulkProductIncoSave();
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public RetailPriceView GetForProductLocationEffectiveDate(int? locationId, string fascia, int productId, DateTime effectiveDate, List<RetailPriceView> prices = null, List<Location> locations = null)
        {
            if (string.IsNullOrWhiteSpace(fascia))
            {
                fascia = null;
            }

            using (var scope = Context.Read())
            {
                List<RetailPriceView> allPrices = prices != null ? prices.Where(rp => rp.ProductId == productId).ToList() : scope.Context.RetailPriceView.Where(rp => rp.ProductId == productId).ToList();

                string locationFascia = null;

                if (locationId.HasValue)
                {
                    locationFascia = locations != null ? locations.FirstOrDefault(l => l.Id == locationId).Fascia : scope.Context.Location.FirstOrDefault(l => l.Id == locationId).Fascia;
                }

                var thisLocation = allPrices.Where(p => p.LocationId == locationId && locationId != null && p.EffectiveDate <= effectiveDate).OrderBy(p => p.EffectiveDate).ToList();

                if (thisLocation.Any())
                {
                    return thisLocation.Last();
                }

                var thisFascia = allPrices.Where(p => ((p.Fascia == fascia && fascia != null) || (p.Fascia == locationFascia && locationFascia != null)) && p.EffectiveDate <= effectiveDate).OrderBy(p => p.EffectiveDate).ToList();

                if (thisFascia.Any())
                {
                    return thisFascia.Last();
                }

                var allLocations = allPrices.Where(p => p.LocationId == null && p.Fascia == null && p.EffectiveDate <= effectiveDate).OrderBy(p => p.EffectiveDate).ToList();

                if (allLocations.Any())
                {
                    return allLocations.Last();
                }

                return null;
            }
        }

        public RetailPriceView Get(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.RetailPriceView.Single(l => l.Id == id);
            }
        }

        public RetailPriceView Get(int? locationId, string fascia, int productId, DateTime effectiveDate)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.RetailPriceView.Single(r => r.LocationId == locationId &&
                    r.Fascia == fascia &&
                    r.ProductId == productId &&
                    r.EffectiveDate == effectiveDate);
            }
        }

        public bool IsDuplicate(RetailPriceViewModel retailPrice)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.RetailPrice.Any(p =>
                    p.Id != retailPrice.Id
                    && ((retailPrice.Fascia == null && p.Fascia == null) || (retailPrice.Fascia == p.Fascia))
                    && ((retailPrice.LocationId == null && p.LocationId == null) || (retailPrice.LocationId == p.LocationId))
                    && p.ProductId == retailPrice.ProductId
                    && p.EffectiveDate == retailPrice.EffectiveDate.Date);
            }
        }

        public void Delete(int id)
        {
            using (var scope = Context.Write())
            {
                var price = scope.Context.RetailPrice.Single(r => r.Id == id);
                var sku = scope.Context.Product.Find(price.ProductId);

                if (price.EffectiveDate.Date <= DateTime.Now.Date)
                {
                    throw new InvalidOperationException("Cannot delete a price which has come into effect");
                }
                scope.Context.RetailPrice.Remove(price);
                scope.Context.SaveChanges();
                this.audit.LogAsync(new { sku.SKU, price }, RetailPriceEvents.Delete, EventCategories.Merchandising);
                scope.Complete();
            }
        }

        public void Delete(int? locationId, string fascia, int productId, DateTime effectiveDate)
        {
            using (var scope = Context.Read())
            {
                var price = scope.Context.RetailPrice.Single(r => r.LocationId == locationId &&
                    r.Fascia == fascia &&
                    r.ProductId == productId &&
                    r.EffectiveDate == effectiveDate);
                Delete(price.Id);
            }
        }

        public RetailPriceViewModel Save(RetailPriceViewModel retailPrice)
        {
            using (var scope = Context.Write())
            {
                var price = scope.Context.RetailPrice.Find(retailPrice.Id);
                var sku = scope.Context.Product.Find(retailPrice.ProductId);

                string eventType;

                if (price == null)
                {
                    eventType = RetailPriceEvents.Create;
                    price = new RetailPrice();
                    scope.Context.RetailPrice.Add(price);
                }
                else
                {
                    eventType = RetailPriceEvents.Edit;
                }

                price = Mapper.Map(retailPrice, price);

                price.LastUpdated = DateTime.UtcNow;

                scope.Context.SaveChanges();
                var product = scope.Context.Product.Single(p => p.Id == retailPrice.ProductId);
                productStatusProgresser.AutoProgress(product);
                scope.Context.SaveChanges();

                this.audit.LogAsync(new { sku.SKU, Price = Mapper.Map<RetailPriceAuditModel>(retailPrice) }, eventType, EventCategories.Merchandising);
                scope.Complete();

                return Mapper.Map<RetailPriceViewModel>(price);
            }
        }
    }
}
