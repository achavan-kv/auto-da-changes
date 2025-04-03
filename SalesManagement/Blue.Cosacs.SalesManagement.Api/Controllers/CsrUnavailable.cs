using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Blue.Cosacs.SalesManagement.Repositories;
using Blue;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.SalesManagement.Api.Controllers
{
    [Permission(SalesManagementPermissionEnum.CSRUnavailable)] 
    [RoutePrefix("api/CsrUnavailable")]
    public class CsrUnavailableController : ApiController
    {
        private readonly ISalesManagementRepository salesManagementRepository;
        private readonly IClock clock;

        public CsrUnavailableController(IClock clock, ISalesManagementRepository salesManagementRepository)
        {
            this.salesManagementRepository = salesManagementRepository;
            this.clock = clock;
        }

        [Route("SalesPeople")]
        [HttpGet]
        public HttpResponseMessage SalesPeople()
        {
            return Request.CreateResponse<List<int>>(salesManagementRepository.GetCustomersSalesPerson(null, null)
                .Select(p => p.SalesPersonId)
                .Distinct()
                .ToList());
        }

        public HttpResponseMessage Get(int? salesPerson, DateTime? from, DateTime? to, int? numberOfRecords)
        {
            if (!numberOfRecords.HasValue)
            { 
                numberOfRecords = 15; 
            }

            return Request.CreateResponse(this.salesManagementRepository.GetCsrUnavailable(this.GetUser().Branch, salesPerson, from, to, numberOfRecords));
        }

        public HttpResponseMessage Put(Blue.Cosacs.SalesManagement.Api.Models.CsrUnavailable value)
        {
            value.Id = 0;
            value.CreatedOn = clock.Now;
            value.CreatedBy = this.GetUser().Id;
            this.salesManagementRepository.SaveCsrUnavailable(value.ToEntitySet());

            return Request.CreateResponse(System.Net.HttpStatusCode.Created);
        }

        public HttpResponseMessage Post(Blue.Cosacs.SalesManagement.Api.Models.CsrUnavailable value)
        {
            this.salesManagementRepository.SaveCsrUnavailable(value.ToEntitySet());

            return Request.CreateResponse();
        }

        public HttpResponseMessage Delete(int id)
        {
            this.salesManagementRepository.DeleteCsrUnavailable(id);

            return Request.CreateResponse();
        }
    }
}