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
using Unicomer.Cosacs.Business.Commands.LoanAdjust;
using Unicomer.Cosacs.Business.Commands.LoanPayOff;
using Unicomer.Cosacs.Business.Commands.LoanRepayment;
using Unicomer.Cosacs.Business.Commands.LoanSchedule;
using Unicomer.Cosacs.Model.ViewModels;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Blue.Cosacs.Unipay.Web.Controllers.V1
{
    [JsonFormatter]
    [HandleException]
    [RoutePrefix("v1")]
    public class AccountV1Controller : ApiController
    {
        private readonly IMediator mediator;
        private readonly IDbLogggerRepository dbLoggger;

        public AccountV1Controller(IMediator mediator, IDbLogggerRepository dbLoggger)
        {
            this.mediator = mediator;
            this.dbLoggger = dbLoggger;
        }

        [HttpGet()]
        [Route("loan-schedule")]
        public async Task<LoanResponseModel> LoanSchedule([FromUri] string cosacsAccountId)
        {
            var request = new LoanScheduleCommand
            {
                CosacsAccountId = cosacsAccountId,
            };

            dbLoggger.LogRequest(request, Request);

            return await mediator.Send(request);
        }

        [HttpPost()]
        [Route("adjust-payment")]
        public async Task<LoanResponseModel> AdjustPayment([FromBody] LoanAdjustCommand command)
        {
            dbLoggger.LogRequest(command, Request);

            return await mediator.Send(command);
        }

        [HttpPost()]
        [Route("repayment")]
        public async Task<LoanResponseModel> Repayment([FromBody] LoanRepaymentCommand command)
        {
            dbLoggger.LogRequest(command, Request);

            return await mediator.Send(command);
        }

        [HttpPost()]
        [Route("loan-payoff")]
        public async Task<LoanResponseModel> LoanPayOff([FromBody] LoanPayOffCommand command)
        {
            dbLoggger.LogRequest(command, Request);

            return await mediator.Send(command);
        }
    }
}
