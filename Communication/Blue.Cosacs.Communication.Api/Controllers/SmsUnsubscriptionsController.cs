using Blue.Cosacs.Communication.Hub.Subscribers;
using Blue.Glaucous.Client.Api;
using Blue.Hub.Client.Web;

namespace Blue.Cosacs.Communication.Api.Controllers
{
    [Permission(CommunicationPermissionEnum.UnsubscribeSms)]
    public class SmsUnsubscriptionsController : HttpSubscriberController<UnsubscribeSms>
    {
        public SmsUnsubscriptionsController(UnsubscribeSms subscriber)
            : base(subscriber)
        {
        }
    }
}
