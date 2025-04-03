using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/SalesPersonTargets")]
    public class SalesPersonTargetsController : ApiController
    {
        private readonly ISalesManagementRepository salesManagementRepository;
        private readonly IClock clock;
        private readonly IEventStore audit;

        public SalesPersonTargetsController(IClock clock, ISalesManagementRepository salesManagementRepository, IEventStore audit)
        {
            this.salesManagementRepository = salesManagementRepository;
            this.clock = clock;
            this.audit = audit;
        }

        [Permission(SalesManagementPermissionEnum.CSRTargets)]
        public HttpResponseMessage Get()
        {
            var salesPersonTargets = salesManagementRepository.GetSalesPersonTargets(this.GetUser().Id);
            return Request.CreateResponse(salesPersonTargets);
        }

        [Route("SaveSalesPersonTargets")]
        [HttpGet]
        [Permission(SalesManagementPermissionEnum.CSRTargets)]
        public HttpResponseMessage SaveSalesPersonTargets(decimal targetYear)
        {
            var salesPersonTargets = new SalesPersonTargets()
            {
                Year = (short)clock.Now.Year,
                CreatedBy = this.GetUser().Id,
                TargetYear = targetYear,
                CreatedOn = clock.Now
            };

            salesManagementRepository.SaveCSRTargets(salesPersonTargets, audit);

            return Request.CreateResponse();
        }
    }
}