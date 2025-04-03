using Blue.Cosacs.Credit.Repositories;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class CustomerDetailsController : ApiController
    {
        private readonly ICustomerRepository customerRepository;

        public CustomerDetailsController(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        public HttpResponseMessage Get(int id)
        {
            var customerFullDetails = customerRepository.GetCustomerFullDetails(id);
            var response = Request.CreateResponse(new { customerFullDetails = customerFullDetails });

            return response;
        }
    }
}