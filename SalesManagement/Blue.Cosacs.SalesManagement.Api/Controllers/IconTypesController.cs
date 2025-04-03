using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/IconTypes")]
    public class IconTypesController : ApiController
    {
        private ISalesManagementRepository salesManagementRepository;
        private IClock clock;
        private IEventStore audit;

        public IconTypesController(ISalesManagementRepository repository, IClock clock, IEventStore audit)
        {
            this.salesManagementRepository = repository;
            this.clock = clock;
            this.audit = audit;
        }

        [Permission(SalesManagementPermissionEnum.CallIcon)]
        public HttpResponseMessage Get()
        {
            var iconTypes = salesManagementRepository.GetIconTypes();
            return Request.CreateResponse(iconTypes);
        }

        [Permission(SalesManagementPermissionEnum.CallIcon)]
        public HttpResponseMessage Post(IList<IconTypes> iconTypes)
        {
            salesManagementRepository.SaveIconTypes(iconTypes);
            return Request.CreateResponse(string.Empty);
        }
    }
}