using Blue.Cosacs.Unipay.Web.Exceptions;
using Blue.Cosacs.Unipay.Web.Formatters;
using MediatR;
using System.Threading.Tasks;
using System.Web.Http;
using Unicomer.Cosacs.Business.Commands.SaveSalesOrder;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Blue.Cosacs.Unipay.Web.Controllers.V1
{

    [JsonFormatter]
    [HandleException]
    [RoutePrefix("v1")]
    public class SalesOrderV1Controller : ApiController
    {
        private readonly IMediator mediator;
        private readonly IDbLogggerRepository dbLoggger;

        public SalesOrderV1Controller(IMediator mediator, IDbLogggerRepository dbLoggger)
        {
            this.mediator = mediator;
            this.dbLoggger = dbLoggger;
        }


        [HttpPost()]
        [Route("salesorder")]
        public async Task<SalesOrderResponseViewModel> Save([FromBody] SaveSalesOrderCommand command)
        {
            dbLoggger.LogRequest(command, Request);

            return await mediator.Send(command);
        }
    }
}