using Blue.Cosacs.Test.Mocks;
using Blue.Events;
using Blue.Hub.Client;

namespace Blue.Cosacs.Test.Service
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<IClock>().Singleton().Add<MockIClock>();
            For<IPublisher>().Singleton().Add<MockHubClientPublisher>();
            For<IEventStore>().Singleton().Add<MockEventStore>();
        }
    }
}
