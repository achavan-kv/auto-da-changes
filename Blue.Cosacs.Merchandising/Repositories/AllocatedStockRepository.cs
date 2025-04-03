namespace Blue.Cosacs.Merchandising.Repositories
{
    using System.Collections.Generic;
    using System.Linq;

    public interface IAllocatedStockRepository
    {
        List<AllocatedStockView> Get(int? locationId, string fascia, string sku, List<string> tags);
    }

    public class AllocatedStockRepository : IAllocatedStockRepository
    {
        private readonly IProductRepository productRepository;

        public AllocatedStockRepository(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public List<AllocatedStockView> Get(int? locationId, string fascia, string sku, List<string> tags)
        {
            using (var scope = ReportingContext.Read())
            {
                var results = scope.Context.AllocatedStockView.AsQueryable();

                if (locationId.HasValue)
                {
                    results = results.Where(q => q.LocationId == locationId.Value);
                }

                if (!string.IsNullOrWhiteSpace(fascia))
                {
                    results = results.Where(q => q.Fascia == fascia || q.LocationIsWarehouse);
                }

                if (!string.IsNullOrWhiteSpace(sku))
                {
                    results = results.Where(q => q.Sku == sku);
                }

                if (tags != null && tags.Any())
                {
                    var products = productRepository.SearchBySku(results.Select(r => r.Sku).ToList(), tags).Select(p => p.SKU).ToList();
                    results = results.Where(r => products.Contains(r.Sku));
                }
                return results.ToList();
            }
        }
    }
}
