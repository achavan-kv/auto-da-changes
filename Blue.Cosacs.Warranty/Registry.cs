using Blue.Caching;
using Blue.Config.Repositories;
using Blue.Cosacs.Stock.Repositories;
using Blue.Cosacs.Warranty.Repositories;
using Blue.Transactions;

namespace Blue.Cosacs.Warranty
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<IProductRepository>().Use<ProductRepository>();
            //For<ReadScope<Context>>().Use(() => new ReadScope<Context>());
            For<IWarrantyPromotionRepository>().Use<WarrantyPromotionRepository>();
            For<IWarrantyPriceRepository>().Use<WarrantyPriceRepository>();
            For<ISettings>().Use<Blue.Config.Repositories.Settings>();
            For<IWarrantyLinkRepository>().Use<WarrantyLinkRepository>();
            For<ICacheClient>().Singleton().Use(ctx => new MemoryCacheClient());
        }
    }
}
