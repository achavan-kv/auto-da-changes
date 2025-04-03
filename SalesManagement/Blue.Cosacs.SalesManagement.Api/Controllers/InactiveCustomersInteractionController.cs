using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [RoutePrefix("api/InactiveCustomersInteraction")]
    public class InactiveCustomersInteractionController : ApiController
    {
        private readonly ISalesManagementRepository repository;

        public InactiveCustomersInteractionController(ISalesManagementRepository repository)
        {
            this.repository = repository;
        }

        [Permission(SalesManagementPermissionEnum.InactiveCustomersInteraction)]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(repository.LoadInactiveCustomersInteraction());
        }

        [Permission(SalesManagementPermissionEnum.InactiveCustomersInteraction)]
        public HttpResponseMessage Put(Models.InactiveCustomersInteraction value)
        {
            repository.UpdateInactiveCustomersInteraction(value.ToEntitySet());

            return Request.CreateResponse();
        }
    }
}