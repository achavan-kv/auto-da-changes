//using Blue.Cosacs.SalesManagement.Test.Mocks;
//using Blue.Events;
//using Blue.Networking;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using StackExchange.Redis;
//using Blue.Cosacs.SalesManagement.Repositories;
//using Blue.Cosacs.SalesManagement.Hub.Subscribers;

//namespace Blue.Cosacs.SalesManagement.Test.Repositories
//{
//    public class Registry : StructureMap.Configuration.DSL.Registry
//    {
//        public Registry()
//        {
//            For<IClock>().Singleton().Add<ClockMock>();
//            For<IEventStore>().Singleton().Add<EventStoreMock>();
//            For<IHttpClient>().Add<HttpClient>();
//            For<IDatabase>().Use(Blue.Glaucous.Client.RedisConnection.Database());
//            For<IDashboardRepository>().Add<DashboardRepositoryMock>();
//            For<ICallSummarySubscriber>().Add<CallSummarySubscriberMock>();

//            var settings = new Settings();
//            Settings.Register(this, settings);
//            For<Blue.Config.ISettingsReader>().Singleton().Use(settings);
//        }
//    }
//}
