using System.Net.Http;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Api.Models;
using Blue.Cosacs.SalesManagement.EventTypes;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/QuickDetailsCapture")]
    public class QuickDetailsCaptureController : ApiController
    {
        private readonly ISalesManagementRepository salesManagementRepository;
        private readonly IClock clock;
        private readonly IEventStore audit;

        public QuickDetailsCaptureController(IClock clock, ISalesManagementRepository salesManagementRepository, IEventStore audit)
        {
            this.salesManagementRepository = salesManagementRepository;
            this.clock = clock;
            this.audit = audit;
        }

        [Route("InsertCall")]
        [Permission(SalesManagementPermissionEnum.QuickDetailsCapture)]
        public HttpResponseMessage InsertCall(CallLog callLog)
        {
            var call = new Call()
            {
                CustomerId = callLog.CustomerId,
                ToCallAt = callLog.ScheduleCallback.Value,
                CreatedOn = clock.Now,
                CustomerFirstName = callLog.CustomerFirstName,
                CustomerLastName = callLog.CustomerLastName,
                CallTypeId = (byte)CallTypeEnum.Prospective,
                Source = (byte)CallSourceEnum.UserInterface,
                ReasonToCall = callLog.ReasonToCallAgain,
                SpokeToCustomer = callLog.SpokeToCustomer,
                CreatedBy = this.GetUser().Id,
                Branch = this.GetUser().Branch,
                MobileNumber = callLog.MobileNumber,
                LandLinePhone = callLog.LandLinePhone,
                SalesPersonId = this.GetUser().Id
            };

            salesManagementRepository.ScheduleCall(call);

            audit.Log(call, EventType.QuickDetailsCaptureCall);

            return Request.CreateResponse(new
            {
                Message = "Calls inserted"
            });
        }
    }
}