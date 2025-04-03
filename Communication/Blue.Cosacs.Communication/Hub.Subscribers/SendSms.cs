using System.Linq;
using Blue.Cosacs.Communication.Messages;
using Blue.Cosacs.Communication.Repositories;
using Blue.Hub.Client;

namespace Blue.Cosacs.Communication.Hub.Subscribers
{
    public sealed class SendSms : Subscriber
    {
        private readonly ICommunicationRepository repository;

        public SendSms(ICommunicationRepository repository)
        {
            this.repository = repository;
        }

        public override void Sink(int id, System.Xml.XmlReader message)
        {
            var smsMessage = Deserialize<SmsMessage>(message);

            //exclude people that do not want to receive sms's
            var blackSmsList = repository.GetSmsUnsubcription(smsMessage.Sms
                .Select(p => p.CustomerId)
                .ToList())
                .Select(p=> p.CustomerId);
            
            repository.InsertSmsToSend(smsMessage.Sms
                .Where(p=> !blackSmsList.Contains(p.CustomerId))
                .Select(p => new SmsToSend
                {
                    CustomerId = p.CustomerId,
                    Body = p.Body,
                    PhoneNumber = p.Phone
                })
                .ToList());
        }
    }
}