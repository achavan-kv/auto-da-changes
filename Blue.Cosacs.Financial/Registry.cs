namespace Blue.Cosacs.Financial
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            this.Scan(x =>
            {
                x.AssemblyContainingType<Registry>();
                x.WithDefaultConventions();
            });
        }
    }
}
