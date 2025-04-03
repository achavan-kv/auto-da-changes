using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/PreviousCalls")]
    public class PreviousCallController : ApiController
    {
        private readonly ISalesManagementRepository repository;
        private readonly IClock clock;
        private readonly IEventStore audit;
        private readonly Blue.Cosacs.SalesManagement.Settings cosacsSettings;

        public PreviousCallController(IClock clock, ISalesManagementRepository repository, IEventStore audit)
        {
            this.clock = clock;
            this.repository = repository;
            this.audit = audit;
            this.cosacsSettings = new Blue.Cosacs.SalesManagement.Settings();
        }

        [Route("GetPreviousCalls")]
        [Permission(SalesManagementPermissionEnum.CSRCallLog)]
        public HttpResponseMessage GetPreviousCalls(string customerId)
        {
           var numberofCallsToDisplay = cosacsSettings.LastXCalls;

           var calls = repository.GetPreviousCallsForACustomer(customerId, numberofCallsToDisplay);

           var previousCalls = calls.Select(p => new
           {
               CallId = p.CallId,
               CalledAt = p.CalledAt,
               CallTypeName = repository.GetCallTypeName(p.CallTypeId),
               ReasonForCalling = p.ReasonForCalling,
               SpokeToCustomer = p.SpokeToCustomer,
               Comments = p.Comments,
               SalesPersonId = p.SalesPersonId,
               RescheduledOn = p.RescheduledOn
           })
           .ToList();

           return Request.CreateResponse(previousCalls);
        }
    }
}