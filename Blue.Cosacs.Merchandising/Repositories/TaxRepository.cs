namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ITaxRepository
    {
        IEnumerable<TaxRateModel> Get();

        IEnumerable<TaxRateModel> GetAll();

        IEnumerable<TaxRateModel> GetAllSystem();

        TaxRateModel Get(int id);

        IEnumerable<TaxRateModel> GetByProduct(int id);

        TaxRateModel GetCurrent();

        TaxRateModel GetActive(DateTime date);

        TaxRateModel GetCurrentByProduct(int id);

        void Delete(int id);

        TaxRateModel Save(TaxRateModel model);
    }

    public class TaxRepository : ITaxRepository
    {
        private readonly IEventStore audit;

        public TaxRepository(IEventStore audit)
        {
            this.audit = audit;
        }

        public IEnumerable<TaxRateModel> Get()
        {
            using (var scope = Context.Read())
            {
                return LimitByDate(scope.Context.TaxRate.Where(r => r.ProductId == null).ToList());
            }
        }

        public IEnumerable<TaxRateModel> GetAll()
        {
            using (var scope = Context.Read())
            {
                var rates = scope.Context.TaxRate.ToList();
                return rates.Any() ? Mapper.Map<IEnumerable<TaxRateModel>>(rates) : new List<TaxRateModel>();
            }
        }

        public IEnumerable<TaxRateModel> GetAllSystem()
        {
            using (var scope = Context.Read())
            {
                var rates = scope.Context.TaxRate.Where(t => !t.ProductId.HasValue).ToList();
                return rates.Any() ? Mapper.Map<IEnumerable<TaxRateModel>>(rates) : new List<TaxRateModel>();
            }
        }

        public TaxRateModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var rate = scope.Context.TaxRate.SingleOrDefault(l => l.Id == id);
                return rate != null ? Mapper.Map<TaxRateModel>(rate) : null;
            }
        }

        public IEnumerable<TaxRateModel> GetByProduct(int id)
        {
            using (var scope = Context.Read())
            {
                return LimitByDate(scope.Context.TaxRate.Where(l => l.ProductId == id).ToList());
            }
        }

        public TaxRateModel GetActive(DateTime date)
        {
            var rates = Get();
            var current = rates.Where(t => t.EffectiveDate.Date <= date.Date)
                .OrderByDescending(t => t.EffectiveDate)
                .FirstOrDefault();

            return current;
        }

        public TaxRateModel GetCurrent()
        {
            return GetActive(DateTime.Now);
        }

        public TaxRateModel GetCurrentByProduct(int id)
        {
            var rates = GetByProduct(id);
            var current = rates.Where(t => t.EffectiveDate.Date <= DateTime.Now.Date)
                .OrderByDescending(t => t.EffectiveDate)
                .FirstOrDefault();

            return current;
        }

        private IEnumerable<TaxRateModel> LimitByDate(List<TaxRate> rates)
        {
            // Get current tax rate
            List<TaxRate> final = new List<TaxRate>();
            var current = rates.Where(t => t.EffectiveDate.Date <= DateTime.Now.Date)
                    .OrderByDescending(t => t.EffectiveDate)
                    .FirstOrDefault();
            
            if (current != null)
            {
                final.Add(current);
            }

            // Add future tax rates
            final.AddRange(
                rates.Where(t => t.EffectiveDate.Date > DateTime.Now.Date)
                    .OrderBy(t => t.EffectiveDate));

            if (final.Any())
            {
                return from f in final
                       select new TaxRateModel
                      {
                          Id = f.Id,
                          Name = f.Name,
                          Rate = f.Rate,
                          EffectiveDate = f.EffectiveDate,
                          ProductId = f.ProductId
                      };
            }
            return new List<TaxRateModel>();
        }

        public void Delete(int id)
        {
            using (var scope = Context.Write())
            {
                var rate = scope.Context.TaxRate.Find(id);
                if (rate != null)
                {
                    var sku = scope.Context.Product.Find(rate.ProductId ?? 0);
                    scope.Context.TaxRate.Remove(rate);
                    audit.LogAsync(new { Type = sku != null ? " Sku " + sku.SKU : " system rate", rate }, RetailPriceEvents.DeleteTaxRate, EventCategories.Merchandising);
                }
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public TaxRateModel Save(TaxRateModel model)
        {
            TaxRateModel tr;
            using (var scope = Context.Write())
            {
                var rate = scope.Context.TaxRate.Find(model.Id);
                var sku = scope.Context.Product.Find(model.ProductId ?? 0);

                string eventType;
               
                if (rate == null)
                {
                    rate = new TaxRate();
                    Mapper.Map(model, rate);
                    scope.Context.TaxRate.Add(rate);
                    eventType = RetailPriceEvents.CreateTaxRate;
                }
                else
                {
                    Mapper.Map(model, rate);
                    eventType = RetailPriceEvents.EditTaxRate;
                }
                
                scope.Context.SaveChanges();
               
                audit.LogAsync(new { Type = sku != null ? " Sku "+sku.SKU : " system rate", rate }, eventType, EventCategories.Merchandising);
                model.Id = rate.Id;
                tr = model;
                scope.Complete();
                return tr;
            }
        }
    }
}
