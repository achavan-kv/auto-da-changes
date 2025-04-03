using StructureMap;

namespace Blue.Cosacs.SalesManagement.Api
{
    public static class Bootstrapper
    {
        public static void Initialize()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new Blue.Admin.Registry());
                x.AddRegistry(new Blue.Cosacs.SalesManagement.Registry());
            });
        }
    }
}