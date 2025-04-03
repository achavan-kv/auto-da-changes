using System.Web.Http;
using Blue.Cosacs.Communication.Hub.Subscribers;
using Blue.Glaucous.Client.Api;
using Blue.Hub.Client.Web;

namespace Blue.Cosacs.Communication.Api.Controllers
{
    [RoutePrefix("api/SendEmails")]
    [Permission(CommunicationPermissionEnum.SendEmails)]
    public sealed class SendEmailsController : HttpSubscriberController<SendMails>
    {
        public SendEmailsController(SendMails subscriber)
            : base(subscriber)
        {
        }
    }
}
