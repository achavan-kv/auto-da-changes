using System.Web.Http;
using Blue.Cosacs.Communication.Hub.Subscribers;
using Blue.Glaucous.Client.Api;
using Blue.Hub.Client.Web;


namespace Blue.Cosacs.Communication.Api.Controllers
{
    [RoutePrefix("api/SendSms")]
    public sealed class SendSmsController : HttpSubscriberController<SendSms>
    {
        public SendSmsController(SendSms subscriber)
            : base(subscriber)
        {
        }
    }
}
