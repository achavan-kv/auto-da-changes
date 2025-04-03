using System.Collections.Generic;
using System.Threading.Tasks;
using Blue.Cosacs.Communication.Messages;
using Blue.Hub.Client;

namespace Blue.Cosacs.Communication
{
    public static class ComHub
    {
        private static readonly IPublisher Publicsher;

        static ComHub()
        {
            Publicsher = StructureMap.ObjectFactory.GetInstance<IPublisher>();
        }

        public static void PublishBalckListEmails(BalckListEmailsMessage message)
        {
            //Publicsher.Publish<BalckListEmailsMessage>("Cosacs.Communication.BalckListEmails", message);
        }
    }
}
