namespace Blue.Cosacs.Merchandising.Repositories
{
    using Blue.Cosacs.Event;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Events;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProductStatusRepository
    {
        List<ProductStatus> Get();

        ProductStatus Get(int id);

        void Delete(int id);

        bool CanDelete(int id);

        ProductStatus Save(ProductStatus model);
    }

    public class ProductStatusRepository : IProductStatusRepository
    {
        private readonly IEventStore audit;

        public ProductStatusRepository(IEventStore audit)
        {
            this.audit = audit;
        }

        public List<ProductStatus> Get()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ProductStatus.ToList();
            }
        }

        public ProductStatus Get(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ProductStatus.SingleOrDefault(l => l.Id == id);
            }
        }

        public void Delete(int id)
        {
            using (var scope = Context.Write())
            {
                var status = scope.Context.ProductStatus.Find(id);
                if (status != null)
                {
                    scope.Context.ProductStatus.Remove(status);
                    audit.LogAsync(status, ProductEvents.DeleteProductStatus, EventCategories.Merchandising);
                }
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public bool CanDelete(int id)
        {
            using (var scope = Context.Read())
            {
                return !scope.Context.Product.Any(p => p.Status == id);
            }
        }

        public ProductStatus Save(ProductStatus model)
        {
            using (var scope = Context.Write())
            {
                var status = scope.Context.ProductStatus.Find(model.Id);
                string eventType;

                if (status == null)
                {
                    status = model;
                    scope.Context.ProductStatus.Add(status);
                    eventType = ProductEvents.CreateProductStatus;
                }
                else
                {
                    eventType = ProductEvents.EditProductStatus;
                }

                scope.Context.SaveChanges();
                audit.LogAsync(status, eventType, EventCategories.Merchandising);
                scope.Complete();
                return status;
            }
        }
    }
}
