using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Messages;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [Permission(SalesManagementPermissionEnum.PublishSms)]
    public class PublishSmsController : ApiController
    {
        private readonly ISalesManagementRepository repository;
        private readonly IClock clock;

        public PublishSmsController(ISalesManagementRepository repository, IClock clock)
        {
            this.repository = repository;
            this.clock = clock;
        }

        [CronJob]
        public HttpResponseMessage Get()
        {
            var smsToSend = repository.LoadSmsToSend(clock.Now.Date);

            var message = new SmsMessage
            {
                Sms = smsToSend
                    .Select(p => new Sms()
                    {
                        Body = p.Body,
                        Phone = p.PhoneNumber,
                        CustomerId = p.CustomerId
                    })
                    .ToArray()
            };

            SMHub.PublishSms(message);

            repository.DeleteSmsToSend(smsToSend.Select(p => p.Id).ToList());

            return Request.CreateResponse();
        }
    }
}