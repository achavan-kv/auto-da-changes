using Blue.Cosacs.SalesManagement.Hub.Subscribers;
using Blue.Hub.Client;
using Blue.Glaucous.Client;
using Blue.Admin;
using StackExchange.Redis;

namespace Blue.Cosacs.SalesManagement.Api
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            //For<CustomerInstalmentEndingSubscriber>().Use<CustomerInstalmentEndingSubscriber>();
        }
    }
}