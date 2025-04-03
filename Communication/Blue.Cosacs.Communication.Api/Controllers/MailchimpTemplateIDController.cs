using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Blue.Glaucous.Client.Api;
using Blue.Cosacs.Communication.Repositories;

namespace Blue.Cosacs.Communication.Api.Controllers
{
    [RoutePrefix("api/MailchimpTemplate")]
    public class MailchimpTemplateIDController : ApiController
    {
        private readonly ICommunicationRepository repository;
        private readonly IClock clock;

        public MailchimpTemplateIDController(IClock clock, ICommunicationRepository repository)
        {
            this.repository = repository;
            this.clock = clock;
        }

        [Route("{id:int}")]
        public HttpResponseMessage Get(int id)
        {
            return Request.CreateResponse(repository.GetMailchimpTemplateID(id));
        }

        [Route("")]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(repository.GetMailchimpTemplateID());
        }

        [Route("")]
        [Permission(CommunicationPermissionEnum.MailChimpTemplate)]
        public HttpResponseMessage Post(MailchimpTemplateID value)
        {
            if (value.Id == 0)
            {
                value.CreatedBy = this.GetUser().Id;
                value.CreatedOn = this.clock.Now;
            }

            repository.SaveMailchimpTemplateID(value);

            return Request.CreateResponse();
        }

        [Route("{id:int}")]
        [Permission(CommunicationPermissionEnum.MailChimpTemplate)]
        public HttpResponseMessage Delete(int id)
        {
            if (repository.DeleteMailchimpTemplateID(id))
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.NoContent);
            }

            return Request.CreateErrorResponse(System.Net.HttpStatusCode.Conflict, "Can not delete this Template because it is been used.");
        }
    }
}
