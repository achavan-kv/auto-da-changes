namespace Blue.Cosacs.Merchandising
{
    using Blue.Cosacs.Merchandising.DataWarehousing;
    using System.Configuration;

    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            // structuremap dependency mappings here
            this.Scan(
                x =>
                    {
                        x.AssemblyContainingType<Registry>();
                        x.WithDefaultConventions();
                        x.RegisterConcreteTypesAgainstTheFirstInterface();
                    }
                );

            Add(new LocationsListProvider());

            For<CosacsDW>().Use(ctx => new CosacsDW(ConfigurationManager.ConnectionStrings["Olap"].ConnectionString));
        }

        private void Add(IPickListProvider provider)
        {
            For<IPickListProvider>().Singleton().Add(provider).Named(provider.Id);
        }
    }
}
