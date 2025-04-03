using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Blue.Cosacs.Communication.Repositories;
using System.Linq;

namespace Blue.Cosacs.Communication.Api.Controllers
{
    public class CustomerSmsUnsubscribesController : ApiController
    {
        private readonly ICommunicationRepository repository;

        public CustomerSmsUnsubscribesController(ICommunicationRepository repository)
        {
            this.repository = repository;
        }

        public SmsUnsubscribesResponse[] Get([FromUri] string[] customers)
        {
            var all = new HashSet<string>(customers);

            var unsubscribed = repository.GetSmsUnsubcription(customers)
                .Select(p => p.CustomerId)
                .ToList();

            //remove from all people that unsubscribe so i get only subscriptions
            all.ExceptWith(unsubscribed);

            return all.Select(p => new SmsUnsubscribesResponse
            {
                CustomerId = p,
                IsUnsubscribe = false
            })
            .Union(unsubscribed
                .Select(p => new SmsUnsubscribesResponse
                {
                    CustomerId = p,
                    IsUnsubscribe = true
                }))
            .ToArray();
        }
    }

    public class SmsUnsubscribesResponse
    {
        public string CustomerId { get; set; }
        public bool IsUnsubscribe { get; set; }
    }
}
