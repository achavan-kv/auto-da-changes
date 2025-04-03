using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/FollowUpCalls")]
    public class FollowUpCallsController : ApiController
    {
        private readonly ISalesManagementRepository repository;

        public FollowUpCallsController(ISalesManagementRepository repository)
        {
            this.repository = repository;
        }

        [Permission(SalesManagementPermissionEnum.ConfigureFollowupCalls)]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(GetFollowUpCalls());
        }

        private IList<Models.FollowUpCall> GetFollowUpCalls()
        {
            return repository.GetFollowUpCalls()
                    .Select(p => new Models.FollowUpCall
                    {
                        Id = p.Id,
                        Quantity = p.Quantity,
                        ReasonToCall = p.ReasonToCall,
                        TimePeriod = p.TimePeriod,
                        Icon = p.Icon,
                        AlternativeContactMeanId = p.AlternativeContactMeanId,
                        MailchimpTemplateID = p.MailchimpTemplateID,
                        SmsText = p.SmsText,
                        ContactMeansId = p.ContactMeansId,
                        ContactEmailSubject = p.ContactEmailSubject,
                        FlushedEmailSubject = p.FlushedEmailSubject
                    })
                    .ToList();
        }

        [Permission(SalesManagementPermissionEnum.ConfigureFollowupCalls)]
        public HttpResponseMessage Put(Models.FollowUpCall followUpCall)
        {
            repository.SaveFollowUpCall(followUpCall.ToEntitySet());

            return Request.CreateResponse();
        }

        [Permission(SalesManagementPermissionEnum.ConfigureFollowupCalls)]
        public HttpResponseMessage Delete(int id)
        {
            repository.DeleteFollowUpCall(id);

            return Request.CreateResponse();
        }

        [Permission(SalesManagementPermissionEnum.ConfigureFollowupCalls)]
        public HttpResponseMessage Post(Models.FollowUpCall followUpCall)
        {
            repository.SaveFollowUpCall(followUpCall.ToEntitySet());

            return Request.CreateResponse(HttpStatusCode.Created, GetFollowUpCalls());
        }
    }
}