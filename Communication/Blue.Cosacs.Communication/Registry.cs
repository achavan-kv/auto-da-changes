using StructureMap.Pipeline;
using Blue.Hub.Client;
using Blue.Cosacs.Communication.Repositories;
using Blue.Cosacs.Communication.MailsHandlers;

namespace Blue.Cosacs.Communication
{
    public class Registry : StructureMap.Configuration.DSL.Registry
    {
        public Registry()
        {
            For<IClock>().Singleton().Add<DateTimeClock>();

            For<ICommunicationRepository>().Add<CommunicationRepository>();

            var settings = new Settings();
            Settings.Register(this, settings);
            For<Settings>().Singleton().Use(settings);
            For<Blue.Config.ISettingsReader>().Singleton().Use(settings);

            For<IEmail>().Add<MandrillProvider>().Named("Mandrill");
            For<IEmail>().Add<Sandbox>().Named("SandBoxMode");

        }
    }
}
