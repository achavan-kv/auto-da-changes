using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blue.Cosacs.Communication.Hub.Subscribers;
using Blue.Cosacs.Communication.Messages;
using Blue.Cosacs.Communication.Repositories;
using Mandrill;
using Mandrill.Models;
using Mandrill.Requests.Messages;
using Mandrill.Requests.Rejects;

namespace Blue.Cosacs.Communication.MailsHandlers
{
    internal sealed class MandrillProvider : IEmail
    {
        private readonly Settings settings;
        private readonly ICommunicationRepository repository;
        private readonly IClock clock;

        public MandrillProvider(Settings settings, ICommunicationRepository repository, IClock clock)
        {
            this.settings = settings;

            if (string.IsNullOrWhiteSpace(settings.MandrillApiKey))
            {
                throw new ArgumentException("The MandrillApiKey is not been set.");
            }

            this.repository = repository;
            this.clock = clock;
        }

        public void Send(Messages.MailMessage mailMessage)
        {
            var templates = this.LoadTemplates();
            var mails = GroupMailsToSend(mailMessage, templates
                .Select(p => p)
                .ToDictionary(k => k.Value, v => v.Key));
            const string recipientType = "to";

            Parallel.ForEach(mails, (current) =>
            {
                var mandrill = new MandrillApi(settings.MandrillApiKey);
                var recipients = current
                    .Select(p => new EmailAddress
                    {
                        Email = p.To,
                        Type = recipientType
                    })
                    .ToList();

                var email = new EmailMessage
                {
                    To = recipients,
                    FromEmail = settings.EmailsSender,
                    FromName = settings.EmailsSender,
                    Subject = current.Key.Subject
                };

                //add the Recipient Variables for each mail to send
                //theses variables are in the in the ArrayOfTags of each Mail object
                foreach (var item in current.SelectMany(p => p.ArrayOfTags, (mailToSend, tag) => new
                {
                    mailToSend.To,
                    tag.Key,
                    tag.Value
                }))
                {
                    email.AddRecipientVariable(item.To, item.Key, item.Value);
                }

                var template = new SendMessageTemplateRequest(email, templates[current.Key.TemplateId], null);

                mandrill.SendMessageTemplate(template);
            });
        }

        public IList<BlackEmailList> GetRejected()
        {
            var result = new List<BlackEmailList>();

            //loader.ConfigureAwait(false);
            //IAsyncResult
            var loader = Task.Run(() =>
            {
                var mandrill = new MandrillApi(settings.MandrillApiKey);

                return mandrill.ListRejects(new ListRejectsRequest
                {
                    IncludeExpired = false,
                });
            });

            if (loader.Result != null && loader.Result.Count > 0)
            {
                result = loader.Result
                    .Select(p => new BlackEmailList
                    {
                        CreatedOn = this.clock.Now,
                        Email = p.Email,
                        Provider = "Mandrill",
                        Reason = string.Compare(p.Reason, "unsub", true) == 0 ? "Unsubscribe" : p.Reason
                    })
                    .ToList();
            }

            return result;
        }

        private IList<IGrouping<MailGrouping, Mail>> GroupMailsToSend(MailMessage mailMessage, Dictionary<string, short> templates)
        {
            var allMails = mailMessage.Mails
                .Select(p =>
                {
                    p.From = (p.From ?? mailMessage.From) ?? DefaultFrom();

                    if (!string.IsNullOrEmpty(p.TemplateNameId) && !p.TemplateId.HasValue)
                    {
                        p.TemplateId = templates[p.TemplateNameId];
                    }
                    else if (!string.IsNullOrEmpty(mailMessage.TemplateNameId) && !mailMessage.TemplateId.HasValue)
                    {
                        p.TemplateId = templates[mailMessage.TemplateNameId];
                    }
                    else if (string.IsNullOrEmpty(p.TemplateNameId) && !p.TemplateId.HasValue && string.IsNullOrEmpty(mailMessage.TemplateNameId) && !mailMessage.TemplateId.HasValue)
                    {
                        throw new MailMessageIncongruousException("TemplateId/TemplateNameId");
                    }

                    p.OverrideUnsubscribe = (p.OverrideUnsubscribe ?? mailMessage.OverrideUnsubscribe) ?? new Nullable<bool>(false);

                    return p;
                })
                .GroupBy(p => new MailGrouping(p.From, p.TemplateId, p.OverrideUnsubscribe, p.Subject))
                .ToList();

            return allMails;
        }

        private From DefaultFrom()
        {
            return new From
            {
                FromMail = settings.EmailsSender
            };
        }

        private Dictionary<short, string> LoadTemplates()
        {
            var result = new Dictionary<short, string>();

            foreach (var item in repository.GetMailchimpTemplateID())
            {
                result.Add(item.Id, item.TemplateId);
            }

            return result;
        }
    }
}