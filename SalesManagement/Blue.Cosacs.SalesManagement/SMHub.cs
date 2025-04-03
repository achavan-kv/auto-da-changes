using Blue.Cosacs.SalesManagement.Messages;
using Blue.Hub.Client;

namespace Blue.Cosacs.SalesManagement
{
    public static class SMHub
    {
        private static readonly IPublisher Publicsher;

         static SMHub()
        {
            Publicsher = StructureMap.ObjectFactory.GetInstance<IPublisher>();
        }

        public static void PublishMail(MailMessage message)
        {
            Publicsher.Publish<MailMessage>("Cosacs.Communication.SendMail", message);
        }

        public static void PublishSms(SmsMessage message)
        {
            Publicsher.Publish<SmsMessage>("Cosacs.Communication.SendSms", message);
        }
    }
}
