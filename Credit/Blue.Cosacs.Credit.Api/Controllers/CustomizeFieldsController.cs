using Blue.Cosacs.Credit.EventTypes;
using Blue.Cosacs.Credit.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using System.Net.Http;
using System.Web.Http;

namespace Blue.Cosacs.Credit.Api.Controllers
{
    public class CustomizeFieldsController : ApiController
    {
        private IEventStore audit;
        private readonly ICustomizeFieldsRepository repository;

        public CustomizeFieldsController(IEventStore audit, ICustomizeFieldsRepository repository)
        {
            this.audit = audit;
            this.repository = repository;
        }

        public HttpResponseMessage Get()
        {
            var fields = repository.GetCustomizedFields();
            return Request.CreateResponse(fields);
        }

        [Permission(PermissionsEnum.CustomizeMandatoryFields)]
        public HttpResponseMessage Post(Model.Field field)
        {
            repository.SaveCustomizedField(field);
            audit.Log(field, EventType.CustomizeMandatoryFieldsUpdated, EventCategory.CustomizeFields);

            return Request.CreateResponse();
        }
    }
}
