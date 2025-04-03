namespace Blue.Cosacs.Customer.Api
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        //private const string ConnectionStringName = "Default";

        public Registry()
        {
            /*For<Events.IEventStore>().Singleton().Use(ctx => new EventStore(ConnectionStringName));
            For<CustomerInstalmentEndingSubscriber>().Use<CustomerInstalmentEndingSubscriber>();*/
            For<Settings>().Singleton().Use<Settings>();
            For<IClock>().Singleton().Use<Blue.DateTimeClock>();

            var settings = new Settings();
            Settings.Register(this, settings);
            For<Blue.Config.ISettingsReader>().Singleton().Use(settings);
        }
    }
}