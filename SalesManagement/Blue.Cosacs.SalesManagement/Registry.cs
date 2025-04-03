using Blue.Cosacs.SalesManagement.Hub.Subscribers;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Networking;
using Blue.Transactions;
using StackExchange.Redis;
using StructureMap.Pipeline;
using Blue.Hub.Client;

namespace Blue.Cosacs.SalesManagement
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<IClock>().Singleton().Add<DateTimeClock>();
            For<ISalesManagementRepository>().Add<SalesManagementRepository>();
            For<IDashboardRepository>().Add<DashboardRepository>();
            For<IHttpClient>().Use<HttpClient>();
            For<IHttpClientJson>().Use<HttpClientJsonRelative>();
            For<ICallSummarySubscriber>().Use<CallSummarySubscriber>();

            var settings = new Settings();
            Settings.Register(this, settings);
            For<Blue.Config.ISettingsReader>().Singleton().Use(settings);

            For<ITimeoutManager>().LifecycleIs(new ThreadLocalStorageLifecycle()).Use<TimeoutManager>();

            For<IPublisher>().Singleton().Use(
                (ctx) => new SqlPublisher("Default", ctx.GetInstance<IClock>()));
        }
    }
}
