using Blue.Cosacs.Credit.Api.Models;
using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Repositories;
using Blue.Events;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class CustomerPhotoController : ApiController
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IEventStore audit;

        public CustomerPhotoController(ICustomerRepository customerRepository, IEventStore audit)
        {
            this.customerRepository = customerRepository;
            this.audit = audit;
        }

        public HttpResponseMessage Post(CustomerPhotoIdentifier customerProfilePhoto)
        {
            customerRepository.UpdateCustomerProfilePhoto(customerProfilePhoto.CustomerId, customerProfilePhoto.PhotoIdentifier);
            audit.Log(customerProfilePhoto, EventType.UpdateCustomerProfilePhoto, EventCategory.Customer);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
