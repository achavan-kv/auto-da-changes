using System.Web.Http;
using Blue.Cosacs.SalesManagement.Hub.Subscribers;
using Blue.Glaucous.Client.Api;
using Blue.Hub.Client.Web;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/CustomerFollowUpCalls")]
    [Permission(SalesManagementPermissionEnum.SalesFollowUpCallsJob)]
    public class CustomerFollowUpCallsController : HttpSubscriberController<ScheduleFollowUpCalls>
    {
        public CustomerFollowUpCallsController(ScheduleFollowUpCalls subscriber)
            : base(subscriber)
        {
        }
    }
}