using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Blue.Cosacs.Payments.Models;
using System.Net.Http;

namespace Blue.Cosacs.Payments.Api.Controllers
{
    [RoutePrefix("api/BankMaintenance")]
    public class BankMaintenanceController : ApiController
    {
        private readonly BankMaintenanceRepository bankRepository;

        public BankMaintenanceController(BankMaintenanceRepository bankRepository)
        {
            this.bankRepository = bankRepository;
        }

        public HttpResponseMessage Put(Bank bank)
        {
            try
            {
                var savedBank = bankRepository.SaveBank(bank);

                return Request.CreateResponse(new { Result = "Success", @Bank = bank });
            }
            catch (OperationCanceledException ex)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex);
            }
            catch (Exception e)
            {
                throw e;
            }
        
        }

        [Route("GetBanks")]
        [HttpGet]
        public HttpResponseMessage GetBanks()
        {
            var banks = bankRepository.GetBanks();

            return Request.CreateResponse(new { Result = "Done", @Banks = banks });
        }

        [Route("GetActiveBanks")]
        [HttpGet]
        public HttpResponseMessage GetActiveBanks()
        {
            var banks = bankRepository.GetBanks(getActiveBanks: true);

            return Request.CreateResponse(new { Result = "Done", @Banks = banks });
        }
      
    }
}
