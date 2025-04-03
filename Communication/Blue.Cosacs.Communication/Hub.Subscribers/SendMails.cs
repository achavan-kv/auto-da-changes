using Blue.Cosacs.Communication.MailsHandlers;
using Blue.Cosacs.Communication.Messages;
using Blue.Hub.Client;
using System.Linq;
using Blue.Cosacs.Communication.Repositories;

namespace Blue.Cosacs.Communication.Hub.Subscribers
{
    public class SendMails : Subscriber
    {
        private readonly Settings communicationSettings;
        private readonly ICommunicationRepository repository;
        private readonly IClock clock;

        public SendMails(Settings communicationSettings, ICommunicationRepository repository, IClock clock)
        {
            this.communicationSettings = communicationSettings;
            this.repository = repository;
            this.clock = clock;
        }

        public override void Sink(int id, System.Xml.XmlReader message)
        {
            var mailMessage = Deserialize<MailMessage>(message);
            var senderType = communicationSettings.SandBoxMode ? "SandBoxMode" : "Mandrill";

            StructureMap.ObjectFactory.Container.GetInstance<IEmail>(senderType).Send(mailMessage);

            repository.UpdateCustomerInteraction(mailMessage.Mails
                .Where(p => !string.IsNullOrEmpty(p.CustomerId))
                .Select(p => p.CustomerId)
                .Distinct()//remove duplicated customers 
                .Select(p => new CustomerInteraction
                {
                    CustomerId = p,
                    LastEmailSentOn = clock.Now
                })
                .ToList());
        }
    }
}