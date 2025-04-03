using Blue.Cosacs.Payments.ExternalHttpService;
using Blue.Cosacs.Payments.Test;
using Blue.Cosacs.Payments.Test.Mocks;
using Blue.Networking;

namespace Blue.Cosacs.Payments.Test.Payments
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<IClock>().Singleton().Add<IClockMock>();
            For<IHttpClientJson>().Use<HttpClientJson>();
            For<IHttpClient>().Use<HttpClient>();
        }
    }
}
