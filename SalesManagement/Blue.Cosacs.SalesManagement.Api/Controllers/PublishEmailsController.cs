using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Messages;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [Permission(SalesManagementPermissionEnum.PublishEmails)]
    public class PublishEmailsController : ApiController
    {
        private readonly ISalesManagementRepository repository;
        private readonly IClock clock;

        public PublishEmailsController(ISalesManagementRepository repository, IClock clock)
        {
            this.repository = repository;
            this.clock = clock;
        }

        [CronJob]
        public HttpResponseMessage Get()
        {
            Func<MailsToSend, KeyValuePair[]> getArrayOfTags = (mail) =>
            {
                var result = new List<KeyValuePair>(2);

                result.Add(new KeyValuePair
                {
                    Key = "CustomerName",
                    Value = mail.CustomerName
                });

                if (!string.IsNullOrEmpty(mail.Body))
                {
                    result.Add(new KeyValuePair
                    {
                        Key = "Body",
                        Value = mail.Body
                    });
                }

                return result.ToArray();
            };

            var mailsToSend = repository.LoadMailsToSend(clock.Now.Date);
            var mails = mailsToSend
                    .Select(p => new Mail()
                    {
                        ArrayOfTags = getArrayOfTags(p),
                        To = p.MailAdress,
                        Subject = p.MailSudject,
                        TemplateId = p.TemplateId,
                        OverrideUnsubscribe = p.OverrideUnsubscribe,
                        CustomerId = p.CustomerId
                    })
                    .ToArray();

            var message = new MailMessage
            {
                Mails = mails,
            };

            SMHub.PublishMail(message);

            repository.DeleteMailsToSend(mailsToSend.Select(p => p.id).ToList());

            return Request.CreateResponse();
        }
    }
}