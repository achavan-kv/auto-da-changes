using StructureMap;

namespace Blue.Cosacs.Payments.Api
{
    public static class Bootstrapper
    {
        public static void Initialize()
        {
            ObjectFactory.Initialize(x =>
            {
                //x.AddRegistry(new GlobalRegistry());
                x.AddRegistry(new Blue.Admin.Registry());
                x.AddRegistry(new Blue.Cosacs.Payments.Registry());
            });
        }
    }
}
