using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Blue.Cosacs.Payments.Models;
using Blue.Cosacs.Payments.Repositories;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.Payments.Api.Controllers
{
    [RoutePrefix("api/PaymentMethodLookUp")]
    public class PaymentMethodMapController : ApiController
    {
        private IPaymentMethodMapRepository repository;

        public PaymentMethodMapController(IPaymentMethodMapRepository repos)
        {
            repository = repos;
        }

        public List<PaymentMethodMapDto> Get()
        {
            var user = this.GetUser();
            var branchNo = user.Branch;

            return repository.Get(branchNo);
        }

        public string Post(short posId, short winCosacsId)
        {
            var user = this.GetUser();
            var branchNo = user.Branch;

            return repository.Save(branchNo, posId, winCosacsId);
        }
    }
}
