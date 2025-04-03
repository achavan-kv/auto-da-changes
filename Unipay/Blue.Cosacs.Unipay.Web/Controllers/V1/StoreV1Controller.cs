/* 
Author: 
Date: 
Description:JM BlueStart-"Get Store Service" API Controller
 */
using Amazon.OpsWorks.Model;
using Blue.Cosacs.Unipay.Web.Exceptions;
using Blue.Cosacs.Unipay.Web.Formatters;
using MediatR;
using System.Threading.Tasks;
using System.Web.Http;
using Unicomer.Cosacs.Business.Queries.Store;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Blue.Cosacs.Unipay.Web.Controllers.V1
{

    [JsonFormatter]
    [HandleException]
    [RoutePrefix("v1")]
    public class StoreV1Controller : ApiController
    {
        private readonly IMediator mediator;
        private readonly IDbLogggerRepository dbLoggger;

        public StoreV1Controller(IMediator mediator, IDbLogggerRepository dbLoggger)
        {
            this.mediator = mediator;
            this.dbLoggger = dbLoggger;
        }

        [HttpGet()]
        [Route("stores")]
        public async Task<StoreResponseModel> StoreServiceAsync([FromUri] string countryIsoCode)
        {
            var request = new StoreQuery
            {
                CountryIsoCode = countryIsoCode,
            };

            dbLoggger.LogRequest(request, Request);

            return await mediator.Send(request);
        } 
    }
}