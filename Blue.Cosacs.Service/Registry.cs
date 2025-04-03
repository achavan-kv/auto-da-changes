
using Blue.Cosacs.Service.Repositories;
using Blue.Events;
using Blue.Transactions;
namespace Blue.Cosacs.Service
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            Add(new SupplierAccountPickListProvider());
            For<TechnicianRepository>().Singleton().Use((ctx) => new TechnicianRepository(ctx.GetInstance<IEventStore>(), ctx.GetInstance<IClock>()));
            For<ReadScope<Context>>().Use(() => new ReadScope<Context>());
        }

        private void Add(IPickListProvider provider)
        {
            For<IPickListProvider>().Singleton().Add(provider).Named(provider.Id);
        }
    }
}
