using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Repositories;
using Blue.Events;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class CustomerContactsController : ApiController
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IEventStore audit;

        public CustomerContactsController(ICustomerRepository customerRepository, IEventStore audit)
        {
            this.customerRepository = customerRepository;
            this.audit = audit;    
        }

        public HttpResponseMessage Post(CustomerContact newContactDetails)
        {
            int newContactDetailsId = customerRepository.AddContactDetails(newContactDetails);
            audit.Log(newContactDetails, EventType.AddCustomerContactDetails, EventCategory.Customer);

            return Request.CreateResponse(HttpStatusCode.OK, newContactDetailsId);
        }

        public HttpResponseMessage Delete(CustomerContact contactDetailsToDelete)
        {
            customerRepository.DeleteContactDetails(contactDetailsToDelete);
            audit.Log(contactDetailsToDelete, EventType.DeleteCustomerContactDetails, EventCategory.Customer);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
