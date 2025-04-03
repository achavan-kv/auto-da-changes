using Blue.Cosacs.Stock.Repositories;
using Blue.Cosacs.Test.Mocks;
using Blue.Cosacs.Warranty.Repositories;
using Blue.Events;

namespace Blue.Cosacs.Test.Warranty
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<IClock>().Singleton().Add<MockIClock>();
            For<IEventStore>().Singleton().Use(ctx => new MockEventStore());
            For<Blue.Config.Repositories.ISettings>().Use<MockSettings>(); 
        }
    }
}
