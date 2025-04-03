/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API Controller
 */

using Blue.Cosacs.Unipay.Web.Exceptions;
using Blue.Cosacs.Unipay.Web.Formatters;
using MediatR;
using System.Threading.Tasks;
using System.Web.Http;
using Unicomer.Cosacs.Business.Commands.SaveCustomer;
using Unicomer.Cosacs.Business.Queries.PreQualifications;
using Unicomer.Cosacs.Business.Queries.Qualification;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Blue.Cosacs.Unipay.Web.Controllers.V1
{
    [JsonFormatter]
    [HandleException]
    [RoutePrefix("v1")]
    public class CustomerV1Controller : ApiController
    {
        private readonly IMediator mediator;
        private readonly IDbLogggerRepository dbLoggger;

        public CustomerV1Controller(IMediator mediator, IDbLogggerRepository dbLoggger)
        {
            this.mediator = mediator;
            this.dbLoggger = dbLoggger;
        }

        [HttpGet()]
        [Route("prequalification-customer")]
        public async Task<PreQualificationResponseModel> PreQualificationAsync([FromUri] string salesCustomerId)
        {
            var request = new PreQualificationQuery
            {
                SalesCustomerId = salesCustomerId,
            };

            dbLoggger.LogRequest(request, Request);

            return await mediator.Send(request);
        }

        [HttpPost()]
        [Route("customer")]
        public async Task<CustomerResponseViewModel> Save([FromBody] SaveCustomerCommand command)
        {
            dbLoggger.LogRequest(command, Request);
            return await mediator.Send(command);
        }

        [HttpGet()]
        [Route("qualification-customer")]
        public async Task<QualificationResponseModel> QualificationAsync([FromUri] string countryIsoCode, string salesCustomerId)
        {
            var request = new QualificationQuery
            {
                CountryIsoCode = countryIsoCode,
                SalesCustomerId = salesCustomerId,
            };

            dbLoggger.LogRequest(request, Request);

            return await mediator.Send(request);
        }
    }
}
