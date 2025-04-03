using StructureMap;

namespace Blue.Cosacs.Sales.Api
{
    public static class Bootstrapper
    {
        public static void Initialize()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new Admin.Registry());
                x.AddRegistry(new Sales.Registry());
            });
        }
    }
}