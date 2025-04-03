namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Event;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Events;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProductStockRepository
    {
        IEnumerable<ProductStockLocationView> Get(int productId);

        IEnumerable<ProductStockLocationView> Get(int locationId, List<int> productIds = null);

        void Delete(int id);

        ProductStockLocationView Save(ProductStockLocationView model);
    }

    public class ProductStockRepository : IProductStockRepository
    {
        private readonly IEventStore audit;

        public ProductStockRepository(IEventStore audit)
        {
            this.audit = audit;
        }

        public IEnumerable<ProductStockLocationView> Get(int productId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ProductStockLocationView.Where(p => p.ProductId == productId).OrderBy(l => l.LocationName).ToList();
            }
        }

        public IEnumerable<ProductStockLocationView> Get(int locationId, List<int> productIds = null)
        {
            using (var scope = Context.Read())
            {
                if (productIds == null)
                {
                    return scope.Context.ProductStockLocationView.Where(p => p.LocationId == locationId).ToList();
                }
                /*return scope.Context.ProductStockLocationView.Where(p => p.LocationId == locationId && productIds.Any(ids => ids == p.ProductId)).ToList(); */
				return scope.Context.ProductStockLocationView.Where(p => p.LocationId == locationId && productIds.Contains(p.ProductId)).ToList(); //Incident #5639230, #5438712 
            }
        }

        public void Delete(int id)
        {
            using (var scope = Context.Write())
            {
                var status = scope.Context.ProductStockLocationView.Find(id);
                if (status != null)
                {
                    scope.Context.ProductStockLocationView.Remove(status);
                    audit.LogAsync(status, ProductEvents.DeleteProductStockLevel, EventCategories.Merchandising);
                }
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public ProductStockLocationView Save(ProductStockLocationView model)
        {
            using (var scope = Context.Write())
            {
                var stock = scope.Context.ProductStockLocationView.Find(model.Id);
                string eventType;

                if (stock == null)
                {
                    stock = model;
                    scope.Context.ProductStockLocationView.Add(stock);
                    eventType = ProductEvents.CreateProductStockLevel;
                }
                else
                {
                    Mapper.Map(model, stock);
                    eventType = ProductEvents.EditProductStockLevel;
                }
                scope.Context.SaveChanges();
                audit.LogAsync(stock, eventType, EventCategories.Merchandising);
                scope.Complete();
                return stock;
            }
        }
    }
}
