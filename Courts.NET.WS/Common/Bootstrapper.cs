using StructureMap;
using System;
using System.Configuration;

namespace Blue.Cosacs.Web.WS
{
    public static class Bootstrapper
    {
        public static void Initialize()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new Cosacs.Registry());
            });
        }
    }
}