using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Repositories;
using Blue.Events;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class CustomerAddressController : ApiController
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IEventStore audit;

        public CustomerAddressController(ICustomerRepository customerRepository, IEventStore audit)
        {
            this.customerRepository = customerRepository;
            this.audit = audit;
        }

        public HttpResponseMessage Post(CustomerAddress newCustomerAddress)
        {
            int newAddressDetailsId = customerRepository.AddAddressDetails(newCustomerAddress);
            audit.Log(newCustomerAddress, EventType.AddCustomerAddressDetails, EventCategory.Customer);

            return Request.CreateResponse(HttpStatusCode.OK, newAddressDetailsId);
        }
    }
}
