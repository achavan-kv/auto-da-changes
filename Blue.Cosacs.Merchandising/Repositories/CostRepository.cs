namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ICostRepository
    {
        CostPriceModel Get(int id);

        IEnumerable<CostPriceModel> GetByProduct(int id);

        CostPriceModel GetLatestByProduct(int productId);

        CostPriceModel GetLatestBySku(string sku);

        List<CostPrice> GetCurrentByProducts(List<int> productIds, DateTime? date = null);

        void Delete(int id);

        CostPrice Save(CostPriceCreateModel model);
    }

    public class CostRepository : ICostRepository
    {
        private readonly IEventStore audit;
        private readonly IRetailPriceRepository retailPriceRepository;
        private readonly Settings settings;
        private readonly Config.Settings configSettings;

        private readonly IProductStatusProgresser productStatusProgresser;

        public CostRepository(IEventStore audit, IRetailPriceRepository retailPriceRepository, Settings settings, Config.Settings configSettings, IProductStatusProgresser productStatusProgresser)
        {
            this.audit = audit;
            this.retailPriceRepository = retailPriceRepository;
            this.settings = settings;
            this.configSettings = configSettings;
            this.productStatusProgresser = productStatusProgresser;
        }

        public CostPriceModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var cost = scope.Context.CostPrice.FirstOrDefault(cp => cp.Id == id);
                return cost != null ? Mapper.Map<CostPriceModel>(cost) : null;
            }
        }

        public IEnumerable<CostPriceModel> GetByProduct(int id)
        {
            using (var scope = Context.Read())
            {
                return Mapper.Map<IEnumerable<CostPriceModel>>(scope.Context.CostPriceView.Where(l => l.ProductId == id).ToList());
            }
        }

        public CostPriceModel GetLatestByProduct(int productId)
        {
            using (var scope = Context.Read())
            {
                var cost = scope.Context.CurrentCostPriceView
                    .Where(x => x.ProductId == productId).FirstOrDefault();

                return Mapper.Map<CostPriceModel>(cost);
            }
        }

        public CostPriceModel GetLatestBySku(string sku)
        {
            using (var scope = Context.Read())
            {
                var product = scope.Context.Product
                    .FirstOrDefault(x => x.SKU == sku);

                if (product == null)
                {
                    return null;
                }
                            
                var cost = scope.Context.CurrentCostPriceView
                    .FirstOrDefault(x => x.ProductId == product.Id);

                return Mapper.Map<CostPriceModel>(cost);
            }
        }

        public List<CostPrice> GetCurrentByProducts(List<int> productIds, DateTime? date = null)
        {
            using (var scope = Context.Read())
            {
                var prices = scope.Context.CurrentCostPriceView.Where(cp => productIds.Contains(cp.ProductId)).ToList();
                return Mapper.Map<List<CostPrice>>(prices);
            }
        }
        
        public void Delete(int id)
        {
            using (var scope = Context.Write())
            {
                var cost = scope.Context.CostPrice.Find(id);
                var sku = scope.Context.Product.Find(cost.ProductId);

                if (cost != null)
                {
                    scope.Context.CostPrice.Remove(cost);
                    audit.LogAsync(new { sku.SKU, cost }, RetailPriceEvents.DeleteCost, EventCategories.Merchandising);
                }
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public CostPrice Save(CostPriceCreateModel model)
        {
            using (var scope = Context.Write())
            {
                var sku = scope.Context.Product.Find(model.ProductId);
                var cost = new CostPrice();
                Mapper.Map(model, cost);
                
                var current = scope.Context.CurrentCostPriceView.FirstOrDefault(p => p.ProductId == cost.ProductId);

                if (current == null)
                {
                    cost.AverageWeightedCost = 0;
                    cost.AverageWeightedCostUpdated = DateTime.UtcNow;
                }
                else
                {
                    cost.AverageWeightedCost = current.AverageWeightedCost;
                    cost.AverageWeightedCostUpdated = current.AverageWeightedCostUpdated;
                }
                if (sku.PreviousProductType== "RegularStock")
                {
                    cost.AverageWeightedCost = model.AverageWeightedCost;
                }

                cost.LastLandedCostUpdated = DateTime.UtcNow;
                scope.Context.CostPrice.Add(cost);
                scope.Context.SaveChanges();

                var product = scope.Context.Product.Single(p => p.Id == model.ProductId);
                productStatusProgresser.AutoProgress(product);
                scope.Context.SaveChanges();
              
                audit.LogAsync(new { sku.SKU, cost }, RetailPriceEvents.CreateCost, EventCategories.Merchandising);
                scope.Complete();
                return cost;
            }
        }
    }
}
