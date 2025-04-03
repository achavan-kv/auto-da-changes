using Blue.Cosacs.Sales.Models;
using Blue.Cosacs.Sales.Repositories;
using Blue.Events;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Sales.Api.Controllers
{
    public class ContractsSetupController : ApiController
    {
        private readonly IEventStore audit;
        private readonly ContractsRepository repository;

        public ContractsSetupController(ContractsRepository repository, IEventStore audit)
        {
            this.repository = repository;
            this.audit = audit;
        }

        public IEnumerable<ContractsSetupDto> Get()
        {
            return repository.GetContracts().ToList();
        }

        public HttpResponseMessage Put(ContractsSetupDto[] contractsSetup)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            audit.LogAsync(new { contractsSetup = contractsSetup }, EventType.SaveContractsSetup, EventCategory.ContractsSetup);

            repository.SaveContractsSetup(contractsSetup);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
