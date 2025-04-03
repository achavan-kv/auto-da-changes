using Blue.Cosacs.Payments.ExternalHttpService;
using Blue.Cosacs.Payments.Repositories;
using Blue.Networking;

namespace Blue.Cosacs.Payments
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            //For<Blue.Config.SettingsBase>().Use<Blue.Cosacs.Payments.Settings>();
            For<IClock>().Singleton().Add<DateTimeClock>();
            For<IHttpClient>().Use<HttpClient>();
            For<IHttpClientJson>().Use<HttpClientJsonRelative>(); // HttpClientJsonAuth
            For<ICourtsNetWS>().Use<CourtsNetWS>();
            For<IPaymentMethodMapRepository>().Use<PaymentMethodMapRepository>();
            For<IPaymentSetupRepository>().Use<PaymentSetupRepository>();

            var settings = new Settings();
            Settings.Register(this, settings);
            For<Blue.Config.ISettingsReader>().Singleton().Use(settings);
        }
    }
}
