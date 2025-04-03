
namespace Blue.Cosacs.Warehouse
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            Add(new TruckPickListProvider());
            Add(new DriverPickListProvider());         
        }

        private void Add(IPickListProvider provider)
        {
            For<IPickListProvider>().Singleton().Add(provider).Named(provider.Id);
        }
    }
}
