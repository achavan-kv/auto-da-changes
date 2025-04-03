using Blue.Cosacs.Communication.Hub.Subscribers;
using Blue.Cosacs.Communication.Messages;

namespace Blue.Cosacs.Communication.MailsHandlers
{
    internal class MailGrouping
    {
        public MailGrouping(From from, short? templateId, bool? overrideUnsubscribe, string subject)
        {
            if (from == null)
            {
                throw new MailMessageIncongruousException("From");
            }

            if (!templateId.HasValue)
            {
                throw new MailMessageIncongruousException("TemplateId");
            }

            if (!overrideUnsubscribe.HasValue)
            {
                throw new MailMessageIncongruousException("OverrideUnsubscribe");
            }

            this.Subject = subject;
            this.From = from;
            this.TemplateId = templateId.Value;
            this.OverrideUnsubscribe = overrideUnsubscribe.Value;
        }

        public From From
        {
            get;
            set;
        }

        public short TemplateId
        {
            get;
            set;
        }

        public bool OverrideUnsubscribe
        {
            get;
            set;
        }

        public string Subject { get; set; }
    }
}