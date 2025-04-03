using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Api.Models;
using Blue.Cosacs.SalesManagement.EventTypes;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using Blue.Networking;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/BulkMails")]
    [Permission(SalesManagementPermissionEnum.CustomersearchBranchCustomers)]
    public class BulkMailsController : ApiController
    {
        private readonly ISalesManagementRepository salesManagementRepository;
        private readonly IEventStore audit;
        private readonly IClock clock;
        private readonly IHttpClient httpClient;

        public BulkMailsController(ISalesManagementRepository salesManagementRepository, IEventStore audit, IClock clock, IHttpClient httpClient)
        {
            this.salesManagementRepository = salesManagementRepository;
            this.audit = audit;
            this.clock = clock;
            this.httpClient = httpClient;
        }

        [Route("Selected")]
        [HttpPost]
        public HttpResponseMessage Selected(BulkMailSettings mails)
        {
            InsertMails(mails);

            return Request.CreateResponse();
        }

        private void InsertMails(BulkMailSettings mails)
        {
            var mailsToSend = mails.Customers
                .Select(p => new MailsToSend
                {
                    CustomerId = p.Id,
                    Body = mails.Body,
                    CustomerName = p.Name,
                    DateToSend = mails.ToMailAt,
                    MailAdress = p.Address,
                    MailSudject = mails.Subject,
                    OverrideUnsubscribe = false,
                    TemplateId = mails.MailchimpTemplateID
                })
                .Where(p => !string.IsNullOrWhiteSpace(p.MailAdress))
                .ToList();

            salesManagementRepository.InsertMailsToSend(mailsToSend);

            audit.LogAsync(
                       new
                       {
                           DateToSend = mails.ToMailAt,
                           Subject = mails.Subject,
                           TotalEmails = mails.Customers.Count(),
                           TemplateId = mails.MailchimpTemplateID
                       },
                       EventType.BulkSendEmails);
        }

        [Route("All")]
        [HttpPost]
        public HttpResponseMessage All(BulkMailSettingsAll data)
        {
            var qs = System.Web.HttpUtility.UrlEncode(data.CustomerFilter);
            var query = new CustomersQuery(this.clock, this.httpClient);

            var values = query.QueryCustomers(qs, this.GetUser().Branch, this.GetUser().Id)
                .Where(p => string.Compare("yes", p.ReceiveEmails, true) == 0 && ! string.IsNullOrEmpty(p.Email))
                .Select(p => new Recipient
                {
                    Address = p.Email,
                    Id = p.CustomerId,
                    Name = string.Format("{0} {1}", p.FirstName, p.LastName)
                })
                .ToList();

            InsertMails(new BulkMailSettings
            {
                Body = data.Body,
                Customers = values,
                MailchimpTemplateID = data.MailchimpTemplateID,
                Subject = data.Subject,
                ToMailAt = data.ToMailAt
            });

            return this.Request.CreateResponse();
        }
    }
}