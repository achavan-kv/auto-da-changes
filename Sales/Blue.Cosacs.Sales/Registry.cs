using Blue.Cosacs.Sales.Repositories;
using Blue.Networking;

namespace Blue.Cosacs.Sales
{
   public class Registry : StructureMap.Configuration.DSL.Registry
    {
       public Registry()
       {
           For<IClock>().Singleton().Add<DateTimeClock>();
           For<ISaleRepository>().Add<SaleRepository>();
           For<IOrderRepository>().Add<OrderRepository>();
           For<IWarrantyContractRepository>().Add<WarrantyContractRepository>();
           For<IHttpClient>().Use<HttpClient>();
           For<IHttpClientJson>().Use<HttpClientJsonRelative>();
           For<IProductRepository>().Add<ProductRepository>();
           For<ISalesLookupRepository>().Add<SalesLookupRepository>();

           var settings = new Settings();
           Settings.Register(this, settings);
           For<Blue.Config.ISettingsReader>().Singleton().Use(settings);
       }
    }
}
