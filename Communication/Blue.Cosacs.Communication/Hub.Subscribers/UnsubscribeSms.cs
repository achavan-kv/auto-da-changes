using Blue.Cosacs.Communication.Messages;
using Blue.Cosacs.Communication.Repositories;
using Blue.Hub.Client;

namespace Blue.Cosacs.Communication.Hub.Subscribers
{
    public class UnsubscribeSms : Subscriber
    {
        private readonly ICommunicationRepository repository;

        public UnsubscribeSms(ICommunicationRepository repository)
        {
            this.repository = repository;
        }

        public override void Sink(int id, System.Xml.XmlReader message)
        {
            this.repository.InsertSmsUnsubcription(Deserialize<CustomerPhoneNumbers>(message));
        }
    }
}
