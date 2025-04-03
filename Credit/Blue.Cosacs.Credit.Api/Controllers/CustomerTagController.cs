using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Repositories;
using Blue.Events;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class CustomerTagController : ApiController
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IEventStore audit;

        public CustomerTagController(ICustomerRepository customerRepository, IEventStore audit)
        {
            this.customerRepository = customerRepository;
            this.audit = audit;
        }

        public HttpResponseMessage Post(CustomerTag newCustomerTag)
        {
            int newTagId = customerRepository.AddCustomerTag(newCustomerTag);
            audit.Log(newCustomerTag, EventType.AddCustomerTag, EventCategory.Customer);

            return Request.CreateResponse(HttpStatusCode.OK, newTagId);
        }
    }
}
