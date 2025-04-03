using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Glaucous.Client.Api;
using Blue.Networking;
using System.Linq;
using System.Collections.Concurrent;
using Blue.Cosacs.SalesManagement.Api.Models;
using System;
using System.Text;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [Permission(SalesManagementPermissionEnum.FlushUnmadeCallsJob)]
    public class FlushUnmadeCallsController : ApiController
    {
        private readonly IClock clock;
        private readonly Blue.Cosacs.SalesManagement.Settings cosacsSettings;
        private readonly ISalesManagementRepository repository;
        private readonly HttpClientJson httpClientJson;

        public FlushUnmadeCallsController(IClock clock, ISalesManagementRepository repository, HttpClientJson httpClientJson)
        {
            this.cosacsSettings = new Blue.Cosacs.SalesManagement.Settings();
            this.clock = clock;
            this.repository = repository;
            this.httpClientJson = httpClientJson;
        }

        [HttpGet]
        [LongRunningQueries]
        [CronJob]
        public HttpResponseMessage Get()
        {
            var data = repository.CallsToFlush(clock.Now, cosacsSettings.DaysToFlushUnmadeCalls);
            List<SmsToSend> smsToSend = null;
            List<MailsToSend> mailsToSend = null;

            foreach (var item in data.Where(p=> p.Key.HasValue))
            {
                switch ((ContactMeanEnum)item.Key.Value)
                {
                    case ContactMeanEnum.Email:
                        mailsToSend = SchuduleEmail(item.ToList());
                        break;

                    case ContactMeanEnum.Sms:
                        smsToSend = SchuduleSms(item.ToList());
                        break;
                }
            }

            var callsIds = data
                .Select(p => p.Select(x => x.Id).ToList())
                .SelectMany(p=> p)
                .ToArray();

            repository.FlushCalls(CallClosedReason.CallClosedReasonEnum.FlushedCall, callsIds, smsToSend, mailsToSend);

            return Request.CreateResponse();
        }

        private List<SmsToSend> SchuduleSms(IList<CallToFlush> data)
        {
            var cusatomers = data
                .Select(p => p.CustomerId)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToArray();

            var client = new HttpClientJsonAuth(new Blue.Networking.HttpClient(), this.clock, this.GetUser().Id.ToString());
            var unsubscribes = ExternalHttpSources.Get<List<SmsUnsubscribesResponse>>("/Communication/api/CustomerSmsUnsubscribes?" + ToQueryString(cusatomers, "customers"), client);

            return unsubscribes
                .Where(p => !p.IsUnsubscribe)
                .Join(
                    data,
                    left => left.CustomerId,
                    right => right.CustomerId,
                    (l, r) => new SmsToSend
                    {
                        Body = r.SmsText,
                        CustomerId = r.CustomerId,
                        DateToSend = clock.Now.Date,
                        PhoneNumber = r.MobileNumber
                    })
                .Where(p => !string.IsNullOrWhiteSpace(p.PhoneNumber))
                .ToList();
        }

        private List<MailsToSend> SchuduleEmail(IList<CallToFlush> data)
        {
            var currentData = repository.GetCustomersSalesPerson(null, data.Select(p => p.CustomerId).ToList())
                .Where(p=> !string.IsNullOrEmpty(p.Email))
                .Select(p => new
                {
                    p.CustomerId,
                    p.Email
                })
                .ToList();

            var client = new HttpClientJsonAuth(new Blue.Networking.HttpClient(), this.clock, this.GetUser().Id.ToString());

            var request = RequestJson<byte[]>.Create("/Communication/api/CustomerEmailUnsubscribes?" + ToQueryString(currentData.Select(p => p.Email).ToArray(), "emails"), "GET");

            var unsubscribes = client.Do<byte[], List<EmailUnsubscribesResponse>>(request).Body;

            //exclude customers that do not receive e-mails
            var willReceiveMail = unsubscribes
                .Where(p=> !p.IsUnsubscribe)
                .Join(
                    currentData,
                    left => left.Email.ToLower(),
                    right => right.Email.ToLower(),
                    (l, r) => r)
                .Where(p=> !string.IsNullOrEmpty(p.CustomerId))
                .ToList();

            if (willReceiveMail.Any())
            {
                //get customers data
                var customers = ExternalHttpSources.GetCustomer(httpClientJson, string.Empty, willReceiveMail.Select(p => p.CustomerId).ToArray());

                //create records to save
                return data
                    .Join(
                        customers,
                        left => left.CustomerId,
                        right => right.CustomerId,
                        (r, l) => new MailsToSend
                        {
                            CustomerId = r.CustomerId,
                            CustomerName = string.Format("{0} {1}", l.FirstName, l.LastName),
                            Body = null,
                            DateToSend = clock.Now.Date,
                            MailAdress = r.Email,
                            MailSudject = r.EmailSubject,
                            OverrideUnsubscribe = false,
                            TemplateId = r.MailchimpTemplateID.Value
                        })
                    .ToList();
            }

            return null;
        }

        private string ToQueryString(string[] data, string parameter)
        {
            var result = new StringBuilder();

            foreach (var item in data.Distinct())
            {
                 result.Append(parameter).Append("=").Append(System.Uri.EscapeDataString(item)).Append("&");
            }

            result.Length = result.Length - 1;

            return result.ToString();
        }
    }
}
