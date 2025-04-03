/* 
Author: ArunKarthik-IGT
Date: Feb 15th
Description:JM BlueStart-Get Product Service API
 */
using Blue.Cosacs.Unipay.Web.Exceptions;
using Blue.Cosacs.Unipay.Web.Formatters;
using MediatR;
using System.Threading.Tasks;
using System.Web.Http;
using Unicomer.Cosacs.Business.Commands.ProcessDelivery;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Blue.Cosacs.Unipay.Web.Controllers.V1
{
    [JsonFormatter]
    [HandleException]
    [RoutePrefix("v1")]
    public class ProcessDeliveryV1Controller : ApiController
    {
        private readonly IMediator mediator;
        private readonly IDbLogggerRepository dbLoggger;

        public ProcessDeliveryV1Controller(IMediator mediator, IDbLogggerRepository dbLoggger)
        {
            this.mediator = mediator;
            this.dbLoggger = dbLoggger;
        }

        [HttpPost()]
        [Route("delivery")]
        public async Task<DeliveryNotificationResponseModel> Delivery([FromBody] ProcessDeliveryCommand command)
        {
            dbLoggger.LogRequest(command, Request);
            return await mediator.Send(command);
        }
    }
}