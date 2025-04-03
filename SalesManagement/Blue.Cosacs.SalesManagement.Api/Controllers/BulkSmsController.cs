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
    [RoutePrefix("api/BulkSms")]
    [Permission(SalesManagementPermissionEnum.CustomersearchBranchCustomers)]
    public class BulkSmsController : ApiController
    {
        private readonly ISalesManagementRepository salesManagementRepository;
        private readonly IEventStore audit;
        private readonly IClock clock;
        private readonly IHttpClient httpClient;

        public BulkSmsController(ISalesManagementRepository salesManagementRepository, IEventStore audit, IClock clock, IHttpClient httpClient)
        {
            this.salesManagementRepository = salesManagementRepository;
            this.audit = audit;
            this.clock = clock;
            this.httpClient = httpClient;
        }

        [Route("Selected")]
        [HttpPost]
        public HttpResponseMessage Selected(BulkSmsSettings sms)
        {
            InsertSms(sms);

            return Request.CreateResponse();
        }

        private void InsertSms(BulkSmsSettings sms)
        {
            var smsToSend = sms.Customers
                .Select(p => new SmsToSend
                {
                    CustomerId = p.CustomerId,
                    Body = sms.SmsText,
                    DateToSend = sms.ToSendAt,
                    PhoneNumber = p.PhoneNumber
                })
                .Where(p => !string.IsNullOrWhiteSpace(p.PhoneNumber))
                .ToList();
            salesManagementRepository.InsertSmsToSend(smsToSend);

            audit.LogAsync(
                new
                {
                    DateToSend = sms.ToSendAt,
                    SmsText = sms.SmsText,
                    TotalEmails = sms.Customers.Count(),
                },
                EventType.BulkSendSms);
        }

        [Route("All")]
        [HttpPost]
        public HttpResponseMessage All(BulkSmsSettingsAll data)
        {
            var qs = System.Web.HttpUtility.UrlEncode(data.CustomerFilter);
            var query = new CustomersQuery(this.clock, this.httpClient);

            var values = query.QueryCustomers(qs, this.GetUser().Branch, this.GetUser().Id)
                .Where(p => string.Compare("yes", p.ReceiveSms, true) == 0 && !string.IsNullOrEmpty(p.MobileNumber))
                .Select(p => new SmsRecipient
                {
                    CustomerId = p.CustomerId,
                    PhoneNumber = p.MobileNumber
                })
                .ToList();

            InsertSms(new BulkSmsSettings
            {
                Customers = values,
                SmsText = data.SmsText,
                ToSendAt = data.ToSendAt
            });

            return Request.CreateResponse();
        }
    }
}