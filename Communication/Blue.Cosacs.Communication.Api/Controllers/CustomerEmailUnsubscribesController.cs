using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Blue.Cosacs.Communication.Repositories;

namespace Blue.Cosacs.Communication.Api.Controllers
{
    public class CustomerEmailUnsubscribesController : ApiController
    {
        private readonly ICommunicationRepository repository;

        public CustomerEmailUnsubscribesController(ICommunicationRepository repository)
        {
            this.repository = repository;
        }

        public EmailUnsubscribesResponse[] Get([FromUri] string[] emails)
        {
            var all = new HashSet<string>(emails);

            var unsubscribed = repository.GetEmailUnsubcription(emails)
                .Select(p => p.Email)
                .ToList();

            //remove from all people that unsubscribe so i get only subscriptions
            all.ExceptWith(unsubscribed);

            return all.Select(p => new EmailUnsubscribesResponse
            {
                Email = p,
                IsUnsubscribe = false
            })
            .Union(unsubscribed
                .Select(p => new EmailUnsubscribesResponse
                {
                    Email = p,
                    IsUnsubscribe = true
                }))
            .ToArray();
        }
    }

    public class EmailUnsubscribesResponse
    {
        public string Email { get; set; }
        public bool IsUnsubscribe { get; set; }
    }
}
