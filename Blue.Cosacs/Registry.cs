
using Blue.Cosacs.PickLists;

namespace Blue.Cosacs
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<IClock>().Singleton().Add<DateTimeClock>();

            // Pick Lists registration 
            //foreach (var provider in CodePickListProvider.All())
            //    Add(provider);
            //foreach (var provider in SetsPickListProvider.All())
            //    Add(provider);
            //Add(new BranchPickListProvider());
            //Add(new ProductCategoriesPickListProvider());
            //Add(new EmpeenoPickListProvider());
            //Add(new WarehousePickRejectReasonsPickListProvider());
            //Add(new WarehouseLoadRejectReasonsPickListProvider());
            //Add(new WarehouseDelRejectReasonsPickListProvider());
            //Add(new EmployeeListProvider());

            For<Hub.Client.IPublisher>().Singleton().Use(
                (ctx) => new Hub.Client.SqlPublisher(STL.DAL.Connections.DefaultName, ctx.GetInstance<IClock>()));

            // PickLists
            Add(new BranchPickListProvider());
            Add(new BankPickListProvider());
            //Add(new ProductCategoriesPickListProvider());
            //Add(new EmpeenoPickListProvider());
            //Add(new ServiceResolutionListProvider());
            //Add(new SalePersonListProvider());

        }

        private void Add(IPickListProvider provider)
        {
            For<IPickListProvider>().Singleton().Add(provider).Named(provider.Id);
        }
    }
}
