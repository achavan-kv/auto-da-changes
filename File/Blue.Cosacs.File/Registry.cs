
namespace Blue.Cosacs.File
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<IClock>().Singleton().Add<DateTimeClock>();
            For<IStorageProvider>().Add<SqlStorageProvider>();
        }
    }
}
