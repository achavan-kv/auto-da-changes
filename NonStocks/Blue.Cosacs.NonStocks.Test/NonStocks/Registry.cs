using Blue.Cosacs.NonStocks.ExternalHttpService;
using Blue.Cosacs.NonStocks.Test;
using Blue.Cosacs.NonStocks.Test.Mocks;
using Blue.Networking;

namespace Blue.Cosacs.NonStocks.Test.NonStocks
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<IClock>().Singleton().Add<IClockMock>();
            For<IPriceRepository>().Use<PriceRepositoryMock>();
            For<IPromotionsRepository>().Use<PromotionsRepositoryMock>();
            For<IProductLinkRepository>().Use<ProductLinkRepositoryMock>();
            For<IHttpClientJson>().Use<HttpClientJson>();
            For<IHttpClient>().Use<HttpClient>();
            For<ICourtsNetWS>().Use<CourtsNetWSMock>();
        }
    }
}
