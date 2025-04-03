using Blue.Cosacs.NonStocks.ExternalHttpService;
using Blue.Networking;

namespace Blue.Cosacs.NonStocks
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<Blue.Config.SettingsBase>().Use<Blue.Cosacs.NonStocks.Settings>();
            For<IClock>().Singleton().Add<DateTimeClock>();
            For<INonStocksRepository>().Add<NonStocksRepository>();
            For<IProductLinkRepository>().Add<ProductLinkRepository>();
            For<IPriceRepository>().Add<PriceRepository>();
            For<IHttpClient>().Use<HttpClient>();
            For<IHttpClientJson>().Use<HttpClientJsonRelative>(); // HttpClientJsonAuth
            For<ICourtsNetWS>().Use<CourtsNetWS>();

            var settings = new Settings();
            Settings.Register(this, settings);
            For<Blue.Config.ISettingsReader>().Singleton().Use(settings);
        }
    }
}
