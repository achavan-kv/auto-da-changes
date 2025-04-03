using Blue.Config;

namespace Blue.Cosacs.Merchandising
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            var settings = new Blue.Cosacs.Merchandising.Settings();
            Blue.Config.Settings.Register(this, settings);
            For<ISettingsReader>().Singleton().Use(settings);
            For<IClock>().Singleton().Add<DateTimeClock>();
        }
    }
}
